using UnityEngine;
using System.Collections;

//[ExecuteInEditMode()]
[RequireComponent (typeof (Camera))]
[AddComponentMenu("Image Effects/Fade")]


[ExecuteInEditMode]
public class ScreenFX : MonoBehaviour {
    public Material fxMaterial;
    public float colorSpeed = 0.05F;
    public float camSpeed = 0.005F;

    GameObject cityAnchor;
    Vector3 basePosition;
    Quaternion baseRotation;

    void OnRenderImage(RenderTexture src, RenderTexture dest) {
            Graphics.Blit(src, dest, fxMaterial);
        
	}

    void Start(){
        fxMaterial.SetColor("_Color2", Color.blue);
        cityAnchor = GameObject.Find("CityAnchor");
        basePosition = transform.position;
        baseRotation = transform.rotation;

    }

    void Update() {
        Color current = fxMaterial.GetColor("_Color2");
        float H;
        float S;
        float V;
        Color.RGBToHSV(current, out H, out S, out V);

        H += colorSpeed * Time.deltaTime;

        fxMaterial.SetColor("_Color2", Color.HSVToRGB(H, 0.7f, 1f));
        Vector3 newPosition = new Vector3(Mathf.Cos(cityAnchor.transform.position.z)*camSpeed, 0, 0);
        transform.SetPositionAndRotation(basePosition + newPosition, baseRotation);
    }

}
