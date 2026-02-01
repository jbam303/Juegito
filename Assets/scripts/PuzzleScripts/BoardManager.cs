using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class BoardManager : MonoBehaviour
{
    public int rows = 4;
    public int cols = 4;
    public GameObject tilePrefab;
    public RectTransform board;
    public TilesArray TilesArray;
    public bool interact_01 = false;
    public bool interact_02 = false;
    public bool interact_03 = false;
    public Camera puzzleCamera;

    [Header("Configuración de Victoria")]
    public float tiempoEsperaRecompensa = 3f;
    public WinPanel winPanel; // Referencia al WinPanel
    
    [Header("Crosshair")]
    public GameObject crosshair; // Referencia al crosshair
    
    private List<GameObject> tiles = new List<GameObject>();
    private Vector2 emptySpace;
    
    public static bool puzzleBloqueado = false;
    private bool puzzleCompletado = false;
    private bool crosshairEstabActivo = false;

    void Start()
    {
        
    }

    public void setup()
    {
        // Ocultar crosshair al abrir el puzzle
        OcultarCrosshair();
        
        CreateBoard();
        Shuffle();
        puzzleCamera.gameObject.SetActive(true);
        
        Debug.Log("[PUZZLE] Puzzle iniciado, crosshair oculto");
    }
    
    void OcultarCrosshair()
    {
        if (crosshair != null)
        {
            crosshairEstabActivo = crosshair.activeSelf;
            crosshair.SetActive(false);
        }
    }
    
    void MostrarCrosshair()
    {
        if (crosshair != null && crosshairEstabActivo)
        {
            crosshair.SetActive(true);
        }
    }

    void CreateBoard()
    {
        float size = 100f;
        emptySpace = new Vector2(cols - 1, rows - 1);
        Component tilesArray = this.GetComponent<TilesArray>();

        int number = 1;
        int textureIndex = 0;
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (x == cols - 1 && y == rows - 1) continue;

                GameObject tile = Instantiate(tilePrefab, board);
                tile.GetComponentInChildren<Text>().text = number.ToString();

                RectTransform rt = tile.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(x * size, -y * size);

                tile.GetComponent<Tile>().Init(new Vector2(x, y), this);

                tile.GetComponent<RawImage>().texture = TilesArray.TileTexture[textureIndex];

                tiles.Add(tile);
                number++;
                textureIndex++;
            }
        }
    }

    public bool IsNextToEmpty(Vector2 pos)
    {
        return (Mathf.Abs(pos.x - emptySpace.x) == 1 && pos.y == emptySpace.y) ||
               (Mathf.Abs(pos.y - emptySpace.y) == 1 && pos.x == emptySpace.x);
    }

    public void MoveTile(Tile tile)
    {
        if (IsNextToEmpty(tile.pos))
        {
            Vector2 oldPos = tile.pos;
            tile.Move(emptySpace);
            emptySpace = oldPos;
        }

        CheckWin();
    }

    void Shuffle()
    {
        for (int i = 0; i < 100; i++)
        {
            foreach (var tile in tiles)
            {
                if (IsNextToEmpty(tile.GetComponent<Tile>().pos))
                    MoveTile(tile.GetComponent<Tile>());
            }
        }
    }

    public void CheckWin()
    {
        if (puzzleCompletado) return;
        
        int number = 1;
        foreach (var tile in tiles)
        {
            int expectedX = (number - 1) % cols;
            int expectedY = (number - 1) / cols;
            if (tile.GetComponent<Tile>().pos != new Vector2(expectedX, expectedY))
                return;
            number++;
        }
        
        Debug.Log("You Win!");
        puzzleCompletado = true;
        puzzleBloqueado = true;
        
        StartCoroutine(MostrarRecompensa());
    }
    
    private IEnumerator MostrarRecompensa()
    {
        yield return new WaitForSeconds(tiempoEsperaRecompensa);
        
        // Destruir todas las tiles del puzzle
        foreach (var tile in tiles)
        {
            if (tile != null)
            {
                Destroy(tile);
            }
        }
        tiles.Clear();
        
        // Desactivar el tablero del puzzle
        if (board != null)
        {
            board.gameObject.SetActive(false);
        }
        
        // Desactivar la cámara del puzzle
        if (puzzleCamera != null)
        {
            puzzleCamera.gameObject.SetActive(false);
        }
        
        // Marcar el puzzle como terminado permanentemente
        popUpGame.puzzleTerminado = true;
        
        // Mostrar WinPanel (el crosshair se maneja dentro del WinPanel)
        if (winPanel != null)
        {
            // Pasar la referencia del crosshair al WinPanel si no la tiene
            if (winPanel.crosshair == null && crosshair != null)
            {
                winPanel.crosshair = crosshair;
            }
            
            winPanel.MostrarPanelVictoria();
            Debug.Log("[PUZZLE] WinPanel mostrado");
        }
        else
        {
            // Si no hay WinPanel, mostrar crosshair y desbloquear
            MostrarCrosshair();
            popUpGame.movimientoBloqueado = false;
            Debug.Log("[PUZZLE] No hay WinPanel, desbloqueando directamente");
        }
        
        Debug.Log("¡Puzzle completado!");
    }
    
    public void CerrarRecompensa()
    {
        if (winPanel != null)
        {
            winPanel.CerrarYContinuar();
        }
    }
}