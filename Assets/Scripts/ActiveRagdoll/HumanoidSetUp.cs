using UnityEngine;

public class HumanoidSetUp : MonoBehaviour
{

    // THIS NEEDS TO BE SET UP IN INSPECTOR 
    public Transform masterRoot;    // master hips
    public Transform slaveRoot;     // slave hips

    // THIS IS SET UP AUTOMATICALLY
    private MasterController masterController;
    private SlaveController slaveController;
    private AnimationFollowing animFollow;
    private Animator anim;
    private TargetManager targetManager;
    private PlayerController playerController;


    // Awake() is called before all Start() methods
    void Awake()
    {
        if (masterRoot == null) Debug.LogError("masterRoot not assigned.");
        if (slaveRoot == null) Debug.LogError("slaveRoot not assigned.");

        masterController = this.GetComponentInChildren<MasterController>();
        if (masterController == null) Debug.LogError("MasterControler not found.");

        slaveController = this.GetComponentInChildren<SlaveController>();
        if (slaveController == null) Debug.LogError("SlaveController not found.");

        animFollow = this.GetComponentInChildren<AnimationFollowing>();
        if (animFollow == null) Debug.LogError("AnimationFollowing not found.");

        anim = this.GetComponentInChildren<Animator>();
        if (anim == null) Debug.LogError("Animator not found.");

        targetManager = this.GetComponentInParent<TargetManager>();
        if (targetManager == null) Debug.LogError("TargetManager not found.");

        //playerController = this.GetComponent<PlayerController>();
        //if (playerController == null) Debug.LogError("PlayerController not found.");
    }

    public Transform GetMasterRoot()
    {
        return masterRoot;
    }

    public Transform GetSlaveRoot()
    {
        return slaveRoot;
    }

    public MasterController GetMasterController()
    {
        return masterController;
    }

    public SlaveController GetSlaveController()
    {
        return slaveController;
    }

    public AnimationFollowing GetAnimationFollowing()
    {
        return animFollow;
    }

    public Animator GetAnimator()
    {
        return anim;
    }

    public TargetManager GetTargetManager()
    {
        return targetManager;
    }

    public PlayerController GetPlayerController()
    {
        return playerController;
    }

}
