using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject person;
    [SerializeField] private Vector2 minMax_Count; 


    void Start()
    {
        int count = (int)Random.Range(minMax_Count.x,minMax_Count.y);

        for (int i = 0; i < count; i++)
        {
            Transform trans = Instantiate(person).transform;
            trans.position = transform.position + Random.insideUnitSphere;
            trans.position = new Vector3(trans.position.x, trans.position.y + trans.localScale.y, trans.position.z);
            trans.parent = transform;
        }
    }

}
