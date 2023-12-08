using UnityEngine;

public class LevelEndTrigger : MonoBehaviour
{
    [SerializeField] private GameObject endLevelCanvas;
    [SerializeField] private Movement playerMovement;
    [SerializeField] private CameraController cam;
    [SerializeField] private Gun_Weapon gun;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            endLevelCanvas.SetActive(true);
            Cursor.lockState = CursorLockMode.None; // Desbloquea el cursor
            Cursor.visible = true; // Hace el cursor visible

            if (playerMovement != null)
            {
                playerMovement.enabled = false; // Desactiva el script de movimiento
            }
            else
            {
                Debug.Log("playerMovement no detectado");
            }

            if (cam != null)
            {
                cam.enabled = false; // Desactiva el script de movimiento
            }
            else
            {
                Debug.Log("cam no detectada");
            }

            if (gun != null)
            {
                gun.enabled = false; // Desactiva el script de movimiento
            }
            else
            {
                Debug.Log("gun no detectada");
            }
        }
    }

}
