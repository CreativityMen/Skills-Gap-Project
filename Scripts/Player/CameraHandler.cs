/*
    Title: CameraHandler.cs

    Author: Afonso Marques

    Description: Script used to handle the camera movement detected
    from the player's mouse input.

    I wrote a bunch of comments to remind future self how I did certain
    things in case I need it when working on other projects, and to also
    show others how I did them in case they want to learn.

 */

using UnityEngine;
using UnityEngine.InputSystem;

public class CameraHandler : MonoBehaviour
{
    // Objects
    private Transform player;

    // Properties
    private const float YMin = -50.0f;
    private const float YMax = 50.0f;

    private float currentX = 0.0f;
    private float currentY = 0.0f;

    public float distance = 5.0f;
    public float sensitivity = 30.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Player variable is assigned
        player = GameObject.FindWithTag("Player").transform;

        // Hides the cursor and locks it in the center of the screen
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // LateUpdate is called once per frame, after all "Update()" calls have finished
    void LateUpdate()
    {
        // Create a Vector2 variable to get X and Y movement from the mouse
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // Adds both currentX and Y with mouse input detected (the variable we just created above)
        // from the player to their own respective axis along with the sensitivity multiplied
        // with deltaTime for consistent results.

        currentX += mouseDelta.x * sensitivity * Time.deltaTime;
        currentY -= mouseDelta.y * sensitivity * Time.deltaTime;

        // Clamps the currentY variable with the minimum and maximum variable preventing the camera
        // from going upside down.
        currentY = Mathf.Clamp(currentY, YMin, YMax);

        // Creates a new Vector3 variable and adds distance between the camera and the player
        Vector3 Direction = new Vector3(0, 0, -distance);

        // Rotational camera movement using Quaternion with currentX and currentY based on our mouse input
        // on both axis
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

        // Sets the camera's position relative to the player's position, along with information from
        // the rotation and direction variables we've created.
        transform.position = player.position + rotation * Direction;
        transform.LookAt(player.position);
    }
}
