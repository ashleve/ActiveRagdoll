using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;

public class SlaveController : MonoBehaviour
{

    public AnimationFollowing animFollow;
    private Timer aTimer;

    // USEFUL VARIABLES
    [Range(0f, 1f)] public float interpolationStep = 1f;
    private float forceInterpolationInterrval = 4f; // Time it takes for a slave to regain it's full strength (in seconds)
    public int numberOfCollisions;

    float toContactLerp = 15f;              // Determines how fast the character loses strength when in contact
    float fromContactLerp = 0.1f;             // Determines how fast the character gains strength after freed from contact

    float contactForce = 0.1f;
    float contactJointTorque = 0.1f;

    public bool gettingUp;

    public



    // Start is called before the first frame update
    void Start()
    {
        animFollow = transform.root.GetComponentInChildren<AnimationFollowing>();
        interpolationStep = 0f;

        numberOfCollisions = 0;

        gettingUp = true;

        aTimer = new Timer();
        aTimer.Elapsed += new ElapsedEventHandler(EnableAnimFollowWithTimer);
        aTimer.AutoReset = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!gettingUp)
        {
            if (numberOfCollisions != 0) LooseStrength();
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

    public void IncrementInterpolationStep()
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
        animFollow.torqueCoefficient = Mathf.Lerp(animFollow.torqueCoefficient, contactJointTorque, toContactLerp * Time.fixedDeltaTime);
    }

    private void GainStrength()
    {
        animFollow.forceCoefficient = Mathf.Lerp(animFollow.forceCoefficient, 1f, fromContactLerp * Time.fixedDeltaTime);
        animFollow.torqueCoefficient = Mathf.Lerp(animFollow.torqueCoefficient, 1f, fromContactLerp * Time.fixedDeltaTime);
    }

    private void ComeAlive()
    {

    }

    public float InterpolateForceCoefficient(float x)
    {
        //return (float)(0.001767887 - 1.214784 * x + 44.06199 * Mathf.Pow(x, 2) - 139.4125 * Mathf.Pow(x, 3) + 153.2927 * Mathf.Pow(x, 4) - 55.71611 * Mathf.Pow(x, 5));
        //return (float)(0.0008196062 + 6.827898 * x - 49.84974 * Mathf.Pow(x, 2) + 125.6229 * Mathf.Pow(x, 3) - 129.4296 * Mathf.Pow(x, 4) + 47.83376 * Mathf.Pow(x, 5));
        //return (float)(-0.0007976415 + 3.264004 * x - 38.32158 * Mathf.Pow(x, 2) + 124.011 * Mathf.Pow(x, 3) - 153.7231 * Mathf.Pow(x, 4) + 65.76459 * Mathf.Pow(x, 5));
        return (float)(0.0001804733 + 0.7707137 * x - 8.36575 * Mathf.Pow(x, 2) + 30.10769 * Mathf.Pow(x, 3) - 42.97538 * Mathf.Pow(x, 4) + 21.46389 * Mathf.Pow(x, 5));
        //return (float)(8.5293 * Mathf.Pow(x, 8) + -3.1478 * Mathf.Pow(x, 7) + 4.7496 * Mathf.Pow(x, 6) + -3.7719 * Mathf.Pow(x, 5) + 1.6897 * Mathf.Pow(x, 4) + -4.2294 * Mathf.Pow(x, 3) + 5.3835 * Mathf.Pow(x, 2) + -2.4110 * Mathf.Pow(x, 1));
    }


}
