using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class PaintManager : MonoBehaviour
{
    public Declarations.IntVector2 CurrectMousePos = new Declarations.IntVector2(-5, -5);
    public UnityEvent UpdateTiles = new UnityEvent();
    public Declarations.TileType CurrentTileType;
    public bool Painting = false;
    public float BrushSize = 1.7f;

    [SerializeField]
    Image grassImage;
    [SerializeField]
    Image pathImage;
    [SerializeField]
    Image objectiveImage;
    [SerializeField]
    Image spawnImage;
    [SerializeField]
    Image environmentImage;

    Image currentImage;

    private void Awake()
    {
        CurrentTileType = Declarations.TileType.Unknown;
    }

    public void ChangeMousePos(Declarations.IntVector2 mousePos)
    {
        CurrectMousePos = mousePos;
        UpdateTiles.Invoke();
    }

    public void SetBrushSize(float value)
    {
        BrushSize = (int)value * 1.4f;
        CurrectMousePos = new Declarations.IntVector2(-5, -5);
        UpdateTiles.Invoke();
    }

    //because unity does not support enum as parameter, maybe there is a better way
    public void SetTile(int index)
    {
        CurrentTileType = (Declarations.TileType)index;
        if (currentImage != null)
        {
            currentImage.color = new Color(1, 1, 1, 1);
        }
        switch (CurrentTileType)
        {
            case Declarations.TileType.Environment:
                currentImage = environmentImage;
                break;
            case Declarations.TileType.Grass:
                currentImage = grassImage;
                break;
            case Declarations.TileType.Path:
                currentImage = pathImage;
                break;
            case Declarations.TileType.Objective:
                currentImage = objectiveImage;
                break;
            case Declarations.TileType.Spawn:
                currentImage = spawnImage;
                break;
        }

        currentImage.color = new Color(1, 1, 0, 1);
    }

    public void Update()
    {
        if (Painting && Input.GetMouseButtonUp(0))
        {
            Painting = false;
        }
    }
}
