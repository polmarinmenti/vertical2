using UnityEngine;

public class FakeTiles : MonoBehaviour
{
    public MineSweeperManager mineSweeperManager;

    private void Start()
    {
        //mineSweeperManager = new MineSweeperManager();
    }

    public void StartGame()
    {
        GetComponent<Collider>().enabled = false;

        // Llamar a CreateGameBoard para crear el juego real
        mineSweeperManager.CreateGameBoard(12, 12, 16); // Ajusta estos valores según sea necesario
        mineSweeperManager.ResetGameState();

        // Destruir el muro falso
        Destroy(this.gameObject);
    }
}
