using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityMovement : MonoBehaviour
{
    public float velocity = 1;
    public int step = 0;
    public int preloadFront = 5;
    public int preloadBack = 2;
    public Transform[] buildingPrefabs;
    public Transform startBuildingPrefab;
    public Transform restartBuildingPrefab;
    public Transform backgroundPrefab;
    public float gridSize = 90;

    // Start is called before the first frame update
    void Start()
    {
        bool didRestart = RestartDetector.didRestart;

        // create first few modules, behind player
        for (int z = -preloadBack; z < 0; z++)
        {
            CreateBuildings(z*gridSize, GetRandomBuilding(), true);
        }

        // create start module
        if (didRestart)
        {  
            CreateBuildings(0, restartBuildingPrefab, false);
        }
        else
        {
            CreateBuildings(0, startBuildingPrefab, false);
        }

        // create module after start
        if (didRestart)
        {  
            CreateBuildings(gridSize, GetRandomBuilding(), true);
        }
        else
        {
            CreateBuildings(gridSize, restartBuildingPrefab, false);
        }

        // create modules after player
        for (int z = 2; z < preloadFront; z++)
        {
            CreateBuildings(z*gridSize, GetRandomBuilding(), true);
        }            
    }

    // Update is called once per frame
    void Update()
    {
        // movement
        transform.Translate(Vector3.back * velocity * Time.deltaTime);

        // check if new building has to be generated
        if (Mathf.FloorToInt(Mathf.Abs(transform.position.z)/gridSize) > step)
        {
            CreateBuildings(preloadFront*gridSize, GetRandomBuilding(), true);
            step = Mathf.FloorToInt(Mathf.Abs(transform.position.z)/gridSize);
        }
    }

    void CreateBuildings(float z, Transform prefabBuilding, bool createBackground)
    {
        Instantiate(prefabBuilding, new Vector3(0, 0, z+gridSize/2), Quaternion.identity, transform);

        if (createBackground)
        {
            Instantiate(backgroundPrefab, new Vector3(0, 0, z+gridSize/2), Quaternion.identity, transform);
        }

    }

    Transform GetRandomBuilding()
    {
       return buildingPrefabs[((uint)Random.Range(0, buildingPrefabs.GetLength(0)))];
    }
}
