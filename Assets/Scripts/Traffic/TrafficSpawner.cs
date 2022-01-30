using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficSpawner : MonoBehaviour
{
    [SerializeField] private Vector3 laneStart; 
    [SerializeField] private Vector3 laneEnd;
    [SerializeField] private GameObject ship;
    [SerializeField] int shipCount;
    [SerializeField] float shipSpeed;



    void Start()
    {

        for (int i = 0; i < shipCount; i++)
        {
            GameObject go = Instantiate(ship);
            Transform shipT = go.transform;
            shipT.position = transform.position;
            go.transform.parent = transform;


            shipT.position += new Vector3(
                laneStart.x + i*8 + Random.value, 
                laneStart.y, 
                laneStart.z);


            Vector3 pos = laneStart;

            InnocentShip shipSc = go.GetComponent<InnocentShip>();
            shipSc.SetShipSpeed(shipSpeed);
            shipSc.SetStart(transform);
            shipSc.SetTargetDir(laneEnd - laneStart);


        }
    }


    void OnDrawGizmos() 
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position + laneStart, .5f);
        Gizmos.DrawLine(transform.position + laneStart, transform.position + laneEnd);   
    }
}
