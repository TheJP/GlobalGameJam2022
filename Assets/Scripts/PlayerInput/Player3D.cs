using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player3D : MonoBehaviour
{
    [SerializeField] GameObject personBullet;
    [Range(1,12)]
    [SerializeField] private float speed = 8;
    [SerializeField] private float moveSpace_X = 6;
    [SerializeField] private float rotationSpeed = 4;
    [SerializeField] private float lerpSpeed = 2;
    [SerializeField] private Vector3 lanes_Y;
    [SerializeField] private GameObject[] energyBars;

    public event Action BusDied;
    public event Action TookDamage;
    
    private Vector2 movement;
    private Vector3 origin;
    private Vector3 targetPos;
    private int peopleBullets = 6;
    private int currentLaneLevel = 1;
    private int busLife = 3;
    private float currentheight = 0;
    private bool isInAnimation = false;

    private AudioSource audioSource;

    private void Awake()
    {
        TookDamage += UpdateEnergyBar;
    }

    void Start()
    {
        origin = transform.position;
        origin = new Vector3(0,origin.y,origin.z);
        currentheight = lanes_Y[1];
        audioSource = GetComponent<AudioSource>();
    }

    
    void Update()
    {
        //float currentheight = lanes_Y[currentLaneLevel];

        targetPos = transform.position + new Vector3(movement.x,0,0) + Vector3.forward;
        //targetPos = transform.position + Vector3.forward * 5 + new Vector3(movement.x,movement.y,0) * 4;
        Quaternion targetRotation = Quaternion.LookRotation(targetPos - transform.position);
        //* Smoothly rotate towards the target point.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);


        //* Move Bus
        transform.Translate((new Vector3(movement.x,0,0) * speed) * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, currentheight, origin.z);
        //transform.Translate((new Vector3(movement.x,movement.y,0) * speed) * Time.deltaTime);
        //transform.position = new Vector3(transform.position.x, transform.position.y, origin.z);

        //* Clamp Positions 
        //* X
        if (transform.position.x > moveSpace_X + origin.x) 
            transform.position = new Vector3(moveSpace_X + origin.x, transform.position.y, origin.z);
        else if (transform.position.x < -moveSpace_X + origin.x) 
            transform.position = new Vector3(-moveSpace_X + origin.x, transform.position.y, origin.z);
        //* Y
        if (transform.position.y > lanes_Y.z) 
            transform.position = new Vector3(transform.position.x, lanes_Y.z, origin.z);
        if (transform.position.y < lanes_Y.x) 
            transform.position = new Vector3(transform.position.x, lanes_Y.x, origin.z);


    }


    public void SetMovement(Vector2 input) => movement = input;
    public void ChangeLane(float input) 
    {
        if (isInAnimation || Mathf.Abs(input) < 0.9f) return;

        if (input > 0 && currentLaneLevel < 2)
        {
            //currentLaneLevel++;
            StartCoroutine(ChangeLaneAnimation(1));
        }
        else if (input < 0 && currentLaneLevel > 0) 
        {
            //currentLaneLevel--;
            StartCoroutine(ChangeLaneAnimation(-1));
        }

        //Debug.Log("Current Height: " + lanes_Y[currentLaneLevel]);
    }



    public int GetPeople() {return peopleBullets;}
    public void ShotAction() 
    {
        if (peopleBullets > 0) 
        {
            //* instantinate people-bullet
            GameObject bullet = Instantiate(personBullet, transform.position + transform.forward * 3, Quaternion.identity);
            //* move ppb in bus forward direction
            bullet.GetComponent<Rigidbody>().AddForce(transform.forward * 15, ForceMode.Impulse);
            // play bus-bullet sound

            peopleBullets--;
        }
        else 
        {
            // play bus-empty sound
            Debug.Log("Captain!!! We're out of ammunition!!!");
        }
    } 


    void OnTriggerEnter(Collider other) 
    {
        // Debug.Log("Collision");
        if (other.CompareTag("Person"))
        {
            //* load person
            this.peopleBullets++;
            Destroy(other.gameObject);
        }
        else 
        {
            //* TAKE DAMAGE
            audioSource.Play();
            Debug.Log("CAPTAIN!!! WE HAVE TAKEN DAMAGE!!!!");
            busLife--;
            TookDamage?.Invoke();
        }

    }

    void UpdateEnergyBar()
    {
        for (int i = 0; i < energyBars.Length; i++)
        {
            bool isActive = busLife > i;
            energyBars[i].SetActive(isActive);
        }

        if (busLife <= 0)
        {
            //* Kill the bus
            BusDied?.Invoke();
            Debug.Log("WE LOST THE BUS!!!!");
        }
    }

    
    IEnumerator ChangeLaneAnimation(int laneDiff)
    {
        float startime = Time.time;
        float targetHeight = lanes_Y[currentLaneLevel + laneDiff];
        float traveledDist = 0;
        float heighDiff = lanes_Y[2] - lanes_Y[1];
        
        isInAnimation = true;
        while (traveledDist < heighDiff && ((Time.time - startime) * lerpSpeed) < 1f) 
        { 
            float move = Mathf.Lerp (0,1, (Time.time - startime) * lerpSpeed);

            currentheight += (lerpSpeed*laneDiff);
            traveledDist += lerpSpeed;
            
            yield return null;
        }

        currentLaneLevel += laneDiff;
        currentheight = lanes_Y[currentLaneLevel];
        isInAnimation = false;
    }
    
}
