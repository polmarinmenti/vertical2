using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using System.Drawing;
using System.Collections;

public class MineSweeperManager : MonoBehaviour
{
    public Camera cam;

    [SerializeField] private Transform tilePrefab;
    [SerializeField] private Transform gameHolder;

    private List<Tile> tiles = new List<Tile>();

    private int width;
    private int height;
    private int numMines;

    // Considera ajustar el tamaño del tile según tu diseño en 3D
    private readonly float tileSize = 0.25f;

    // Start se llama antes de la primera actualización del frame
    void Start()
    {
        //CreateGameBoard(12, 12, 20); // Ejemplo: tablero de 12x12 con 20 minas
        //ResetGameState();
    }

    public void CreateGameBoard(int width, int height, int numMines)
    {
        // Guarda los parámetros del juego que estamos utilizando
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

                // Mantén una referencia al tile para configurar el juego
                Tile tile = tileTransform.GetComponent<Tile>();
                tiles.Add(tile);
                tile.gameManager = this;
            }
        }

        StartCoroutine(ExecuteRaycastAfterDelay());
        //FirstTileEmpty();
    }

    // Se asegura de que el primer tile al que disparas esté vacío
    private void FirstTileEmpty()
    {
        // Lanzar un rayo desde la posición de la cámara hacia adelante
        RaycastHit hit;
        Debug.DrawRay(cam.transform.position, cam.transform.forward, UnityEngine.Color.red, 100f);

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
        {
            Tile tile = hit.collider.GetComponent<Tile>();
            if (tile != null)
            {
                MeshRenderer meshRenderer = hit.collider.gameObject.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    // Comprobar si el material del MeshRenderer es igual a Tile.clickedMaterials[0]
                    if (meshRenderer.material == tile.clickedMaterials[0])
                    {
                        Debug.Log("El tile golpeado SÍ está vacío");
                    }
                    else
                    {
                        Debug.Log("El tile golpeado no está vacío");
                        //ResetGameState();
                        //StartCoroutine(RecreateGameBoard());
                    }
                }
                else
                {
                    Debug.Log("El tile no tiene meshrenderer"); // Esto no pasará nunca (en principio)
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
        yield return null; // Espera hasta el próximo frame

        // Crea nuevas tiles y configura el juego
        CreateGameBoard(12, 12, 16);
        // ResetGameState se llama dentro de CreateGameBoard después de instanciar las nuevas tiles
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
}