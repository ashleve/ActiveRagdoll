using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{

    public List<GameObject> observers;

    public int numOfAttached = 0;

    private float spawnRangeX = 20;
    private float spawnRangeY = 6;
    private float spawnRangeZ = 20;


    // Awake() is called before all Start() methods
    void Awake()
    {
        observers = new List<GameObject>();
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

    public void SetSpawnRange(float x, float y, float z)
    {
        spawnRangeX = x;
        spawnRangeY = y;
        spawnRangeZ = z;
    }


}
