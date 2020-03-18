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

    public int numberOfCurrentCollisions;

    public bool gettingUp;

    private Timer aTimer;
    private float interpolationStep;

    // PARAMETERS
    private float forceInterpolationInterrval = 4f; // Time it takes for a slave to regain it's full strength (in seconds)

    private float toContactLerp = 15f;  // Determines how fast the character loses strength when in contact
    private float fromContactLerp = 0.1f;   // Determines how fast the character gains strength after freed from contact

    private float contactForce = 0.1f;  // Minimal strength during collision
    private float contactTorque = 0.1f;    // Minimal torque strength during collision


    // Start is called before the first frame update
    void Start()
    {
        HumanoidSetUp setUp = this.GetComponentInParent<HumanoidSetUp>();
        slaveRoot = setUp.GetSlaveRoot();
        animFollow = setUp.GetAnimationFollowing();
        masterController = setUp.GetMasterController();

        slaveSpawnInfo = new SpawnInfo(this.transform);

        numberOfCurrentCollisions = 0;

        gettingUp = false;

        interpolationStep = 0f;

        aTimer = new Timer();
        aTimer.Elapsed += new ElapsedEventHandler(EnableAnimFollowWithTimer);
        aTimer.AutoReset = false;
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

    public void Respawn()
    {
        this.transform.localPosition = slaveSpawnInfo.localPosition;

        Transform[] transforms = this.GetComponentsInChildren<Transform>();
        for (int i = 0; i < slaveSpawnInfo.childrenLocalPositions.Length; i++)
        {
            Transform child = transforms[i];
            child.localPosition = slaveSpawnInfo.childrenLocalPositions[i];
            Rigidbody rb = child.GetComponent<Rigidbody>();
            if(rb != null)
            {
                child.GetComponent<Rigidbody>().velocity = Vector3.zero;
                child.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
        }


    }

    public void GoRagdoll(int time)
    {
        DisableAnimFollow();
        aTimer.Stop();
        aTimer.Interval = time * 1000;
        aTimer.Enabled = true;
        aTimer.Start();
    }

    private void EnableAnimFollowWithTimer(object source, ElapsedEventArgs e)
    {
        EnableAnimFollow();
        interpolationStep = 0f;
        gettingUp = true;
        animFollow.forceCoefficient = 0f;
        animFollow.torqueCoefficient = 0f;
    }

    private void EnableAnimFollow()
    {
        animFollow.isAlive = true;
    }

    private void DisableAnimFollow()
    {
        animFollow.isAlive = false;
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

    public void DropTargets()
    {
        CollisionDetector[] cdArr = this.GetComponentsInChildren<CollisionDetector>();
        foreach (var cd in cdArr)
            cd.dropTarget();
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