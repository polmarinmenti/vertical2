using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float sensitivity = 100f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float smoothSpeed = 0.125f;
    private float rotationX = 0f;
    private float rotationY = 0f;

    private Input_Manager inputManager;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        inputManager = Input_Manager._INPUT_MANAGER;
    }

    void Update()
    {
        float mouseX = inputManager.GetMouseX() * sensitivity * Time.deltaTime;
        float mouseY = inputManager.GetMouseY() * sensitivity * Time.deltaTime;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        rotationY += mouseX;

        //transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        playerTransform.localRotation = Quaternion.Euler(0f, rotationY, 0f);
    }

    void LateUpdate()
    {
        // Sigue al jugador
        Vector3 desiredPosition = playerTransform.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // Alinea la rotación horizontal de la cámara con el jugador
        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }
}
