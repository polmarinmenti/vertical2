using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine.SceneManagement;

public class MineSweeperManager : MonoBehaviour
{
    public Camera cam;

    [SerializeField] private Transform tilePrefab;
    [SerializeField] private Transform gameHolder;

    private List<Tile> tiles = new List<Tile>();

    private int width;
    private int height;
    private int numMines;

    // Considera ajustar el tama�o del tile seg�n tu dise�o en 3D
    private readonly float tileSize = 0.25f;

    // Start se llama antes de la primera actualizaci�n del frame
    void Start()
    {
        //CreateGameBoard(12, 12, 20); // Ejemplo: tablero de 12x12 con 20 minas
        //ResetGameState();
    }

    public void CreateGameBoard(int width, int height, int numMines)
    {
        // Guarda los par�metros del juego que estamos utilizando
        this.width = width;
        this.height = height;
        this.numMines = numMines;

        // Limpia el tablero anterior si es necesario
        foreach (Transform child in gameHolder)
        {
            Destroy(child.gameObject);
        }
        tiles.Clear();

        // Crea el array de tiles
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                // Posiciona el tile en el lugar correcto (centrado)
                Transform tileTransform = Instantiate(tilePrefab, gameHolder);
                float xIndex = col - ((width - 1) / 2.5f);
                float yIndex = row - ((height - 1) / 2.5f);
                tileTransform.localPosition = new Vector3(xIndex * tileSize, yIndex * tileSize, 0);

                // Mant�n una referencia al tile para configurar el juego
                Tile tile = tileTransform.GetComponent<Tile>();
                tiles.Add(tile);
                tile.gameManager = this;
            }
        }

        StartCoroutine(ExecuteRaycastAfterDelay());
        //FirstTileEmpty();
    }

    // Se asegura de que el primer tile al que disparas est� vac�o
    private void FirstTileEmpty()
    {
        // Lanzar un rayo desde la posici�n de la c�mara hacia adelante
        RaycastHit hit;
        Debug.DrawRay(cam.transform.position, cam.transform.forward, UnityEngine.Color.red, 100f);

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
        {
            Tile tile = hit.collider.GetComponent<Tile>();
            if (tile != null)
            {
                if (tile.mineCount == 0 && !tile.isMine)
                {
                    //Debug.Log("El tile golpeado S� est� vac�o");
                    tile.ClickedTile();
                }
                else
                {
                    //Aqu�, deber�a de resetearse el tablero
                    //Debug.Log("El tile golpeado no est� vac�o");
                    CreateGameBoard(width, height, numMines);
                    ResetGameState();
                }

            }
            else
            {
                Debug.Log("No se ha detectado Tile");
            }
        }
        else
        {
            Debug.Log("Raycast no ha impactado");
        }
    }

    // Delay inventao temporal
    IEnumerator ExecuteRaycastAfterDelay()
    {
        yield return new WaitForNextFrameUnit();
        FirstTileEmpty();
    }

    IEnumerator RecreateGameBoard()
    {
        // Destruye las tiles actuales
        foreach (Transform child in gameHolder)
        {
            Destroy(child.gameObject);
        }
        yield return null; // Espera hasta el pr�ximo frame

        // Crea nuevas tiles y configura el juego
        CreateGameBoard(width, height, numMines);
        // ResetGameState se llama dentro de CreateGameBoard despu�s de instanciar las nuevas tiles
    }


    public void ResetGameState()
    {
        // Randomly shuffle the tile positions to get indices for mine positions.
        int[] minePositions = Enumerable.Range(0, tiles.Count).OrderBy(x => Random.Range(0.0f, 1.0f)).ToArray();

        // Set mines at the first numMines positions.
        for (int i = 0; i < numMines; i++)
        {
            int pos = minePositions[i];
            tiles[pos].isMine = true;
        }

        // Update all the tiles to hold the correct number of mines.
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].mineCount = HowManyMines(i);
        }
    }

    // Given a location work out how many mines are surrounding it.
    private int HowManyMines(int location)
    {
        int count = 0;
        foreach (int pos in GetNeighbours(location))
        {
            if (tiles[pos].isMine)
            {
                count++;
            }
        }
        return count;
    }

    // Given a position, return the positions of all neighbours.
    private List<int> GetNeighbours(int pos)
    {
        List<int> neighbours = new();
        int row = pos / width;
        int col = pos % width;
        // (0,0) is bottom left.
        if (row < (height - 1))
        {
            neighbours.Add(pos + width); // North
            if (col > 0)
            {
                neighbours.Add(pos + width - 1); // North-West
            }
            if (col < (width - 1))
            {
                neighbours.Add(pos + width + 1); // North-East
            }
        }
        if (col > 0)
        {
            neighbours.Add(pos - 1); // West
        }
        if (col < (width - 1))
        {
            neighbours.Add(pos + 1); // East
        }
        if (row > 0)
        {
            neighbours.Add(pos - width); // South
            if (col > 0)
            {
                neighbours.Add(pos - width - 1); // South-West
            }
            if (col < (width - 1))
            {
                neighbours.Add(pos - width + 1); // South-East
            }
        }
        return neighbours;
    }

    public void ClickNeighbours(Tile tile)
    {
        int location = tiles.IndexOf(tile);
        foreach (int pos in GetNeighbours(location))
        {
            tiles[pos].ClickedTile();
        }
    }

    public void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void CheckGameOver()
    {
        // If there are numMines left active then we're done.
        int count = 0;
        foreach (Tile tile in tiles)
        {
            if (tile.active)
            {
                count++;
            }
        }
        if (count == numMines)
        {
            // Flag and disable everything, we're done.
            Debug.Log("Winner!");
            foreach (Tile tile in tiles)
            {
                tile.active = false;
                tile.SetFlaggedIfMine();
            }
        }
    }

    // Click on all surrounding tiles if mines are all flagged.
    public void ExpandIfFlagged(Tile tile)
    {
        int location = tiles.IndexOf(tile);
        // Get the number of flags.
        int flag_count = 0;
        foreach (int pos in GetNeighbours(location))
        {
            if (tiles[pos].flagged)
            {
                flag_count++;
            }
        }
        // If we have the right number click surrounding tiles.
        if (flag_count == tile.mineCount)
        {
            // Clicking a flag does nothing so this is safe.
            ClickNeighbours(tile);
        }
    }

    public void CheckAndUnrootMines(Tile flaggedTile)
    {
        // Verificar si el tile flagged es una mina
        if (!flaggedTile.isMine || !flaggedTile.flagged)
        {
            return; // Si no es una mina o no est� flagged, no hacer nada
        }

        // Inicializar una bandera para seguir el estado de los vecinos
        bool allNeighborsUnrooted = true;

        // Obtener los �ndices de los vecinos y revisar cada uno
        foreach (int neighborIndex in GetNeighbours(tiles.IndexOf(flaggedTile)))
        {
            Tile neighborTile = tiles[neighborIndex];

            // Verificar si el vecino est� activo (no clickeado) y no est� flagged
            if (neighborTile.active && !neighborTile.flagged)
            {
                allNeighborsUnrooted = false; // Si encuentra un vecino no clickeado, marcar la bandera como false
                break; // Salir del bucle, ya que se encontr� un vecino que no cumple la condici�n
            }

            // Verificar si el vecino est� flagged pero no es una mina
            if (neighborTile.flagged && !neighborTile.isMine)
            {
                allNeighborsUnrooted = false; // Si encuentra un tile flagged incorrectamente, marcar la bandera como false
                break; // Salir del bucle por la misma raz�n
            }
        }

        // Si todos los vecinos est�n unrooted (clickeados o flagged correctamente)
        if (allNeighborsUnrooted)
        {
            //Debug.Log("unrooted"); // Mostrar mensaje en el log
            flaggedTile.isMine = false; // Cambiar el estado del tile para que ya no sea una mina

            Rigidbody tileRigidbody = flaggedTile.GetComponent<Rigidbody>();
            tileRigidbody.isKinematic = false;
            tileRigidbody.useGravity = true;
        }
    }

}