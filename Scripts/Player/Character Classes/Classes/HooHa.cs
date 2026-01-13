using System.Linq;
using UnityEngine;

public class HooHa : CharacterClassManager
{
    // Player
    private GameObject player;
    private Rigidbody rigidBody;

    // Floats
    private float poundVelocity = 60f;

    // Animator
    public Animator animator;

    // Booleans
    private bool db = false;
    private bool requestedPound = false;
    private bool waitBeforePound = false;
    private bool canGroundPound = false;
    private bool canAdd = false;

    // Timers
    private float poundTimer = 0.0f;
    private float maxPoundTimer = 0.5f;

    // Modules
    [Header("Modules")]
    [SerializeField] private InputManager inputManager;
    [SerializeField] private MotorHandler motorHandler;

    private void Start()
    {
        player = gameObject;

        inputManager = GameObject.Find("Client").GetComponent<InputManager>();
        motorHandler = gameObject.GetComponent<MotorHandler>();
        animator = GameObject.Find("HooHa").GetComponent<Animator>();
    }

    void GroundPound()
    {
        if (motorHandler.currentGroundState == MotorHandler.GroundState.FreeFalling)
        {
            if (!requestedPound) return;

            if (!rigidBody)
                rigidBody = player.GetComponent<Rigidbody>();

            if (!waitBeforePound)
            {
                motorHandler.ChangeActionState(MotorHandler.ActionState.Pounded);
                animator.SetBool("onPound", true);

                rigidBody.isKinematic = true;
                waitBeforePound = true;
                poundTimer = 0.0f;
            }

            transform.LookAt(GameObject.Find("Main Camera").transform.position);

            if (!canAdd)
            {
                if (poundTimer < maxPoundTimer)
                {
                    //Debug.Log(poundTimer);
                    poundTimer += 1f * Time.deltaTime;
                }
                else if (poundTimer >= maxPoundTimer)
                {
                    canAdd = true;
                    canGroundPound = true;
                }
            }

            if (canGroundPound)
            {
                if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2))
                {
                    motorHandler.ChangeActionState(MotorHandler.ActionState.None);
                    animator.SetBool("onPound", false);

                    rigidBody.linearVelocity = Vector3.zero;
                    transform.position = hit.point + Vector3.up * 4;

                    canGroundPound = false;
                    canAdd = false;

                    waitBeforePound = false;
                    requestedPound = false;
                }

                Vector3 downVelocity = new Vector3(
                    rigidBody.linearVelocity.x,
                    transform.up.y * -poundVelocity,
                    rigidBody.linearVelocity.z
                    );

                rigidBody.isKinematic = false;
                rigidBody.linearVelocity = downVelocity;
            }
        }
        else
        {
            if (rigidBody.isKinematic)
                rigidBody.isKinematic = false;

            animator.SetBool("onPound", false);

            canGroundPound = false;
            canAdd = false;

            waitBeforePound = false;
            requestedPound = false; 

            return;
        }
    }

    public void CheckForInput()
    {
        if (!db)
        {
            Debug.Log("Current Character Class: HooHa");
            db = true;
        }

        foreach (var action in inputManager.currentActions)
        {
            if (!action.value) continue;

            switch (action.name)
            {
                case "Pound":
                    if (action.value)
                    {
                        requestedPound = action.value;
                    }
                    break;
            }
        }

        if (requestedPound)
        {
            GroundPound();
        }
    }

    public void Update()
    {
        if (!characterNames.Any()) return;

        // If a boolean value is set to true and the name is the same as this class
        bool exists = characterNames.Any(x => x.name == "HooHa");

        // If it doesn't exist, returns
        if (!exists) return;

        CheckForInput();
    }
}
