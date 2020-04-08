using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxPool : MonoBehaviour
{

    private int numberOfBoxes = 40;
    private Transform[] boxes;
    public GameObject box;
    private int i = 0;
    private int spawnTime = 0;
    private float spawnDelay = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        boxes = new Transform[numberOfBoxes];

        InvokeRepeating("SpawnObject", spawnTime, spawnDelay);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Transform FindBox()
    {
        return boxes[Random.Range(0, i)];
    }

    private void SpawnObject()
    {
        if (i >= numberOfBoxes) return;
        GameObject b = Instantiate(box, new Vector3(Random.Range(-10f, 20.0f), Random.Range(8, 25), Random.Range(10.0f, 35.0f)), Quaternion.identity);
        boxes[i] = b.transform;
        i++;
    }
}
