using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public Vector2 pos;
    private BoardManager board;

    public void Init(Vector2 startPos, BoardManager manager)
    {
        pos = startPos;
        board = manager;
    }

    public void Move(Vector2 newPos)
    {
        pos = newPos;
        RectTransform rt = GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(newPos.x * 100f, -newPos.y * 100f);
    }

    public void OnClick()
    {
        board.MoveTile(this);
    }
}