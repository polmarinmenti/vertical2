using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Tile : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material unclickedMaterial;
    [SerializeField] private Material flaggedMaterial;
    [SerializeField] public Material[] clickedMaterials; // Array de materiales para cada número
    [SerializeField] private Material mineMaterial;
    [SerializeField] private Material mineWrongMaterial;
    [SerializeField] private Material mineHitMaterial;

    public MineSweeperManager gameManager;

    private MeshRenderer meshRenderer;
    public bool flagged = false;
    public bool active = true;
    public bool isMine = false;
    public int mineCount = 0;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = unclickedMaterial;
    }

    private void FixedUpdate()
    {
        if (flagged)
        {
            gameManager.CheckAndUnrootMines(this); // esto es mega nazi pero no tengo tiempo, Dios perdóname
        }
    }

    public void ToggleFlag()
    {
        flagged = !flagged;
        meshRenderer.material = flagged ? flaggedMaterial : unclickedMaterial;
    }

    public void ClickedTile()
    {
        if (active && !flagged)
        {
            active = false;
            if (isMine)
            {
                meshRenderer.material = mineHitMaterial;
                gameManager.GameOver();
            }
            else
            {
                meshRenderer.material = clickedMaterials[mineCount];

                // Desactivar el collider para hacer el tile atravesable
                Collider tileCollider = GetComponent<Collider>();
                if (tileCollider != null)
                {
                    tileCollider.enabled = false;
                }

                if (mineCount == 0)
                {
                    gameManager.ClickNeighbours(this);
                }
                gameManager.CheckGameOver();
            }
        }
    }

    public void SetFlaggedIfMine()
    {
        if (isMine)
        {
            flagged = true;
            meshRenderer.material = flaggedMaterial;
        }
    }
}