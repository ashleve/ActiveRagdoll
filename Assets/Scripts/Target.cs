using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{

    GameObject[] observers;

    public string STATE = "FREE";
    public int numOfAttached = 0;

    private int spawnRangeX = 20;
    private int spawnRangeY = 6;
    private int spawnRangeZ = 20;

    private bool fallen = false;


    // Awake() is called before all Start() methods
    void Awake()
    {
        //observers = this.GetComponentsInParent<GameObject>();
    }

    // Unity method for physics update
    void FixedUpdate()
    {
    }


    public void Respawn()
    {
        float x = Random.value * spawnRangeX - (spawnRangeX / 2);
        float y = Random.value * spawnRangeY + 4;
        float z = Random.value * spawnRangeZ - (spawnRangeZ / 2);

        // Move the target to a new spot
        this.transform.localPosition = new Vector3(x, y, z);

        Rigidbody rb = this.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        numOfAttached = 0;

        //NotifyAgents();
    }

    public void NotifyAgents()
    {
        foreach (GameObject agent in observers)
            ;
            //agent.UpdateTargetStatus();
    }


}
