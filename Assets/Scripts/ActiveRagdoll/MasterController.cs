using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class MasterController : MonoBehaviour   // Master = Static Animation
{
    /// <summary>
    /// Manages Inverse Kinematics of static animation.
    /// </summary>

    private Animator anim;
    private Transform masterRoot;
    private AnimationFollowing animFollow;
    private SlaveController slaveController;
    private PlayerController playerController;
    private RigBuilder rigBuilder;
    private RigTarget leftArmTarget;
    private RigTarget rightArmTarget;

    private TargetManager targetManager;
    public Target closestTarget;

    public Transform centralPoint;

    private bool IKEnabled = false;
    public bool targetAttached = false;

    public int numberOfAttachments;

    private float rotationSpeed = 200f;
    private float runSpeed = 3f;


    // Start is called before the first frame update
    void Start()
    {
        HumanoidSetUp setUp = this.GetComponentInParent<HumanoidSetUp>();
        masterRoot = setUp.GetMasterRoot();
        animFollow = setUp.GetAnimationFollowing();
        slaveController = setUp.GetSlaveController();
        targetManager = setUp.GetTargetManager();
        playerController = setUp.GetPlayerController();

        rigBuilder = this.GetComponent<RigBuilder>();   // we need this for disabling IK
        RigTarget[] rigs = this.GetComponentsInChildren<RigTarget>();
        leftArmTarget = rigs[0];
        rightArmTarget = rigs[1];

        anim = this.GetComponent<Animator>();
        anim.SetInteger("Cond", 2); // set animation to running

        if (IKEnabled) EnableIK();
        else DisableIK();

        numberOfAttachments = 0;
    }

    // Unity method for physics update
    void FixedUpdate()
    {
        if (closestTarget == null)
            closestTarget = targetManager.GetClosestTarget(this.transform.position);

        if (closestTarget == null) return;


        Debug.DrawRay(this.transform.position, closestTarget.transform.position - this.transform.position);
        /*        playerController.RotateTowards(closestTarget.transform);
                playerController.MoveForward();*/

        RotateTowards();
        MoveForward();


        // HERE COMES THE SPAGHETTTIII I HAVE NO IDEA WHAT IM DOING
        /*
                if (numberOfAttachments == 1)
                {
                    if (!IKEnabled)
                        EnableIK();
                    leftArmTarget.MoveTowards(closestTarget.transform.position, 0.03f);
                    rightArmTarget.MoveTowards(closestTarget.transform.position, 0.03f);
                    return;
                }


                // move left hand
                if (numberOfAttachments < 2 && (leftArmTarget.transform.position - centralPoint.position).magnitude < 1.1f)
                {
                    if ((leftArmTarget.transform.position - closestTarget.transform.position).magnitude > 0.2f && (closestTarget.transform.position - leftArmTarget.transform.position).magnitude < 4f)
                    {
                        if (!IKEnabled)
                            EnableIK();
                        leftArmTarget.MoveTowards(closestTarget.transform.position, 0.03f);
                    }
                }

                // move right hand
                if (numberOfAttachments < 2 && (rightArmTarget.transform.position - centralPoint.position).magnitude < 1.1f)
                {
                    if ((rightArmTarget.transform.position - closestTarget.transform.position).magnitude > 0.2f && (closestTarget.transform.position - rightArmTarget.transform.position).magnitude < 4f)
                    {
                        if (!IKEnabled)
                            EnableIK();
                        rightArmTarget.MoveTowards(closestTarget.transform.position, 0.03f);
                    }
                }

                if (numberOfAttachments == 2)
                {
                    leftArmTarget.transform.position = leftArmTarget.spawnPosition + new Vector3(0, 0.7f, 0);
                    if (!IKEnabled)
                        EnableIK();
                }*/

        if (numberOfAttachments == 1)
        {
            EnableIK();
            MoveLeftHandTowardsBox();
            MoveRightHandTowardsBox();
            return;
        }



        if (numberOfAttachments < 2 && (leftArmTarget.transform.position - centralPoint.position).magnitude < 1.1f)
        {
            if ((leftArmTarget.transform.position - closestTarget.transform.position).magnitude > 0.2f && (closestTarget.transform.position - leftArmTarget.transform.position).magnitude < 4f)
            {

                EnableIK();
                MoveLeftHandTowardsBox();
            }
        }

        if (numberOfAttachments < 2 && (rightArmTarget.transform.position - centralPoint.position).magnitude < 1.1f)
        {
            if ((rightArmTarget.transform.position - closestTarget.transform.position).magnitude > 2f && (closestTarget.transform.position - rightArmTarget.transform.position).magnitude < 4f)
            {
                EnableIK();
                MoveRightHandTowardsBox();
            }
        }

        if (numberOfAttachments == 2)
        {
            leftArmTarget.transform.position = centralPoint.position + new Vector3(0, 0.7f, 0);
            EnableIK();
        }


    }

    public void DisableIK()
    {
        IKEnabled = false;
        foreach (var l in rigBuilder.layers)
        {
            l.active = false;
        }
    }

    public void EnableIK()
    {
        IKEnabled = true;
        foreach (var l in rigBuilder.layers)
        {
            l.active = true;
        }
    }

    public void TargetAttached(bool value)
    {
        targetAttached = value;
    }

    private void MoveForward()
    {
        transform.position += transform.forward * Time.fixedDeltaTime * runSpeed;
    }

    private void RotateTowards()
    {
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, closestTarget.transform.position - transform.position, 0.04f, 5f);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        Quaternion rot = Quaternion.LookRotation(newDirection);
        Vector3 tmp = rot.eulerAngles;
        tmp.x = transform.rotation.eulerAngles.x;
        tmp.z = transform.rotation.eulerAngles.z;
        transform.rotation = Quaternion.Euler(tmp);
    }


    private void MoveLeftHandTowardsBox()
    {
        Vector3 direction = (closestTarget.transform.position - leftArmTarget.transform.position).normalized;
        leftArmTarget.transform.position += direction / 80;
    }

    private void MoveRightHandTowardsBox()
    {
        Vector3 direction = (closestTarget.transform.position - rightArmTarget.transform.position).normalized;
        rightArmTarget.transform.position += direction / 80;
    }

}
