using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using System;


public class SlaveController : MonoBehaviour
{

    /// <summary>
    /// Controlls animation following.
    /// </summary>

    private AnimationFollowing animFollow;
    private MasterController masterController;


    [NonSerialized]
    public int numberOfCollisions;
    [NonSerialized]
    public bool isInGettingUpState;
    [NonSerialized]
    public float interpolationStep;
    [NonSerialized]
    private DeadTimer timer;

    
    // PARAMETERS
    private float forceInterpolationInterrval = 4f; // Time it takes for a slave to regain it's full strength (in seconds) after colliding with object

    private float toContactLerp = 15f;  // Determines how fast the character loses strength when in contact
    private float fromContactLerp = 0.1f;   // Determines how fast the character gains strength after freed from contact

    private float contactForce = 0.05f;  // Force strength during collision
    private float contactTorque = 0.05f;    // Torque strength during collision

    private float maxForceCoefficient = 1f;
    private float maxTorqueCoefficient = 1f;


    // Start is called before the first frame update.
    void Start()
    {
        HumanoidSetUp setUp = this.GetComponentInParent<HumanoidSetUp>();
        animFollow = setUp.animFollow;
        masterController = setUp.masterController;

        timer = new DeadTimer(this);
        numberOfCollisions = 0;
        isInGettingUpState = false;
        interpolationStep = 0f;
    }

    // Unity method for physics update.
    void FixedUpdate()
    {

        if (!isInGettingUpState)
        {
            if (numberOfCollisions != 0) LooseStrength();
            else GainStrength();
        }

        animFollow.FollowAnimation();

        if (isInGettingUpState)
        {
            animFollow.SetJointTorque(0, 0);
            IncrementInterpolationStep();
            animFollow.forceCoefficient = InterpolateForceCoefficient(interpolationStep);
            animFollow.torqueCoefficient = InterpolateForceCoefficient(interpolationStep);
        }

    }

    private void LooseStrength()
    {
        animFollow.forceCoefficient = Mathf.Lerp(animFollow.forceCoefficient, contactForce, toContactLerp * Time.fixedDeltaTime);
        animFollow.torqueCoefficient = Mathf.Lerp(animFollow.torqueCoefficient, contactTorque, toContactLerp * Time.fixedDeltaTime);
    }

    private void GainStrength()
    {
        animFollow.forceCoefficient = Mathf.Lerp(animFollow.forceCoefficient, maxForceCoefficient, fromContactLerp * Time.fixedDeltaTime);
        animFollow.torqueCoefficient = Mathf.Lerp(animFollow.torqueCoefficient, maxTorqueCoefficient, fromContactLerp * Time.fixedDeltaTime);
    }

    public float InterpolateForceCoefficient(float x)
    {
        return (float)(0.0001804733 + 0.7707137 * x - 8.36575 * Mathf.Pow(x, 2) + 30.10769 * Mathf.Pow(x, 3) - 42.97538 * Mathf.Pow(x, 4) + 21.46389 * Mathf.Pow(x, 5));
    }

    public void IncrementInterpolationStep()
    {
        if (interpolationStep >= 1f)
            return;

        interpolationStep += Time.fixedDeltaTime * 1f / forceInterpolationInterrval;
    }

    // Sets all forces to zero for time given in seconds.
    public void Die(float time)
    {
        timer.Die(time);
    }

    public void EnableAnimFollow()
    {
        animFollow.isAlive = true;
    }

    public void DisableAnimFollow()
    {
        animFollow.isAlive = false;
    }

    // Sets forces to zero. After calling this function ragdoll will gradually regain strength.
    public void ResetForces()
    {
        animFollow.forceCoefficient = 0f;
        animFollow.torqueCoefficient = 0f;
        interpolationStep = 0f;
        isInGettingUpState = true;
    }
}


public class DeadTimer
{
    /// <summary>
    /// A class for making ragdoll "dead" for specified time.
    /// </summary>

    private SlaveController slaveController;
    private Timer timer;

    public DeadTimer(SlaveController slaveController)
    {
        this.slaveController = slaveController;
        timer = new Timer();
        timer.Elapsed += new ElapsedEventHandler(EnableAnimFollow);
        timer.AutoReset = false;
    }

    // Disables animation following and wakes it up after given time.
    public void Die(float time)
    {
        slaveController.DisableAnimFollow();
        StartTimer(time);
    }

    private void StartTimer(float time)
    {
        timer.Stop();
        timer.Interval = time * 1000;
        timer.Enabled = true;
        timer.Start();
    }

    private void EnableAnimFollow(object source, ElapsedEventArgs e)
    {
        slaveController.ResetForces();
        slaveController.EnableAnimFollow();
    }
}
