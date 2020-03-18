using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTrainingArea : MonoBehaviour
{
    public GameObject TrainingArea;


    // Awake() is called before all Start() methods
    void Awake()
    {
        for(int i=0; i<3; i++)
        {
            for(int j=0; j<3; j++)
            {
                Instantiate(TrainingArea, new Vector3(50 * i, 0, 50 * j), Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
