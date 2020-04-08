using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class MasterController : MonoBehaviour
{

    public Animator anim;
    private float rotationSpeed = 200f;
    private float runSpeed = 3f;
    private float walkSpeed = 1.5f;

    public Transform slave;
    public Transform floor;
    public AnimationFollowing animFollow;
    public SlaveController slaveController;
    public Transform box;
    public Transform IKTarget;
    public RigBuilder rigBuilder;

    private Quaternion deltaRotateLeft;
    private Quaternion deltaRotateRight;

    public Transform centralPoint;
    //public Transform armTargetTarget;
    public Transform rightArmTarget;
    public Transform leftArmTarget;

    //public bool handsConnected = false;
    public int handsConnected = 0;


    // Start is called before the first frame update
    void Start()
    {
        Vector3 rotationLeft = new Vector3(0f, -rotationSpeed, 0f);
        Vector3 rotationRight = new Vector3(0f, rotationSpeed, 0f);

        rotationLeft = -rotationLeft.normalized * -rotationSpeed;
        rotationRight = rotationRight.normalized * rotationSpeed;

        deltaRotateLeft = Quaternion.Euler(rotationLeft * Time.fixedDeltaTime);
        deltaRotateRight = Quaternion.Euler(rotationRight * Time.fixedDeltaTime);

        animFollow = transform.root.GetComponentInChildren<AnimationFollowing>();
        slaveController = transform.root.GetComponentInChildren<SlaveController>();
        rigBuilder = transform.root.GetComponentInChildren<RigBuilder>();

        //InvokeRepeating("FindBox", 4, 7);
        Invoke("FindBox", 2);

        DisableIK();
        handsConnected = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        transform.position = new Vector3(transform.position.x, floor.position.y + 0.6f, transform.position.z);


        /*
                if (slaveController.interpolationStep < 1) return;

                if(!animFollow.isAlive)
                {
                    transform.position = new Vector3(slave.position.x, transform.position.y, slave.position.z);
                    Vector3 newRotation = transform.eulerAngles;
                    newRotation.y = slave.transform.eulerAngles.y;
                    transform.eulerAngles = newRotation;
                }
        */


        anim.SetInteger("Cond", 2);
        RotateTowards();
        MoveForward();


        /*
                anim.SetInteger("Cond", 0);

                // Rotation
                if (Input.GetKey(KeyCode.A))
                {
                    transform.rotation = transform.rotation * deltaRotateLeft;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    transform.rotation = transform.rotation * deltaRotateRight;
                }

                // Move Forward
                if (Vector3.Magnitude(slave.position - (transform.position + transform.forward)) < 1.5f)
                {
                    if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
                    {
                        anim.SetInteger("Cond", 2);
                        transform.position += transform.forward * Time.fixedDeltaTime * runSpeed;
                    }
                    else if (Input.GetKey(KeyCode.W))
                    {
                        anim.SetInteger("Cond", 1);
                        transform.position += transform.forward * Time.fixedDeltaTime * walkSpeed;
                    }
                }

                if (Input.GetKey(KeyCode.S))
                {
                    anim.SetInteger("Cond", 1);
                    transform.position += -transform.forward * Time.fixedDeltaTime * walkSpeed;
                }
        */



        /*
                Vector3 direction = (box.position - centralPoint.position).normalized;
                armTargetTarget.position = centralPoint.position + 0.4f*direction;

                rightArmTarget.position += (armTargetTarget.position - rightArmTarget.position).normalized / 1000;
                leftArmTarget.position += (armTargetTarget.position - leftArmTarget.position).normalized / 1000;
                //Debug.DrawRay(transform.position, collision.relativeVelocity, Color.red, 2f, true);
        */

        /*        if (handsConnected)
                {
                    if ((rightArmTarget.position - centralPoint.position + leftArmTarget.position - centralPoint.position).magnitude > 0.5f)
                    {
                        Vector3 direction = ((centralPoint.position - leftArmTarget.position).normalized + (centralPoint.position - rightArmTarget.position).normalized)/2;


                        leftArmTarget.position += direction / 100;
                        rightArmTarget.position += direction / 100;
                    }

                }else*/

        // HERE COMES THE SPAGHETTIIII
        if(handsConnected == 1)
        {
            EnableIK();
            MoveLeftHandTowardsBox();
            MoveRightHandTowardsBox();
            return;
        }

       

            if (handsConnected < 2 && (leftArmTarget.position - centralPoint.position).magnitude < 1.1f)
            {
                if((leftArmTarget.position - box.position).magnitude > 0.2f && (box.position - leftArmTarget.position).magnitude < 4f)
                {

                    EnableIK();
                    MoveLeftHandTowardsBox();
                }
            }

            if(handsConnected < 2 && (rightArmTarget.position - centralPoint.position).magnitude < 1.1f)
            {
                if ((rightArmTarget.position - box.position).magnitude > 2f && (box.position - rightArmTarget.position).magnitude < 4f) 
                {
                    EnableIK();
                    MoveRightHandTowardsBox();
                }
            }

        if(handsConnected == 2)
        {
            leftArmTarget.position = centralPoint.position + new Vector3(0, 0.7f, 0);
            EnableIK();
        }

        //IKTarget.position = box.position;
    }

    public void DisableIK()
    {
        foreach(var l in rigBuilder.layers)
        {
            l.active = false;
        }
    }

    public void EnableIK()
    {
        foreach (var l in rigBuilder.layers)
        {
            l.active = true;
        }
    }





    private void MoveForward()
    {
       
        //transform.position += transform.forward * Time.fixedDeltaTime * runSpeed;
        transform.position += transform.forward * Time.fixedDeltaTime * runSpeed;
    }

    private void RotateTowards()
    {
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, box.position - transform.position, 0.04f, 5f);

        // Draw a ray pointing at our target in
        Debug.DrawRay(transform.position, box.position - transform.position, Color.red);
        
        /*
                // XDDD
                //newDirection.x = transform.eulerAngles.x;
                newDirection.y = transform.eulerAngles.y;
                //newDirection.z = transform.eulerAngles.z;
        */

        // Calculate a rotation a step closer to the target and applies rotation to this object
        Quaternion rot = Quaternion.LookRotation(newDirection);
        Vector3 tmp = rot.eulerAngles;
        tmp.x = transform.rotation.eulerAngles.x;
        tmp.z = transform.rotation.eulerAngles.z;
        transform.rotation = Quaternion.Euler(tmp);
    }

    private void MoveLeftHandTowardsBox()
    {
        Vector3 direction = (box.position - leftArmTarget.position).normalized;
        leftArmTarget.position += direction / 80;
    }

    private void MoveRightHandTowardsBox()
    {
        Vector3 direction = (box.position - rightArmTarget.position).normalized;
        rightArmTarget.position += direction / 80;
    }


    private void FindBox()
    {
        box = floor.GetComponent<BoxPool>().FindBox();
    }

    public void ConnectHandsToBox()
    {

    }


}
