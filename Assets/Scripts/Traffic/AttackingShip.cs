using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingShip : MonoBehaviour
{
    private float speed = 16;
    private Vector3 origin;
    //private Vector3 startPos;
    private Vector3 endPos;
    private Vector3 dir;

    private void Start() 
    {
        origin = transform.position;
    }

    void Update()
    {
        //endPos = startPos + dir/2;
        
        //* Move a step closer to the target.
        float step =  speed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, endPos, step);
        transform.LookAt(endPos);

        //* Respawn if of camera
        if (transform.position.z == endPos.z) 
        {
            Destroy(this.gameObject);
        }
    }



    //public void SetStart(Vector3 t) => startPos = t;
    public void SetTarget(Vector3 t) => endPos = t;

    //* Set speed by spawner
    public void SetShipSpeed(float newSpeed) => speed = newSpeed;



    private void OnCollisionEnter(Collision other) 
    {
        Destroy(this.gameObject);
    }


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
