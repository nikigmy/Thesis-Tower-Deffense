using UnityEngine;

public class Tile : MonoBehaviour
{
    public int Row { get; private set; }
    public int Col { get; private set; }
    public Declarations.TileType Type { get; private set; }

    [ContextMenu("HighlightNeibours")]
    private void HighlightNeibours()
    {
        var mapGenerator = FindObjectOfType<MapGenerator>();
        mapGenerator.HighlightNeibours(Row, Col);
    }

    public void SetData(int row, int col, Declarations.TileType type)
    {
        Row = row;
        Col = col;
        Type = type;
    }
}
