using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using System;
using Unity.Collections;


public enum CharacterState
{
    IDLE,
    WALKING,
    RUNNING,
    FALLING
}


public class MasterController : MonoBehaviour
{

    /// <summary>
    /// Manages Inverse Kinematics, position and rotation of static animation.
    /// </summary>


    // CHARACTER STATE
    public CharacterState state;


    // PARAMETERS
    [SerializeField]
    private float walkSpeed = 3;
    [SerializeField]
    private float runSpeed = 7;
    [SerializeField]
    private float gravity = 9.81f;
    [SerializeField]
    private float turnSmoothTime = 0.3f;
    [SerializeField]
    private float groundDistance = 0.05f;
    [SerializeField]
    private LayerMask groundMask;


    // USEFUL VARIABLES
    private Camera characterCamera;
    private Transform slaveRoot;
    private Animator anim;
    private bool isGrounded;
    private float currentCharacterAngle;
    private float currentTurnSmoothVelocity;
    private float currentFallVelocity;


    // Start is called before the first frame update
    void Start()
    {
        HumanoidSetUp setUp = this.GetComponentInParent<HumanoidSetUp>();
        characterCamera = setUp.characterCamera;
        slaveRoot = setUp.slaveRoot;
        anim = setUp.anim;
    }

    // Unity method for physics update
    void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(this.transform.position, groundDistance, groundMask);

        state = GetCharacterState();

        if (state != CharacterState.FALLING)
            currentFallVelocity = 0;

        currentCharacterAngle = CalculateCharacterAngle();
        SetCharacterRotation();
        MoveCharacter();

        SetAnimation();
    }


    private CharacterState GetCharacterState()
    {
        bool WSAD = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);
        if (!isGrounded)
        {
            return CharacterState.FALLING;
        }
        else if (WSAD && Input.GetKey(KeyCode.LeftShift))
        {
            return CharacterState.RUNNING;
        }
        else if (WSAD)
        {
            return CharacterState.WALKING;
        }
        else
        {
            return CharacterState.IDLE;
        }
    }

    private float CalculateCharacterAngle()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        Debug.DrawRay(transform.position, direction);

        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + characterCamera.transform.eulerAngles.y;
        float characterAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref currentTurnSmoothVelocity, turnSmoothTime);

        return characterAngle;
    }

    private void MoveCharacter()
    {
        Vector3 moveDirection = Quaternion.Euler(0f, currentCharacterAngle, 0f) * Vector3.forward;
        Debug.DrawRay(transform.position, moveDirection * 5, Color.red);
        switch (state)
        {
            case CharacterState.FALLING:
                currentFallVelocity -= gravity * Time.fixedDeltaTime;
                transform.position += new Vector3(0, currentFallVelocity * Time.fixedDeltaTime, 0);
                break;
            case CharacterState.RUNNING:
                transform.position += moveDirection.normalized * runSpeed * Time.fixedDeltaTime;
                break;
            case CharacterState.WALKING:
                transform.position += moveDirection.normalized * walkSpeed * Time.fixedDeltaTime;
                break;
            case CharacterState.IDLE:
                break;
            default:
                break;
        }
    }

    private void SetCharacterRotation()
    {
        transform.rotation = Quaternion.Euler(0f, currentCharacterAngle, 0f);
    }

    private void SetAnimation()
    {
        switch (state)
        {
            case CharacterState.FALLING:
                break;
            case CharacterState.RUNNING:
                anim.SetInteger("Cond", 2);
                break;
            case CharacterState.WALKING:
                anim.SetInteger("Cond", 1);
                break;
            case CharacterState.IDLE:
                anim.SetInteger("Cond", 0);
                break;
            default:
                break;
        }
    }

}
