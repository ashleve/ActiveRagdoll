using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    // Start is called before the first frame update

    public bool isTaken = false;
    public int hostID = -1;

    void Start()
    {
        isTaken = false;
        hostID = -1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
