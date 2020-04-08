using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;

public class SlaveController : MonoBehaviour
{

    /// <summary>
    /// Controlls animation following.
    /// </summary>

    private AnimationFollowing animFollow;
    private MasterController masterController;

    private DeadTimer timer;

    public int numberOfCollisions;

    public bool isInGettingUpState;
    private float interpolationStep;

    // PARAMETERS
    private float forceInterpolationInterrval = 4f; // Time it takes for a slave to regain it's full strength (in seconds) after colliding with object

    private float toContactLerp = 15f;  // Determines how fast the character loses strength when in contact
    private float fromContactLerp = 0.1f;   // Determines how fast the character gains strength after freed from contact

    private float contactForce = 0.1f;  // Minimal force strength during collision
    private float contactTorque = 0.1f;    // Minimal torque strength during collision


    private float maxForceCoefficient = 1f;
    private float maxTorqueCoefficient = 1f;


  /*  private float holdingBoxForceCoefficient = 0.15f;
    private float holdingBoxTorqueCoefficient = 0.15f;*/

    // Start is called before the first frame update.
    void Start()
    {
        HumanoidSetUp setUp = this.GetComponentInParent<HumanoidSetUp>();
        animFollow = setUp.GetAnimationFollowing();

        timer = new DeadTimer(this);

        numberOfCollisions = 0;

        isInGettingUpState = false;

        interpolationStep = 0f;

        //ResetForces();
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

    private void EnableAnimFollow(object source, ElapsedEventArgs e)
    {
        slaveController.ResetForces();
        slaveController.EnableAnimFollow();
    }

    // Disables animation following and wakes it up after given time.
    public void Die(float time)
    {
        slaveController.DisableAnimFollow();
        timer.Stop();
        timer.Interval = time * 1000;
        timer.Enabled = true;
        timer.Start();
    }
}
