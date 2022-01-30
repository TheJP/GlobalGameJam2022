using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player2D : MonoBehaviour
{
    [SerializeField] private float speed = 6;
    [SerializeField] private float acc = 0.5F;

    [SerializeField] private RectTransform canvas;
    [SerializeField] private RectTransform blob;

    private Vector2 inputMovement;
    private Vector2 velocity = new Vector2(0,0);
    private Rigidbody rb;


    void Awake() 
    {
        
    }

    

    void Update()
    {
        velocity += (inputMovement-velocity)*acc;
        float velocityLength = velocity.magnitude;
        if (velocityLength > 1)
        {
            velocity = velocity.normalized;
        }
        transform.Translate((velocity * speed) * Time.deltaTime);

        // clamp into screen size
        blob.anchoredPosition = new Vector2(
            Mathf.Clamp(blob.anchoredPosition.x, BlobCollision.BlobRadius/2, canvas.rect.width - BlobCollision.BlobRadius/2),
            Mathf.Clamp(blob.anchoredPosition.y, BlobCollision.BlobRadius/2, canvas.rect.height - BlobCollision.BlobRadius/2)
        );

    
    }



    public void SetMovement(Vector2 input) => inputMovement = input;


}
