using UnityEngine;
using System;


public enum RagdollState
{
    FOLLOWING_ANIMATION,
    LOOSING_STRENGTH,
    GAINING_STRENGTH,
    DEAD
}


public class SlaveController : MonoBehaviour
{
    /// <summary>
    /// Controlls Ragdoll.
    /// </summary>


    // RAGDOLL STATE
    public RagdollState state;


    // PARAMETERS
    [SerializeField]
    [Tooltip("Determines how fast ragdoll loses strength when in contact.")]
    private float looseStrengthLerp = 1.0f;
    [SerializeField]
    [Tooltip("Determines how fast ragdoll gains strength after being freed from contact.")]
    private float gainStrengthLerp = 0.05f;
    [SerializeField]
    [Tooltip("Minimum force strength during collision.")]
    private float minContactForce = 0.1f;
    [SerializeField]
    [Tooltip("Minimum torque strength during collision.")]
    private float minContactTorque = 0.1f;
    [SerializeField]
    [Tooltip("Time of being dead expressed in seconds passed in sumulation.")]
    private float deadTime = 4.0f;


    // USEFUL VARIABLES
    private AnimationFollowing animFollow;
    private float maxTorqueCoefficient;
    private float maxForceCoefficient;
    private float currentDeadStep;
    private float currentStrength;
    [NonSerialized] public int currentNumberOfCollisions;


    // Start is called before the first frame update.
    void Start()
    {
        HumanoidSetUp setUp = this.GetComponentInParent<HumanoidSetUp>();
        animFollow = setUp.animFollow;

        maxForceCoefficient = animFollow.forceCoefficient;
        maxTorqueCoefficient = animFollow.torqueCoefficient;
        currentNumberOfCollisions = 0;
        currentDeadStep = deadTime;
        currentStrength = 1.0f;
    }

    // Unity method for physics update.
    void FixedUpdate()
    {
        // Apply animation following
        animFollow.FollowAnimation();

        // print(currentNumberOfCollisions);
        // print(animFollow.forceCoefficient);

        state = GetRagdollState();
        switch (state)
        {
            case RagdollState.DEAD:
                currentDeadStep += Time.fixedDeltaTime;
                if (currentDeadStep >= deadTime)
                    ComeAlive();
                break;

            case RagdollState.LOOSING_STRENGTH:
                LooseStrength();
                break;

            case RagdollState.GAINING_STRENGTH:
                GainStrength();
                break;
            
            case RagdollState.FOLLOWING_ANIMATION:
                break;

            default:
                break;
        }
    }
     

    private RagdollState GetRagdollState()
    {
        if (!animFollow.isAlive)
        {
            return RagdollState.DEAD;
        }
        else if (currentNumberOfCollisions != 0)
        {
            return RagdollState.LOOSING_STRENGTH;
        }
        else if (currentStrength < 1)
        {
            return RagdollState.GAINING_STRENGTH;
        }
        else
        {
            return RagdollState.FOLLOWING_ANIMATION;
        }
    }

    private void LooseStrength()
    {
        currentStrength -= looseStrengthLerp * Time.fixedDeltaTime;
        currentStrength = Mathf.Clamp(currentStrength, 0, 1);
        InterpolateStrength(currentStrength);
    }

    private void GainStrength()
    {
        currentStrength += gainStrengthLerp * Time.fixedDeltaTime;
        currentStrength = Mathf.Clamp(currentStrength, 0, 1);
        InterpolateStrength(currentStrength);
    }

    private void InterpolateStrength(float ratio)
    {
        animFollow.forceCoefficient = Mathf.Lerp(minContactForce, maxForceCoefficient, ratio);
        animFollow.torqueCoefficient = Mathf.Lerp(minContactTorque, maxTorqueCoefficient, ratio);
    }

    [ContextMenu("Die")]
    private void Die()
    {
        animFollow.isAlive = false;
        currentDeadStep = 0;
        ResetForces();
    }

    [ContextMenu("Come alive")]
    private void ComeAlive()
    {
        animFollow.isAlive = true;
    }

    // Sets animation following forces to zero. After calling this method, ragdoll will gradually regain strength.
    [ContextMenu("Reset forces")]
    private void ResetForces()
    {
        animFollow.forceCoefficient = 0f;
        animFollow.torqueCoefficient = 0f;
        currentStrength = 0;
    }

}
