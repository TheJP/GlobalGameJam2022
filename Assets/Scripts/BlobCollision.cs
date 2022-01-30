using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlobCollision : MonoBehaviour
{
    [field: SerializeField]
    public RenderTexture SceneRender { get; set; }

    [field: SerializeField]
    public RectTransform Blob { get; set; }

    [field: SerializeField]
    public RectTransform BlobCanvas { get; set; }

    [field: SerializeField]
    public Canvas Canvas { get; set; }

    [field: Tooltip("Collision radius of Blob relative to Screen.width.")]
    [field: SerializeField]
    public float CollisionRadius { get; set; } = 0.01f;

    [field: Tooltip("How fast the pixels of the Blob fade away")]
    [field: SerializeField]
    public float FadeOutSpeed { get; set; } = 0.1f;

    private Texture2D blobTexture;
    public const float BlobRadius = 90;
    const float BlobRadiusSquared = BlobRadius * BlobRadius;
    const int TextureSide = 200;
    const float Epsilon = 1e-5f;

    private readonly HashSet<(int, int)> alivePixels = new HashSet<(int, int)>();
    private readonly HashSet<(int,int)> deadPixels = new HashSet<(int, int)>();
    private readonly HashSet<(int, int)> dyingPixels = new HashSet<(int, int)>();

    private float startTime;

    public bool IsDead { get; private set; } = false;
    public event Action BlobDied;
    public event Action<int> BlobLostPixels;

    public void Start()
    {
        SceneRender.width = Screen.width;
        SceneRender.height = Screen.height;

        startTime = Time.time;

        var circleTexture = new Texture2D(TextureSide, TextureSide);
        for (int y = 0; y < TextureSide; ++y)
        {
            for (int x = 0; x < TextureSide; ++x)
            {
                var inCircle = new Vector2(x - TextureSide / 2, y - TextureSide / 2).SqrMagnitude() < BlobRadiusSquared + Epsilon;
                circleTexture.SetPixel(x, y, inCircle ? Color.black : Color.clear);
                if (inCircle) { alivePixels.Add((x, y)); }
            }
        }
        circleTexture.Apply();
        Blob.GetComponent<RawImage>().texture = circleTexture;
        blobTexture = circleTexture;
    }

    static public Texture2D GetRTPixels(RenderTexture rt)
    {
        // Remember currently active render texture
        RenderTexture currentActiveRT = RenderTexture.active;

        // Set the supplied RenderTexture as the active one
        RenderTexture.active = rt;

        // Create a new Texture2D and read the RenderTexture image into it
        Texture2D tex = new Texture2D(rt.width, rt.height);
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);

        // Restorie previously active render texture
        RenderTexture.active = currentActiveRT;
        return tex;
    }

    public void DebugIntersection(Vector2Int min, Vector2Int max)
    {
        var scene = GetRTPixels(SceneRender);
        var pixels = scene.GetPixels();
        for (int y = min.y; y < max.y; ++y)
        {
            for (int x = min.x; x < max.x; ++x)
            {
                var index = y * Screen.width + x;
                if (pixels[index].grayscale < 0.5)
                {
                    Debug.DrawLine(
                        Camera.main.ScreenToWorldPoint(new Vector3(x, y, 100)),
                        Camera.main.ScreenToWorldPoint(new Vector3(x + 1f, y + 1f, 100)),
                        Color.green);
                }
            }
        }
    }

    public void DebugIntersection() => DebugIntersection(new Vector2Int(0, 0), new Vector2Int(Screen.width, Screen.height));

    public static void ClampToScreen(ref Vector2Int v)
    {
        v.x = Mathf.Clamp(v.x, 0, Screen.width - 1);
        v.y = Mathf.Clamp(v.y, 0, Screen.height - 1);
    }

    private Vector2Int ScreenToTextureSpace(Vector3 point)
    {
        // Screen to Viewport
        point = Camera.main.ScreenToViewportPoint(point);

        // Viewport to Canvas
        point.x *= BlobCanvas.sizeDelta.x;
        point.y *= BlobCanvas.sizeDelta.y;

        // Canvas to Blob-Space
        point.x -= Blob.anchoredPosition.x - Blob.pivot.x * Blob.sizeDelta.x;
        point.y -= Blob.anchoredPosition.y - Blob.pivot.y * Blob.sizeDelta.y;

        // Blob-Space to Texture-Space
        point.x = (point.x / Blob.sizeDelta.x) * TextureSide;
        point.y = (point.y / Blob.sizeDelta.y) * TextureSide;

        return new Vector2Int((int)point.x, (int)point.y);
    }

    public void Update()
    {
        // Disable on pause
        if (Time.timeScale < Epsilon)
        {
            startTime = Time.time;
            return;
        }

        // Let TextureRenderer start up.
        if (Time.time - startTime < 0.5f)
        {
            return;
        }

        // Find dying pixels and progress dying.
        var dead = new List<(int, int)>();
        foreach (var (x, y) in dyingPixels)
        {
            var colour = blobTexture.GetPixel(x, y);
            float animateColour(float channel) => Mathf.Max(0f, channel - FadeOutSpeed * Time.deltaTime);
            colour.r = animateColour(colour.r);
            colour.g = animateColour(colour.g);
            colour.b = animateColour(colour.b);
            colour.a = animateColour(colour.a);
            if (colour.a <= 0)
            {
                dead.Add((x, y));
                colour = Color.clear;
            }
            blobTexture.SetPixel(x, y, colour);
        }

        foreach (var d in dead)
        {
            dyingPixels.Remove(d);
        }

        if (IsDead)
        {
            blobTexture.Apply();
            return;
        }

        // Detect collision with blob:
        // Pixels die if they do.
        int count = 0;
        var blobPosition = Blob.transform.position;
        if (Canvas == null && Canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            blobPosition = Camera.main.WorldToScreenPoint(blobPosition);
        }
        var pixelRadius = Screen.width * CollisionRadius;
        var pixelRadiusSquared = pixelRadius * pixelRadius;
        var min = new Vector2Int((int)(blobPosition.x - pixelRadius), (int)(blobPosition.y - pixelRadius));
        var max = new Vector2Int((int)(blobPosition.x + pixelRadius), (int)(blobPosition.y + pixelRadius));
        ClampToScreen(ref min);
        ClampToScreen(ref max);
        var middle = new Vector2(blobPosition.x, blobPosition.y);

        var scene = GetRTPixels(SceneRender);
        var screenPixels = scene.GetPixels();
        Debug.Assert(screenPixels.Length == Screen.width * Screen.height);
        for (int y = min.y; y < max.y; ++y)
        {
            for (int x = min.x; x < max.x; ++x)
            {
                if ((middle - new Vector2(x, y)).SqrMagnitude() > pixelRadiusSquared + Epsilon)
                {
                    continue;
                }

                //var index = y * Screen.width + x;
                //if (pixels[index].grayscale < 0.5)
                //{
                //    Debug.DrawLine(
                //        Camera.main.ScreenToWorldPoint(new Vector3(x, y, 100)),
                //        Camera.main.ScreenToWorldPoint(new Vector3(x + 1f, y + 1f, 100)),
                //        Color.green);
                //}

                var index = y * Screen.width + x;
                var pixelCollides = screenPixels[index].grayscale < 0.5;

                if (!pixelCollides)
                {
                    continue;
                }

                var point1 = ScreenToTextureSpace(new Vector3(x, y, 100));
                var point2 = ScreenToTextureSpace(new Vector3(x + 1f, y + 1f, 100));

                for (int v = point1.y - 1; v <= point2.y; ++v)
                {
                    for (int u = point1.x - 1; u <= point2.x; ++u)
                    {
                        if (deadPixels.Contains((u,v))) { continue; }
                        if (!alivePixels.Contains((u,v))) { continue; }
                        if (blobTexture.GetPixel(u,v) == Color.clear) { continue; }
                        ++count;
                        alivePixels.Remove((u,v));
                        deadPixels.Add((u,v));
                        dyingPixels.Add((u,v));
                        blobTexture.SetPixel(u, v, Color.red);
                    }
                }
            }
        }

        blobTexture.Apply();

        if (count > 0)
        {
            BlobLostPixels?.Invoke(count);
        }

        if (alivePixels.Count * 50 <= deadPixels.Count)
        {
            Debug.Log("Blob Died!!!");
            IsDead = true;
            BlobDied?.Invoke();
        }
    }
}
