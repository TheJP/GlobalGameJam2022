using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnocentShip : MonoBehaviour
{
    private float speed = 16;
    private Transform startTrans;
    private Vector3 endPos;
    private Vector3 dir;
    private bool isFlock = false;


    void Update()
    {
        endPos = startTrans.position + dir/2;

        
        //* Move a step closer to the target.
        float step =  speed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, endPos, step);
        transform.LookAt(endPos);

        //* Respawn if of camera
        if (transform.position.x == endPos.x) 
        {
            if (isFlock) Destroy(this.gameObject);
            else
            {
                transform.position = new Vector3(-endPos.x, endPos.y, endPos.z);
                DisableTransform(false);
            }
        }
    }



    public void SetStart(Transform t) => startTrans = t;
    public void SetTargetDir(Vector3 t) => dir = t;
    public void SetFlock(bool flock) => isFlock = flock;

    //* Set speed by spawner
    public void SetShipSpeed(float newSpeed) => speed = newSpeed;


    //* if hit disable until respawn
    public void DisableTransform(bool state)
    {
        if (state == true) {
            //* Ship go hit!
                //* Sound
                //* Effects
            transform.GetComponent<MeshRenderer>().enabled = false;
            transform.GetComponent<BoxCollider>().enabled = false;
        }
        else {
            transform.GetComponent<MeshRenderer>().enabled = true;
            transform.GetComponent<BoxCollider>().enabled = true;
        }
    }


}
