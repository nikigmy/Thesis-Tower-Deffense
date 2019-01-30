using UnityEngine;
using UnityEngine.Events;

public class PaintManager : MonoBehaviour
{
    public Declarations.IntVector2 CurrectMousePos = new Declarations.IntVector2(-5, -5);
    public UnityEvent UpdateTiles = new UnityEvent();
    public Declarations.TileType CurrentTileType;
    public bool Painting = false;
    public int BrushSize = 1;

    private void Awake()
    {
        CurrentTileType = Declarations.TileType.Unknown;
    }

    public void ChangeMousePos(Declarations.IntVector2 mousePos)
    {
        CurrectMousePos = mousePos;
        UpdateTiles.Invoke();
    }

    public void SetBrushSize(int value)
    {
        BrushSize = value;
        CurrectMousePos = new Declarations.IntVector2(-5, -5);
        UpdateTiles.Invoke();
    }

    //because unity does not support enum as parameter, maybe there is a better way
    public void SetTile(int index)
    {
        CurrentTileType = (Declarations.TileType)index;
    }

    public void Update()
    {
        if (Painting && Input.GetMouseButtonUp(0))
        {
            Painting = false;
        }
    }
}
