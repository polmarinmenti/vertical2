using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform; // The player or character's transform
    [SerializeField] private float sensitivity = 100f;
    private float rotationX = 0f;

    private Input_Manager inputManager;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Get the Input_Manager instance
        inputManager = Input_Manager._INPUT_MANAGER;
    }

    void Update()
    {
        // Get mouse movement from the Input_Manager
        float mouseX = inputManager.GetMouseX() * sensitivity * Time.deltaTime;
        float mouseY = inputManager.GetMouseY() * sensitivity * Time.deltaTime;

        // Adjust the vertical rotation of the camera based on mouseY
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        // Apply the vertical rotation to the camera
        transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        // Adjust the horizontal rotation of the player based on mouseX
        playerTransform.Rotate(Vector3.up * mouseX);
    }
}
