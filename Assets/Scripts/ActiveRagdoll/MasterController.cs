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

        rigBuilder = this.GetComponent<RigBuilder>();   // we need this for disabling IK
        RigTarget[] rigs = this.GetComponentsInChildren<RigTarget>();
        leftArmTarget = rigs[0];
        rightArmTarget = rigs[1];

        anim = this.GetComponent<Animator>();
        anim.SetInteger("Cond", 2); // set animation to running

        if (IKEnabled) EnableIK();
        else DisableIK();
    }

    private int updateCounter = 0;
    // Unity method for physics update
    void FixedUpdate()
    {
        updateCounter++;
        if (updateCounter % 2 == 0) return;

        closestTarget = targetManager.GetClosestTarget();
        //Debug.DrawRay(this.transform.position, closestTarget.position - this.transform.position);

        //if (slaveController.interpolationStep < 1) return;

        if((this.transform.position - closestTarget.transform.position).magnitude < 4f && !targetAttached)
        {
            if (!IKEnabled) 
                EnableIK();
            leftArmTarget.MoveTowards(closestTarget.transform.position, 0.04f);
            rightArmTarget.MoveTowards(closestTarget.transform.position, 0.04f);
        }
        else
        {

            if (!leftArmTarget.isAtSpawnPosition())
            {
                leftArmTarget.MoveTowardsSpawnPosition(0.04f);
                rightArmTarget.MoveTowardsSpawnPosition(0.04f);
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
