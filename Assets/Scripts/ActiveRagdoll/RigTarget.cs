using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigTarget : MonoBehaviour
{
    /// <summary>
    /// This is for controlling inverse kinematics.
    /// </summary>

    private Vector3 spawnPosition;


    // Start is called before the first frame update
    void Start()
    {
        spawnPosition = transform.localPosition;
    }


    public void MoveTowards(Vector3 position, float distance)
    {
        Vector3 dir = (position - this.transform.position);
        Vector3 vec = Vector3.ClampMagnitude(dir, distance);
        this.transform.position += vec;
    }

    public void MoveTowardsSpawnPosition(float distance)
    {
        Vector3 dir = (spawnPosition - this.transform.localPosition);
        Vector3 vec = Vector3.ClampMagnitude(dir, distance);
        this.transform.localPosition += vec;
    }

    public bool isAtSpawnPosition()
    {
        return (spawnPosition - this.transform.localPosition).magnitude < 0.1f;
    }
}
