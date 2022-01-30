using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobCollisionSFX : MonoBehaviour
{
    AudioSource audioSource;
    public float decayTime = 10;
    public float maxVolume = 0.5f;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        FindObjectOfType<BlobCollision>().BlobLostPixels += BlobLostPixelsHandler;
    }

    void BlobLostPixelsHandler(int count){
        audioSource.volume = maxVolume;
    }

    void Update()
    {
        if(Time.timeScale > 0){
            audioSource.volume *= 1 - Time.deltaTime * decayTime;
        } else {
            audioSource.volume = 0;
        }
    } 
}
