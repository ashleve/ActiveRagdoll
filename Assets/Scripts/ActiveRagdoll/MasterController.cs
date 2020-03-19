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

    private bool IKEnabled = false;
    public bool targetAttached = false;


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
    }

    // Unity method for physics update
    void FixedUpdate()
    {
        closestTarget = targetManager.GetClosestTarget(this.transform.position);

        if (closestTarget == null) return;


        Debug.DrawRay(this.transform.position, closestTarget.transform.position - this.transform.position);
        playerController.RotateTowards(closestTarget.transform);
        playerController.MoveForward();

        //if (slaveController.interpolationStep < 1) return;

        if((this.transform.position - closestTarget.transform.position).magnitude < 4f && !targetAttached)
        {
            if (!IKEnabled) 
                EnableIK();
            leftArmTarget.MoveTowards(closestTarget.transform.position, 0.02f);
            rightArmTarget.MoveTowards(closestTarget.transform.position, 0.02f);
        }
        else
        {
            if (!leftArmTarget.isAtSpawnPosition())
            {
                leftArmTarget.MoveTowardsSpawnPosition(0.02f);
                rightArmTarget.MoveTowardsSpawnPosition(0.02f);
            }
            else if (IKEnabled)
                DisableIK();
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

}
