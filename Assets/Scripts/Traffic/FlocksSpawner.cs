using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//* Spawn Ships 
public class FlocksSpawner : MonoBehaviour
{
    [SerializeField] private Vector3 laneStart; 
    [SerializeField] private Vector3 laneEnd;
    [SerializeField] private GameObject ship;
    [SerializeField] private GameObject airShip;
    [SerializeField] int shipCount;
    [SerializeField] float shipSpeed;
    [SerializeField] Vector2 minMax_SpawnPeriod;
    [Space]
    [SerializeField] float width;
    [SerializeField] float heigtDiff;

    private float xPos = 0;
    private float yPos = 0;

    void Start()
    {
        StartCoroutine(Respawn());
    }


    private void SpawnNewFlock()
    {
        yPos = Random.value > .5f ? -heigtDiff : heigtDiff; //* bottom or top
        xPos = Random.value > .5f ? -width : width; //* left or right

        for (int i = 0; i < shipCount; i++)
        {
            //if (Random.value > .5f) continue;
            
            GameObject go = null;
            if (yPos < 0 ) go = Instantiate(ship);
            else go = Instantiate(airShip);

            Transform shipT = go.transform;
            float addedTo_X = GetOne(-1,1);

            shipT.position = new Vector3(
                    xPos + addedTo_X,
                    laneStart.y + yPos, 
                    laneStart.z + i*8);

            go.transform.parent = transform;
    
            
            Vector3 pos = laneStart;
            
            AttackingShip shipSc = go.GetComponent<AttackingShip>();
            shipSc.SetShipSpeed(shipSpeed);
            //shipSc.SetStart(laneStart);
            shipSc.SetTarget(laneEnd + new Vector3(xPos+addedTo_X, yPos, 0));
        }
    }


    IEnumerator Respawn() 
    {
        float timer = Random.Range(minMax_SpawnPeriod.x, minMax_SpawnPeriod.y);
        //Debug.Log("Wait for " + timer);
        yield return new WaitForSeconds(timer);

        //Debug.Log("SPAWN - " + Time.time);
        SpawnNewFlock();

        StartCoroutine(Respawn());
    }


    private float GetOne(float x, float y)
    {
        return Random.value > 0.5f ? x : y;
    }

    void OnDrawGizmos() 
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position + laneStart, .5f);
        Gizmos.DrawLine(transform.position + laneStart, transform.position + laneEnd);   
    }
}
