using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;


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
    public GameObject panelRecompensa; 
    public Animator animadorRecompensa; 
    public string nombreAnimacionRecompensa = "Recompensa"; // Nombre del trigger o estado de animación
    
    private List<GameObject> tiles = new List<GameObject>();
    private Vector2 emptySpace;
    
    // Variable para bloquear el puzzle
    public static bool puzzleBloqueado = false;
    private bool puzzleCompletado = false;

    
    // I copied this script from the internet. only god knows howe it works now.

    
    void Start()
    {
       
        
    }

    public void setup()
    {
        
        CreateBoard();
        Shuffle();
        puzzleCamera.gameObject.SetActive(true);
    }

    void CreateBoard()
    {
        
        float size = 100f; // tile size
        emptySpace = new Vector2(cols - 1, rows - 1);
        Component tilesArray = this.GetComponent<TilesArray>();

        int number = 1;
        int textureIndex = 0;
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (x == cols - 1 && y == rows - 1) continue; // leave empty

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
        // Simple shuffle: randomize tile moves
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
        if (puzzleCompletado) return; // Evitar múltiples activaciones
        
        int number = 1;
        foreach (var tile in tiles)
        {
            int expectedX = (number - 1) % cols;
            int expectedY = (number - 1) / cols;
            if (tile.GetComponent<Tile>().pos != new Vector2(expectedX, expectedY))
                return;
            number++;
        }
        
        // ¡Victoria!
        Debug.Log("You Win!");
        puzzleCompletado = true;
        puzzleBloqueado = true; 
        
        StartCoroutine(MostrarRecompensa());
    }
    
    private IEnumerator MostrarRecompensa()
    {
        // Esperar el tiempo configurado
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
        
        // Mostrar panel de recompensa si existe
        if (panelRecompensa != null)
        {
            panelRecompensa.SetActive(true);
        }
        
        // Reproducir animación de recompensa si existe
        if (animadorRecompensa != null)
        {
            animadorRecompensa.SetTrigger(nombreAnimacionRecompensa);
        }
        
        // Desbloquear el movimiento del jugador
        popUpGame.movimientoBloqueado = false;
        
        Debug.Log("¡Puzzle completado y cerrado permanentemente!");
    }
    
    // Método público para cerrar el panel de recompensa (llamar desde un botón o evento)
    public void CerrarRecompensa()
    {
        if (panelRecompensa != null)
        {
            panelRecompensa.SetActive(false);
        }
    }
}