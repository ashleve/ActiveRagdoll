using UnityEngine;
using System;


public class HumanoidSetUp : MonoBehaviour
{

    // THIS NEEDS TO BE SET UP IN INSPECTOR 
    public Transform masterRoot;    // master hips
    public Transform slaveRoot;     // slave hips
    public Camera characterCamera;


    // THIS IS SET UP AUTOMATICALLY
    [NonSerialized]
    public MasterController masterController;
    [NonSerialized]
    public SlaveController slaveController;
    [NonSerialized]
    public AnimationFollowing animFollow;
    [NonSerialized]
    public Animator anim;
    [NonSerialized]
    public CharacterController characterController;


    // Awake() is called before all Start() methods
    void Awake()
    {
        if (masterRoot == null) Debug.LogError("masterRoot not assigned.");
        if (slaveRoot == null) Debug.LogError("slaveRoot not assigned.");
        if (characterCamera == null) Debug.LogError("characterCamera not assigned.");

        masterController = this.GetComponentInChildren<MasterController>();
        if (masterController == null) Debug.LogError("MasterControler not found.");

        slaveController = this.GetComponentInChildren<SlaveController>();
        if (slaveController == null) Debug.LogError("SlaveController not found.");

        animFollow = this.GetComponentInChildren<AnimationFollowing>();
        if (animFollow == null) Debug.LogError("AnimationFollowing not found.");

        anim = this.GetComponentInChildren<Animator>();
        if (anim == null) Debug.LogError("Animator not found.");

        characterController = this.GetComponentInChildren<CharacterController>();
        if (characterController == null) Debug.LogError("CharacterController not found.");
    }

}