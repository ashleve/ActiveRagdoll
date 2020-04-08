using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class MasterController : MonoBehaviour
{

    /// <summary>
    /// Manages Inverse Kinematics and position of static animation.
    /// </summary>

    public Animator anim;
    private float rotationSpeed = 0.01f;
    private float runSpeed = 3f;
    private float walkSpeed = 1.5f;

    public Transform slave;
    public Transform floor;
    public AnimationFollowing animFollow;
    public SlaveController slaveController;
    public Transform IKTarget;
    public RigBuilder rigBuilder;
    public Transform box;

    private Quaternion deltaRotateLeft;
    private Quaternion deltaRotateRight;

    public Transform centralPoint;
    public Transform rightArmTarget;
    public Transform leftArmTarget;

    //public bool handsConnected = false;
    public int handsConnected = 0;

    float heightOffset;
    float frontOffset;


    // Start is called before the first frame update
    void Start()
    {
        Vector3 rotationLeft = new Vector3(0f, -rotationSpeed, 0f);
        Vector3 rotationRight = new Vector3(0f, rotationSpeed, 0f);

        rotationLeft = -rotationLeft.normalized * -rotationSpeed;
        rotationRight = rotationRight.normalized * rotationSpeed;

        deltaRotateLeft = Quaternion.Euler(rotationLeft * Time.fixedDeltaTime);
        deltaRotateRight = Quaternion.Euler(rotationRight * Time.fixedDeltaTime);

        HumanoidSetUp setUp = this.GetComponentInParent<HumanoidSetUp>();
        animFollow = setUp.GetAnimationFollowing();
        slaveController = setUp.GetSlaveController();
        rigBuilder = transform.root.GetComponentInChildren<RigBuilder>();

        InvokeRepeating("FindBox", 2f, 10); // Choose new box every 10 seconds
        //Invoke("FindBox", 1f);

        DisableIK();
        handsConnected = 0;

        // This is dumb but it will make arm movement more random when grabbing boxes
        Vector3 tmp = rightArmTarget.position;
        tmp += new Vector3(Random.Range(0f, 0.05f), Random.Range(0f, 0.1f), Random.Range(0f, 0.3f));
        rightArmTarget.position = tmp;

        tmp = leftArmTarget.position;
        tmp += new Vector3(Random.Range(0f, 0.05f), Random.Range(0f, 0.1f), Random.Range(0f, 0.3f));
        leftArmTarget.position = tmp;

        heightOffset = Random.Range(0.5f, 0.9f);
        frontOffset = Random.Range(0.0f, 0.3f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (box == null) return;

        if (!animFollow.isAlive) return;

        transform.position = new Vector3(transform.position.x, floor.position.y + 0.6f, transform.position.z);


        anim.SetInteger("Cond", 2);
        RotateTowards();
        MoveForward();


        // HERE COMES THE SPAGHETTIIII
        // those are really dumb IK rules beacause im to lazy to do it in a professional way so I just spammed if statements until it started to work
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
            leftArmTarget.position = centralPoint.position + new Vector3(0, heightOffset, frontOffset);
            EnableIK();
        }
        // END OF SPAGHETTI

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
        transform.position += transform.forward * Time.fixedDeltaTime * runSpeed;
    }

    private void RotateTowards()
    {
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, box.position - transform.position, rotationSpeed, 5f);

        // Draw a ray pointing at our target
        Debug.DrawRay(transform.position, box.position - transform.position, Color.red);
        
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

}
