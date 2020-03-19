using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private float rotationSpeed = 1.5f;
    private float runSpeed = 2.5f;
    private float walkSpeed = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        
    }

    public void MoveForward()
    {
        float singleStep = runSpeed * Time.fixedDeltaTime;
        transform.position += transform.forward * singleStep;
    }

    public void RotateTowards(Transform target)
    {
        Vector3 targetDirection = target.position - transform.position;

        // The step size is equal to speed times frame time.
        float singleStep = rotationSpeed * Time.fixedDeltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
        newDirection.y = 0;

        transform.rotation = Quaternion.LookRotation(newDirection);
    }

}
