using UnityEngine;
using UnityEngine.UI;
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

    private List<GameObject> tiles = new List<GameObject>();
    private Vector2 emptySpace;

    void Start()
    {
        CreateBoard();
        Shuffle();
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
    }
}