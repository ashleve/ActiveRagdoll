using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{

    public GameObject Target;
    public int numOfTargets = 5;
    private int spawnRangeX = 20;
    private int spawnRangeY = 6;
    private int spawnRangeZ = 20;

    private GameObject targetContainer;

    public Transform[] allTargets;


    // Start is called before the first frame update
    void Awake()
    {
        targetContainer = new GameObject();
        targetContainer.transform.SetParent(this.transform);
        targetContainer.transform.localPosition = new Vector3(0, 0, 0);

        spawnTargets();
    }

    // Unity method for physics update
    void FixedUpdate()
    {
        
    }



    public void spawnTargets()
    {
        allTargets = new Transform[numOfTargets];
        for (int i=0; i<numOfTargets; i++)
        {
            GameObject obj = Instantiate(Target, Vector3.zero, Quaternion.identity, targetContainer.transform);  // spawn target in container
            obj.GetComponent<Target>().Respawn();
            allTargets[i] = obj.transform;
        }
    }
    
    public void RespawnTargets()
    {
        /*        foreach(Transform target in allTargets)
                {
                    target.GetComponent<Target>().Respawn();
                }*/
        DestroyTargets();
        spawnTargets();
    }

    public void DestroyTargets()
    {
        foreach (var target in allTargets)
            Destroy(target.gameObject);
    }

    public void hideTarget(int index)
    {
        allTargets[index].position = new Vector3(-100, -100, -100);
        //allTargets[index].GetComponent<Renderer>().enabled = false;
    }




}
