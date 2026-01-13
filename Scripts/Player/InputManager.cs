/*
    Title: InputManager.cs

    Author: Afonso Marques

    Description: Script used to manage detected input from the player
    which the motor handler for the character movement can get ahold of
    in order to make the player do whatever it needs to do.

    I wrote a bunch of comments to remind future self how I did certain
    things in case I need it when working on other projects, and to also
    show others how I did them in case they want to learn.

 */

using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;

// "System.Serializable" makes sure that the class can be seen in the hierarchy
// Made a class for a custom boolean value that can be given a name which I later
// end up using for a list
 
[System.Serializable]
public class ActionBool
{
    // This is the same as creating a class in Roblox Luau when using "self". We're creating our own
    // variables for this class which can later then be used for specific instances, (in this case, several
    // different boolean values with their own information)
    public string name;
    public bool value;

    /* ^^^ ^^^ ^^^
     * ROBLOX LUAU:
         * self.name = ""
         * self.value = false
         */

    // A public function which other classes can access
    public ActionBool(string name, bool value)
    {
        // Instead of just using the "name" string variable (which we've created in the beginning of this class),
        // we instead need to use "this" since are referencing this class. (e.g same example with "self" in Roblox Luau)

        this.name = name;
        this.value = value;
    }
}

public class InputManager : MonoBehaviour
{
    // Action Asset
    private InputActionAsset inputActions;

    // Placeholder Actions
    [Header("Input Actions")]
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction shiftAction;

    // Other Booleans
    private bool shiftLatched = false;

    // Placeholder Booleans
    public Vector2 moveVectorInput;

    // Booleans
    // This will create a list with a class I've created above that allows you to give a name
    // to your boolean value.
    
    // Inside of this list, it will create an instance for each boolean value with their own information.
    [Header("Current Actions")]
    public List<ActionBool> currentActions = new List<ActionBool>()
    {
        // Movement
        new ActionBool("Forward", false),
        new ActionBool("Left", false),
        new ActionBool("Backward", false),
        new ActionBool("Right", false),

        // Actions
        new ActionBool("Jump", false),
        new ActionBool("Pound", false),
    };
    
    // Function that enables boolean values corresponding to the detected input.
    private void EnableBooleanActions(Vector2 move)
    {
        if (move == null) return;

        // Move Maximum Variables
        float moveMaxVal = 0.5f;

        // Boolean variables that will only be true if they're either greater or less than their value
        // Wrote it this way to prevent code clutter, and to also keep it nice and tidy
        (bool forward, bool backward) = (move.y > moveMaxVal, move.y < -moveMaxVal);
        (bool left, bool right) = (move.x > moveMaxVal, move.x < -moveMaxVal);

        moveVectorInput = move;

        // For loop that will go through each boolean value inside the list
        foreach (var action in currentActions)
        {
            // Using switch to avoid code clutter and for overall effiency.
            // This checks every boolean's name, and activates them on the booleans created earlier
            // above that rely on the Vector2 move input variable.
            switch (action.name)
            {
                case "Forward":
                    action.value = forward;
                    break;
                case "Backward":
                    action.value = backward;
                    break;
                case "Left":
                    action.value = left;
                    break;
                case "Right":
                    action.value = right;
                    break;

                case "Jump":
                    action.value = jumpAction.IsPressed();
                    break;
                case "Pound":
                    action.value = shiftAction.WasPressedThisFrame();
                    break;
            }
        }
    }

    private void DetectInput()
    {
        if (moveAction == null) return;

        Vector2 move = moveAction.ReadValue<Vector2>();

        EnableBooleanActions(move);
    }

    private void Start()
    {
        if (inputActions != null) return;

        inputActions = Resources.Load<InputActionAsset>("InputSystem_Actions");

        var playerMap = inputActions.FindActionMap("Player");

        moveAction = playerMap.FindAction("Move");
        jumpAction = playerMap.FindAction("Jump");
        shiftAction = playerMap.FindAction("Pound");

        playerMap.Enable();
    }

    void Update() { DetectInput(); }
}
