using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;

public class SlaveController : MonoBehaviour    // Slave = Ragdoll
{
    /// <summary>
    /// Controls parameters of animation following;
    /// </summary>

    private Transform slaveRoot;
    private AnimationFollowing animFollow;
    private MasterController masterController;

    private SpawnInfo slaveSpawnInfo;

    private GoingRagdollTimer timer;

    public int numberOfCurrentCollisions;

    public bool gettingUp;
    private float interpolationStep;


    // PARAMETERS
    private float forceInterpolationInterrval = 4f; // Time it takes for a slave to regain it's full strength (in seconds)

    private float toContactLerp = 15f;  // Determines how fast the character loses strength when in contact
    private float fromContactLerp = 0.1f;   // Determines how fast the character gains strength after freed from contact

    private float contactForce = 0.1f;  // Minimal force strength during collision
    private float contactTorque = 0.1f;    // Minimal torque strength during collision


    // Start is called before the first frame update
    void Start()
    {
        HumanoidSetUp setUp = this.GetComponentInParent<HumanoidSetUp>();
        slaveRoot = setUp.GetSlaveRoot();
        animFollow = setUp.GetAnimationFollowing();
        masterController = setUp.GetMasterController();

        slaveSpawnInfo = new SpawnInfo(this.transform);

        timer = new GoingRagdollTimer(this);

        numberOfCurrentCollisions = 0;

        gettingUp = false;

        interpolationStep = 0f;
    }

    // Unity method for physics update
    void FixedUpdate()
    {

        if (!gettingUp)
        {
            if (numberOfCurrentCollisions != 0) LooseStrength();
            else GainStrength();
        }

        animFollow.FollowAnimation();

        if (gettingUp)
        {
            animFollow.SetJointTorque(0, 0);
            IncrementInterpolationStep();
            animFollow.forceCoefficient = InterpolateForceCoefficient(interpolationStep);
            animFollow.torqueCoefficient = InterpolateForceCoefficient(interpolationStep);
        }

    }

    // Sets all forces to zero for time given in seconds
    public void GoRagdoll(int time)
    {
        timer.GoRagdoll(time);
    }

    public void EnableAnimFollow()
    {
        animFollow.isAlive = true;
    }

    public void DisableAnimFollow()
    {
        animFollow.isAlive = false;
    }

    // Sets forces to zero. After calling this function ragdoll will gradually regain strength
    public void ResetForces()
    {
        animFollow.forceCoefficient = 0f;
        animFollow.torqueCoefficient = 0f;
        interpolationStep = 0f;
        gettingUp = true;
    }

    private void IncrementInterpolationStep()
    {
        if (interpolationStep >= 1f)
        {
            gettingUp = false;
            return;
        }

        interpolationStep += Time.fixedDeltaTime * 1f / forceInterpolationInterrval;
    }

    private void LooseStrength()
    {
        animFollow.forceCoefficient = Mathf.Lerp(animFollow.forceCoefficient, contactForce, toContactLerp * Time.fixedDeltaTime);
        animFollow.torqueCoefficient = Mathf.Lerp(animFollow.torqueCoefficient, contactTorque, toContactLerp * Time.fixedDeltaTime);
    }

    private void GainStrength()
    {
        animFollow.forceCoefficient = Mathf.Lerp(animFollow.forceCoefficient, 1f, fromContactLerp * Time.fixedDeltaTime);
        animFollow.torqueCoefficient = Mathf.Lerp(animFollow.torqueCoefficient, 1f, fromContactLerp * Time.fixedDeltaTime);
    }

    private float InterpolateForceCoefficient(float x)
    {
        return (float)(0.0001804733 + 0.7707137 * x - 8.36575 * Mathf.Pow(x, 2) + 30.10769 * Mathf.Pow(x, 3) - 42.97538 * Mathf.Pow(x, 4) + 21.46389 * Mathf.Pow(x, 5));
    }

    // Drop all cubes that are attached to ragdoll
    public void DropTargets()
    {
        CollisionDetector[] cdArr = this.GetComponentsInChildren<CollisionDetector>();
        foreach (var cd in cdArr)
            cd.dropTarget();
    }

    public void Respawn()
    {
        this.transform.localPosition = slaveSpawnInfo.localPosition;

        Transform[] transforms = this.GetComponentsInChildren<Transform>();
        for (int i = 0; i < slaveSpawnInfo.childrenLocalPositions.Length; i++)
        {
            Transform child = transforms[i];
            child.localPosition = slaveSpawnInfo.childrenLocalPositions[i];
            Rigidbody rb = child.GetComponent<Rigidbody>();
            if (rb != null)
            {
                child.GetComponent<Rigidbody>().velocity = Vector3.zero;
                child.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
        }
    }
}


[System.Serializable]
public class SpawnInfo
{
    public Vector3 localPosition;
    public Vector3[] childrenLocalPositions;

    public SpawnInfo(Transform t)
    {
        localPosition = t.localPosition;
        Transform[] transforms = t.GetComponentsInChildren<Transform>();
        childrenLocalPositions = new Vector3[transforms.Length];
        for (int i = 0; i < transforms.Length; i++)
            childrenLocalPositions[i] = transforms[i].localPosition;
    }
}


public class GoingRagdollTimer
{
    private SlaveController slaveController;
    private Timer timer;

    public GoingRagdollTimer(SlaveController slaveController)
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

    public void GoRagdoll(int time)
    {
        slaveController.DisableAnimFollow();
        timer.Stop();
        timer.Interval = time * 1000;
        timer.Enabled = true;
        timer.Start();
    }
}