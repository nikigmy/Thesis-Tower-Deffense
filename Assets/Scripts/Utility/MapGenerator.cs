using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Level")]
    [SerializeField]
    GameObject[] grassTiles;
    [SerializeField]
    GameObject[] pathTiles;
    [SerializeField]
    GameObject[] environmentTiles;
    [SerializeField]
    GameObject[] spawnTiles;
    [SerializeField]
    GameObject[] objectiveTiles;
    [SerializeField]
    GameObject baseObj;


    [Header("Level Creator")]
    [SerializeField]
    GameObject editTile;
    [SerializeField]
    Material editMaterial;
    [SerializeField]
    Material editMaterialGlow;

    [SerializeField]
    Color GrassColor;
    [SerializeField]
    Color PathColor;
    [SerializeField]
    Color ObjectiveColor;
    [SerializeField]
    Color SpawnColor;
    [SerializeField]
    Color EnvironmentColor;
    [SerializeField]
    Color EmptyColor;
    public Dictionary<Declarations.TileType, Material> TileMaterials;
    public Dictionary<Declarations.TileType, Material> GlowMaterials;


    private Dictionary<Declarations.TileType, GameObject[]> TilesMap;
    private Tile[,] Map;
    public Declarations.IntVector2 MapSize { get; private set; }

    private void Awake()
    {
        if (grassTiles.Length > 0)
        {
            GenerateTileMap();
        }
        else if(editMaterial != null && editMaterialGlow != null)
        {
            GenerateMateralMap();
        }
    }

    public void GenerateTileMap()
    {
        TilesMap = new Dictionary<Declarations.TileType, GameObject[]>();
        TilesMap.Add(Declarations.TileType.Grass, grassTiles);
        TilesMap.Add(Declarations.TileType.Path, pathTiles);
        TilesMap.Add(Declarations.TileType.Environment, environmentTiles);
        TilesMap.Add(Declarations.TileType.Objective, objectiveTiles);
        TilesMap.Add(Declarations.TileType.Spawn, spawnTiles);
    }
    
    public void GenerateMateralMap()
    {
        TileMaterials = new Dictionary<Declarations.TileType, Material>();
        GlowMaterials = new Dictionary<Declarations.TileType, Material>();
        TileMaterials.Add(Declarations.TileType.Grass, new Material(editMaterial) { color = GrassColor, name = "Edit_Grass" });
        TileMaterials.Add(Declarations.TileType.Path, new Material(editMaterial) { color = PathColor, name = "Edit_Path" });
        TileMaterials.Add(Declarations.TileType.Objective, new Material(editMaterial) { color = ObjectiveColor, name = "Edit_Objective" });
        TileMaterials.Add(Declarations.TileType.Spawn, new Material(editMaterial) { color = SpawnColor, name = "Edit_Spawn" });
        TileMaterials.Add(Declarations.TileType.Environment, new Material(editMaterial) { color = EnvironmentColor, name = "Edit_Environment" });
        TileMaterials.Add(Declarations.TileType.Empty, editMaterial);
        foreach (var tileMaterial in TileMaterials)
        {
            var glowMat = new Material(editMaterialGlow) { color = tileMaterial.Value.color };
            glowMat.SetColor("_EmissionColor", tileMaterial.Value.color);
            glowMat.name += "_Glow";
            GlowMaterials.Add(tileMaterial.Key, glowMat);
        }
    }
    public void GenerateMap(Declarations.LevelData levelData)
    {
        ClearMap();
        MapSize = levelData.MapSize;
        Map = new Tile[MapSize.y, MapSize.x];
        for (int row = 0; row < MapSize.y; row++)
        {
            for (int col = 0; col < MapSize.x; col++)
            {
                var tile = GetObjectByTileType(levelData.Map[row, col]);
                Vector3 tilePosition = Helpers.GetPositionForTile(row, col);

                var tileObj = Instantiate(tile, tilePosition, Quaternion.identity, transform).GetComponent<Tile>();
                tileObj.SetData(row, col, levelData.Map[row, col]);
                Map[row, col] = tileObj;
            }
        }

        PutObjectives();
    }

    public void GenerateMapForEdit(Declarations.IntVector2 mapSize, Declarations.TileType[,] map)
    {
        ClearMap();
        MapSize = mapSize;
        Map = new Tile[MapSize.y, MapSize.x];
        for (int row = 0; row < MapSize.y; row++)
        {
            for (int col = 0; col < MapSize.x; col++)
            {
                Vector3 tilePosition = Helpers.GetPositionForTile(row, col);

                var tileObj = Instantiate(editTile, tilePosition, Quaternion.identity, transform).GetComponent<EditableTile>();
                tileObj.SetData(row, col, map[row, col]);
                Map[row, col] = tileObj;
            }
        }
    }

    public Declarations.TileType[,] GetMap()
    {
        var result = new Declarations.TileType[MapSize.y, MapSize.x];
        bool invalid = false;
        for (int row = 0; row < MapSize.y; row++)
        {
            for (int col = 0; col < MapSize.x; col++)
            {
                result[row, col] = Map[row, col].Type;
                if(Map[row, col].Type == Declarations.TileType.Unknown)
                {
                    invalid = true;
                }
            }
        }
        if (invalid)
        {
            Debug.Log("Map is invalid");
        }
        return result;
    }

    private void PutObjectives()
    {
        if (baseObj != null)
        {
            for (int row = 0; row < Map.GetLength(0); row++)
            {
                for (int col = 0; col < Map.GetLength(1); col++)
                {
                    bool canPlaceBase = true;
                    var neibourCells = GetNeibourCells(row, col);
                    if (neibourCells.Count == 6)
                    {
                        foreach (var cell in neibourCells)
                        {
                            if (cell.Type != Declarations.TileType.Objective)
                            {
                                canPlaceBase = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        canPlaceBase = false;
                    }
                    if (canPlaceBase)
                    {
                        Instantiate(baseObj, Helpers.GetPositionForTile(row, col), Quaternion.identity, transform).GetComponent<Tile>();
                    }
                }
            }
        }
    }

    public void ClearMap()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    public List<Tile> GetNeibourCells(int row, int col)
    {
        List<Tile> tiles = new List<Tile>();

        if (row > 0)//top
        {
            tiles.Add(Map[row - 1, col]);
            if (row % 2 == 0)
            {
                if (col > 0)
                {
                    tiles.Add(Map[row - 1, col - 1]);
                }
            }
            else
            {
                if (col + 1 < MapSize.x)
                {
                    tiles.Add(Map[row - 1, col + 1]);
                }
            }
        }
        if (col > 0)//left
        {
            tiles.Add(Map[row, col - 1]);
        }
        if (col + 1 < Map.GetLength(1))//right
        {
            tiles.Add(Map[row, col + 1]);
        }
        if (row + 1 < Map.GetLength(0))//bot
        {
            tiles.Add(Map[row + 1, col]);
            if (row % 2 == 0)
            {
                if (col > 0)
                {
                    tiles.Add(Map[row + 1, col - 1]);
                }
            }
            else
            {
                if (col + 1 < Map.GetLength(1))
                {
                    tiles.Add(Map[row + 1, col + 1]);
                }
            }
        }

        return tiles;
    }

    internal void HighlightNeibours(int row, int col)
    {
        foreach (var cell in GetNeibourCells(row, col))
        {
            cell.GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }

    private GameObject GetObjectByTileType(Declarations.TileType tileType)
    {
        if (TilesMap.ContainsKey(tileType))
        {
            var tilesForType = TilesMap[tileType];
            var tileIndex = UnityEngine.Random.Range(0, tilesForType.Length - 1);
            return tilesForType[tileIndex];
        }
        else
        {
            Debug.Log("no tiles for tile type" + tileType.ToString());
            return null;//new GameObject();//so it does not crash and generates the other tiles of the map
        }
    }

    public Tile GetSpawnTile()
    {
        for (int i = 0; i < Map.GetLength(0); i++)
        {
            for (int j = 0; j < Map.GetLength(1); j++)
            {
                if (Map[i, j].Type == Declarations.TileType.Spawn)
                {
                    return Map[i, j];
                }
            }
        }
        return null;
    }
}
