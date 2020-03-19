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

    public Target[] allTargets;


    // Start is called before the first frame update
    void Awake()
    {
        targetContainer = new GameObject();
        targetContainer.transform.SetParent(this.transform);
        targetContainer.transform.localPosition = new Vector3(0, 0, 0);
        targetContainer.name = "TargetContainer";

        SpawnTargets();
    }

    // Unity method for physics update
    void FixedUpdate()
    {
        
    }



    public void SpawnTargets()
    {
        allTargets = new Target[numOfTargets];
        for (int i=0; i<numOfTargets; i++)
        {
            GameObject obj = Instantiate(Target, Vector3.zero, Quaternion.identity, targetContainer.transform);  // spawn target in container
            Target target = obj.GetComponent<Target>();
            target.SetSpawnRange(spawnRangeX, spawnRangeY, spawnRangeZ);
            target.Respawn();
            allTargets[i] = target;
        }
    }
    
    public void RespawnTargets()
    {
/*
        foreach (Transform target in allTargets)
        {
            target.GetComponent<Target>().Respawn();
        }
*/
        DestroyTargets();
        SpawnTargets();
    }

    public void DestroyTargets()
    {
        foreach (var target in allTargets)
            Destroy(target.gameObject);
    }

    public Target GetRandomTarget()
    {
        return allTargets[Random.Range(0, allTargets.Length - 1)];
    }

    public Target GetRandomFreeTarget()
    {
        Target target;
        while (true)
        {
            target = allTargets[Random.Range(0, allTargets.Length - 1)];
            if (target.observers.Count == 0) 
                break;
        }
        return target;
    }

    public Target GetClosestTarget()
    {

        float minDist = (allTargets[0].transform.position - this.transform.position).magnitude;
        int index = 0;
        for (int i = 1; i < allTargets.Length; i++)
        {
            float dist = (allTargets[i].transform.position - this.transform.position).magnitude;
            if (dist < minDist)
            {
                minDist = dist;
                index = i;
            }
        }

        return allTargets[index];
    }

}
