using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Input_Manager : MonoBehaviour
{
    public static Input_Manager _INPUT_MANAGER;
    private PlayerInputActions playerInputs;
    private float timeSinceJumpPressed = 0f;
    private Vector2 leftAxisValue = Vector2.zero;


    private void Awake()
    {
        if (_INPUT_MANAGER != null && _INPUT_MANAGER != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            playerInputs = new PlayerInputActions();
            playerInputs.Character.Enable();
            playerInputs.Camera.Enable();

            _INPUT_MANAGER = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    private void Update()
    {
        InputSystem.Update();
        timeSinceJumpPressed += Time.deltaTime;
    }

    public float MouseX()
    {
        return playerInputs.Camera.MouseX.ReadValue<float>();
    }
    public float MouseY()
    {
        return playerInputs.Camera.MouseY.ReadValue<float>();
    }

    public bool GetSouthButtonPressed()
    {
        float oldValue = playerInputs.Character.Jump.ReadValue<float>();
        bool jumpPressed = oldValue != 0f;
        return jumpPressed;
    }


    public bool GetUpPressed()
    {
        Vector2 moveInput = playerInputs.Character.Move.ReadValue<Vector2>();
        return moveInput.y > 0;
    }

    public bool GetDownPressed()
    {
        Vector2 moveInput = playerInputs.Character.Move.ReadValue<Vector2>();
        return moveInput.y < 0;
    }

    public bool GetRightPressed()
    {
        Vector2 moveInput = playerInputs.Character.Move.ReadValue<Vector2>();
        return moveInput.x > 0;
    }

    public bool GetLeftPressed()
    {
        Vector2 moveInput = playerInputs.Character.Move.ReadValue<Vector2>();
        return moveInput.x < 0;
    }

    private void OnDisable()
    {
        playerInputs.Character.Disable();
        playerInputs.Camera.Enable();
    }
}