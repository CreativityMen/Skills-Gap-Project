/*
    Title: MotorHandler.cs

    Author: Afonso Marques

    Description: Script used to handle the player's movement.
    Input information is sent from InputManager.cs to this script,
    allowing certain movement to be enabled depending on which boolean
    values have been enabled from the list in InputManager.cs.

    I wrote a bunch of comments to remind future self how I did certain
    things in case I need it when working on other projects, and to also
    show others how I did them in case they want to learn.

 */

using NUnit.Framework;
using System.Runtime.CompilerServices;
using Unity.Content;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR.Haptics;

public class MotorHandler : MonoBehaviour
{
    // Rigidbody
    [Header("Current Rigidbody")]
    [SerializeField] private Rigidbody rigidBody;

    // States
    [Header("Action State")]
    [SerializeField] public ActionState currentActionState;
    [SerializeField] public ActionState lastActionState;

    [Header("Ground State")]
    [SerializeField] public GroundState currentGroundState;
    [SerializeField] public GroundState lastGroundState;

    public enum GroundState
    {
        Grounded,
        FreeFalling
    }

    public enum ActionState
    {
        None,
        Pounded,
    }

    // Components
    [Header("Components")]
    private CapsuleCollider capsuleCollider;

    // Camera
    [Header("Current Camera")]
    [SerializeField ]private Camera currentCamera;

    // Modules
    [Header("Modules")]
    [SerializeField] private InputManager inputManager;
    [SerializeField] private GetCharacterClass getCharacterClass;

    // Character Class
    [SerializeField]
    [Header("Character Class")]
    public CharacterClassManager currentCharacterClass;

    // Booleans
    [Header("Action Booleans")]
    [SerializeField] private bool jumpRequested = false;

    // Properties
    [Header("Properties")]
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float jumpPower = 14f;
    [SerializeField] private float smoothFactor = 8f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        inputManager = GameObject.Find("Client").GetComponent<InputManager>();

        // Finds GetCharacterClass component from the Client GameObject
        getCharacterClass = GameObject.Find("Client").GetComponent<GetCharacterClass>();

        // Initialises it with the name of the character we want to play as
        getCharacterClass.Init("HooHa");

        // After it finishes finding the class of the moveset from the character we want to play as,
        // it makes that our current character class
        currentCharacterClass = GetComponent<CharacterClassManager>();

        capsuleCollider = GetComponent<CapsuleCollider>();
        rigidBody = GetComponent<Rigidbody>();
    }

    // Function responsible for changing states
    private void ChangeGroundState(GroundState newState)
    {
        lastGroundState = currentGroundState;

        if (currentGroundState == newState) return;
        currentGroundState = newState;

        Debug.Log("Current State: " +  currentGroundState);
    }

    public void ChangeActionState(ActionState newState)
    {
        lastActionState = currentActionState;

        if (currentActionState == newState) return;
        currentActionState = newState;
    }

    public void CheckForGround()
    {
        // Raycast Info
        Ray ray;
        ray = new Ray(transform.position, -transform.up);

        float distance = 1.4f;

        if (Physics.Raycast(ray, out RaycastHit hit, distance))
        {
            if (currentGroundState == GroundState.Grounded) return;

            capsuleCollider.material.staticFriction = 1f;
            ChangeGroundState(GroundState.Grounded);
        }
        else
        {
            if (currentGroundState == GroundState.FreeFalling) return;

            capsuleCollider.material.staticFriction = 0f;
            ChangeGroundState(GroundState.FreeFalling);
        }

        Debug.DrawRay(transform.position, -transform.up, Color.yellow);
    }

    private void Movement()
    {
        // Actions
        var actions = inputManager.currentActions;

        // Camera Vector3's
        Vector3 cameraForward = new Vector3(currentCamera.transform.forward.x, 0, currentCamera.transform.forward.z);
        Vector3 cameraRight = new Vector3(currentCamera.transform.right.x, 0, currentCamera.transform.right.z);

        // Movement Vector3
        Vector2 input = inputManager.moveVectorInput;

        // Multiplies the camera forward look vector along with forward input, same with camera right vector.
        // Allows full 360 analog movement.
        Vector3 moveDir =
            cameraForward * input.y +
            cameraRight * input.x;

        // Directions relative to the player's camera
        (Vector3 forward, Vector3 backward) = (cameraForward, -cameraForward);
        (Vector3 left, Vector3 right) = (cameraRight, -cameraRight);

        forward.Normalize();
        backward.Normalize();

        foreach (var action in actions)
        {
            // We use continue here instead of return because we don't want to exit the entire function.
            // If it is false, then we want to continue running the rest of the code.
            if (!action.value) continue;

            switch (action.name)
            {
                case "Forward":
                    moveDir += forward;
                    break;

                case "Backward":
                    moveDir -= forward;
                    break;

                case "Left":
                    moveDir -= right;
                    break;

                case "Right":
                    moveDir += right;
                    break;

                case "Jump":
                    jumpRequested = action.value;
                    break;
            }
        }

        // We only call action functions outside of the loop to keep it clean
        // and prevent it from firing the functions multiple times in one frame
        if (jumpRequested)
        {
            jumpRequested = false;
            Jump();
        }

        // Normalizing the move direction Vector3 prevents faster diagonal movement
        moveDir = moveDir.normalized;

        // Vector3s
        Vector3 movementVector = new Vector3(
            moveDir.x * walkSpeed,
            rigidBody.linearVelocity.y,
            moveDir.z * walkSpeed);

        rigidBody.linearVelocity = movementVector;

        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = targetRotation;
        }
    }

    private void Jump()
    {
        if (currentGroundState == GroundState.FreeFalling) return;

        // Vector3s
        Vector3 jumpVector = new Vector3(
            rigidBody.linearVelocity.x,
            jumpPower,
            rigidBody.linearVelocity.z);

        rigidBody.linearVelocity = jumpVector;
    }

    // Using FixedUpdate as it's heavily movement based with rigidbody
    private void FixedUpdate()
    {
        if (!inputManager) return;

        Movement();
        CheckForGround();
    }
}
