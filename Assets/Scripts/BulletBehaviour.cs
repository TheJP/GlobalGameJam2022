using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    private float lifeTime = 10;
    private int bulletHitCount = 3;
    private IEnumerator destoryCoroutine;

    void Start()
    {
        destoryCoroutine = DestroyAfterTime(lifeTime);
        StartCoroutine(destoryCoroutine);
    }

    void OnTriggerEnter(Collider other) 
    {
        
        //* reset destroy coroutine
        StopCoroutine(destoryCoroutine);
        
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent<InnocentShip>(out InnocentShip iShip))
            {
                iShip.DisableTransform(true);
            }
            else if (other.TryGetComponent<AttackingShip>(out AttackingShip aShip))
                aShip.DisableTransform(true);
            
            bulletHitCount--;
            if (bulletHitCount <= 0) 
            {
                DisableTransform();
                StartCoroutine(DestroyAfterTime(5.0f)); //* spark & thunder time
            }

            //* Bullet hit enemy
                //* sound
                //* effect
        }
        else 
        {
            DisableTransform();
            StartCoroutine(DestroyAfterTime(5.0f)); //* spark & thunder time

            //* Bullet hit wall
                //* sound
                //* effect
        }


    }


    //* Destroy after Time
    private IEnumerator DestroyAfterTime(float lifeTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(lifeTime);
            Destroy(this.gameObject);
        }
    }

    private void DisableTransform()
    {
        this.GetComponent<MeshRenderer>().enabled = false;
        this.GetComponent<SphereCollider>().enabled = false;
    }
    
}
