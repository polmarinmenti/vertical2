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
            playerInputs.Gun.Enable();

            _INPUT_MANAGER = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    private void Update()
    {
        InputSystem.Update();
        timeSinceJumpPressed += Time.deltaTime;
    }

    //Mouse

    public float GetMouseX()
    {
        return playerInputs.Camera.MouseX.ReadValue<float>();
    }

    public float GetMouseY()
    {
        return playerInputs.Camera.MouseY.ReadValue<float>();
    }


    //Character

    public bool GetJump()
    {
        return playerInputs.Character.Jump.ReadValue<float>() != 0f;
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


    //Gun

    public bool GetShoot()
    {
        return playerInputs.Gun.Shoot.triggered;
    }
    
    public bool GetReload()
    {
        return playerInputs.Gun.Reload.triggered;
    }


    private void OnDisable()
    {
        playerInputs.Character.Disable();
        playerInputs.Camera.Disable();
        playerInputs.Gun.Disable();
    }
}