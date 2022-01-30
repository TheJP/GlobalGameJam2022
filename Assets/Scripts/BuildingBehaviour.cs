using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBehaviour : MonoBehaviour
{
    float viewDistanceBack = 0;
    // Start is called before the first frame update
    void Start()
    {
        CityMovement movement = GameObject.Find("CityAnchor").GetComponent<CityMovement>();
        viewDistanceBack = movement.preloadBack * movement.gridSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.z < -viewDistanceBack)
        {
            Destroy(gameObject);
        }
    }
}
