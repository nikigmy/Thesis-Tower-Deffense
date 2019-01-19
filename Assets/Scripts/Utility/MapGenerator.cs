using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

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
    GameObject Lifter;
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
        else if (editMaterial != null && editMaterialGlow != null)
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

    //private GameObject[] GetCombinedMeshes(GameObject[] grassTiles)        //later
    //{
    //    var result = new GameObject[grassTiles.Length];
    //    for (int i = 0; i < grassTiles.Length; i++)
    //    {
    //        var obj = Instantiate(grassTiles[i], new Vector3(-100,-100,100), Quaternion.identity);
    //        result[i] = obj;
    //        var meshToCombineTo = obj.GetComponent<MeshFilter>();
    //        var allMeshes = obj.GetComponentsInChildren<MeshFilter>();
    //        if (allMeshes.Length > 1)
    //        {
    //            bool hasMainMesh = meshToCombineTo != null;
    //            var meshCountToCombine = hasMainMesh ? allMeshes.Length : allMeshes.Length + 1;
    //            CombineInstance[] combine = new CombineInstance[meshCountToCombine];
    //            for (int j = 0; j < allMeshes.Length; j++)
    //            {
    //                combine[j].mesh = allMeshes[j].sharedMesh;
    //                combine[j].transform = allMeshes[j].transform.localToWorldMatrix;
    //                allMeshes[j].gameObject.SetActive(false);
    //            }
    //            if (hasMainMesh)
    //            {
    //                combine[meshCountToCombine - 1].mesh = meshToCombineTo.sharedMesh;
    //                combine[meshCountToCombine - 1].transform = meshToCombineTo.transform.localToWorldMatrix;
    //                meshToCombineTo.gameObject.SetActive(false);
    //            }
    //            Debug.Log("Combining: " + obj.name + " Mesh count: " + meshCountToCombine);
    //            meshToCombineTo.sharedMesh = new Mesh();
    //            meshToCombineTo.mesh.CombineMeshes(combine);
    //            meshToCombineTo.gameObject.SetActive(true);
    //        }
    //    }
    //    return result;
    //}

    public void GenerateMateralMap()
    {
        TileMaterials = new Dictionary<Declarations.TileType, Material>();
        GlowMaterials = new Dictionary<Declarations.TileType, Material>();
        TileMaterials.Add(Declarations.TileType.Grass, new Material(editMaterial) { color = GrassColor, name = "Edit_Grass" });
        TileMaterials.Add(Declarations.TileType.Path, new Material(editMaterial) { color = PathColor, name = "Edit_Path" });
        TileMaterials.Add(Declarations.TileType.Objective, new Material(editMaterial) { color = ObjectiveColor, name = "Edit_Objective" });
        TileMaterials.Add(Declarations.TileType.Spawn, new Material(editMaterial) { color = SpawnColor, name = "Edit_Spawn" });
        TileMaterials.Add(Declarations.TileType.Environment, new Material(editMaterial) { color = EnvironmentColor, name = "Edit_Environment" });
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
        var startTime = Time.realtimeSinceStartup;
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
                tileObj.transform.rotation = Quaternion.Euler(new Vector3(0,
                    (UnityEngine.Random.Range(0, 2) +
                    UnityEngine.Random.Range(0, 2) +
                    UnityEngine.Random.Range(0, 2) +
                    UnityEngine.Random.Range(0, 2) +
                    UnityEngine.Random.Range(0, 2)) * 60, 0));
                Map[row, col] = tileObj;
            }
        }

        PutObjectives();
        RotateSpawner();

        var levelGenerationTime = Time.realtimeSinceStartup - startTime;
        for (int row = 0; row < MapSize.y; row++)
        {
            for (int col = 0; col < MapSize.x; col++)
            {
                if (Map[row, col].Type == Declarations.TileType.Grass || Map[row, col].Type == Declarations.TileType.Environment)
                {
                    var distance = GetDistanceToClosestFlatTile(row, col);

                    var lift = distance * 0.2f;
                    if (lift > 5)
                    {
                        lift = 5;
                    }
                    lift += UnityEngine.Random.Range(-0.3f, 0.3f);
                    if (lift < 0)
                    {
                        continue;
                    }
                    var currentTileTransform = Map[row, col].transform;
                    var oldPosition = currentTileTransform.position;
                    currentTileTransform.position = new Vector3(oldPosition.x, lift, oldPosition.z);
                    var lifterObj = Instantiate(Lifter, oldPosition, currentTileTransform.rotation, currentTileTransform).GetComponent<Lifter>();
                    lifterObj.transform.localScale = new Vector3(1, lift, 1);

                    Map[row, col].SetLifter(lifterObj);
                    lifterObj.SetTile(Map[row, col]);
                }
            }
        }

        var tileLiftingTime = Time.realtimeSinceStartup - startTime - levelGenerationTime;
        PlaceBorderTiles(15, 13, 6);
        var borderPlacementTime = Time.realtimeSinceStartup - startTime - levelGenerationTime - tileLiftingTime;
        //Debug.Log("Total Seconds for map generation: " + (Time.realtimeSinceStartup - startTime));
        //Debug.Log("Seconds for level generation: " + levelGenerationTime);
        //Debug.Log("Seconds for tile lifting: " + tileLiftingTime);
        //Debug.Log("Seconds for border placement: " + borderPlacementTime);
    }

    private void PlaceBorderTiles(int side, int top, int bot)
    {
        for (int i = 1; i <= side; i++)
        {
            for (int j = 0; j < MapSize.y; j++)
            {
                PlaceBorderTile(j, MapSize.x + i - 1, GetRandomFactor(i));
                PlaceBorderTile(j, -i, GetRandomFactor(i));
            }
        }
        for (int i = MapSize.x / 2; i < MapSize.x + side; i++)
        {
            for (int j = -1; j >= -top; j--)
            {
                PlaceBorderTile(j, i, GetRandomFactor(-j));
            }
            for (int j = MapSize.y; j < MapSize.y + bot; j++)
            {
                PlaceBorderTile(j, i, GetRandomFactor(j - MapSize.y + 1));
            }
        }
        for (int i = MapSize.x / 2 - 1; i >= -side; i--)
        {
            for (int j = -1; j >= -top; j--)
            {
                PlaceBorderTile(j, i, GetRandomFactor(-j));
            }
            for (int j = MapSize.y; j < MapSize.y + bot; j++)
            {
                PlaceBorderTile(j, i, GetRandomFactor(j - MapSize.y + 1));
            }
        }
    }

    private float GetRandomFactor(int offsetFromMap)
    {
        if (offsetFromMap < 3)
        {
            return 0.2f;
        }
        else if (offsetFromMap < 5)
        {
            return 0.3f;
        }
        else
        {
            return 0.5f;
        }
    }

    private void PlaceBorderTile(int row, int col, float randomFactor)
    {
        var positionForTile = Helpers.GetPositionForTile(row, col);
        float height = 0;
        var countOfTiles = 0;
        var nearTileHeights = new List<float>();
        foreach (var colider in Physics.OverlapSphere(positionForTile, 2.5f))
        {
            var tile = colider.gameObject.GetComponent<Tile>();
            if (tile == null)
            {
                var lifter = colider.gameObject.GetComponent<Lifter>();
                if (lifter != null && lifter.Tile != null)
                {
                    tile = lifter.Tile;
                }
            }

            if (tile != null)
            {
                height += tile.transform.position.y;
                countOfTiles++;
            }
        }
        if (countOfTiles > 0)
        {
            height /= countOfTiles;
        }
        else
        {
            Debug.Log("No neibours for tile! Row: " + row + " Col: " + col);
        }
        height += UnityEngine.Random.Range(-randomFactor, randomFactor);
        if (height < 0)
        {
            height = 0;
        }

        var tileObj = Instantiate(GetObjectByTileType(Declarations.TileType.Environment), positionForTile + (Vector3.up * height), Quaternion.identity, transform).GetComponent<Tile>();

        tileObj.SetData(row, col, Declarations.TileType.Environment);
        tileObj.transform.rotation = Quaternion.Euler(new Vector3(0,
            (UnityEngine.Random.Range(0, 2) +
            UnityEngine.Random.Range(0, 2) +
            UnityEngine.Random.Range(0, 2) +
            UnityEngine.Random.Range(0, 2) +
            UnityEngine.Random.Range(0, 2)) * 60, 0));
        if (height > 0)
        {
            var lifterObj = Instantiate(Lifter, positionForTile, Quaternion.identity, tileObj.transform).GetComponent<Lifter>();
            lifterObj.transform.localScale = new Vector3(1, height, 1);
            lifterObj.SetTile(tileObj);
            tileObj.SetLifter(lifterObj);
        }
    }

    private void RotateSpawner()
    {
        var spawnTile = GetSpawnTile();
        var path = GetNeibourCells(spawnTile.Row, spawnTile.Col).Find(x => x.Type == Declarations.TileType.Path);

        spawnTile.transform.rotation = Quaternion.Euler(0, Quaternion.LookRotation((path.transform.position - spawnTile.transform.position)).eulerAngles.y + 90, 0);
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
                if (Map[row, col].Type == Declarations.TileType.Unknown)
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

    private int GetDistanceToClosestFlatTile(int row, int col)
    {
        var offset = 1;
        var paths = new List<Tile>();
        bool found = false;
        while (!found && offset <= 15)
        {
            var minCol = col - offset;
            var maxCol = col + offset;
            var minRow = row - offset;
            var maxRow = row + offset;
            if (minCol < 0)
            {
                minCol = 0;
            }
            if (maxCol >= MapSize.x)
            {
                maxCol = MapSize.x - 1;
            }
            if (minRow < 0)
            {
                minRow = 0;
            }
            if (maxRow >= MapSize.y)
            {
                maxRow = MapSize.y - 1;
            }

            for (int i = minCol; i <= maxCol; i++)
            {
                if (Map[minRow, i].Type == Declarations.TileType.Path || Map[minRow, i].Type == Declarations.TileType.Objective || Map[minRow, i].Type == Declarations.TileType.Spawn)
                {
                    paths.Add(Map[minRow, i]);
                    found = true;
                }
                if (Map[maxRow, i].Type == Declarations.TileType.Path || Map[maxRow, i].Type == Declarations.TileType.Objective || Map[maxRow, i].Type == Declarations.TileType.Spawn)
                {
                    paths.Add(Map[maxRow, i]);
                    found = true;
                }
            }
            for (int i = minRow + 1; i <= maxRow - 1; i++)
            {
                if (Map[i, minCol].Type == Declarations.TileType.Path || Map[i, minCol].Type == Declarations.TileType.Objective || Map[i, minCol].Type == Declarations.TileType.Spawn)
                {
                    paths.Add(Map[i, minCol]);
                    found = true;
                }
                if (Map[i, maxCol].Type == Declarations.TileType.Path || Map[i, maxCol].Type == Declarations.TileType.Objective || Map[i, maxCol].Type == Declarations.TileType.Spawn)
                {
                    paths.Add(Map[i, maxCol]);
                    found = true;
                }
            }
            if (minRow == 0 && maxRow == MapSize.y - 1 && minCol == 0 && maxCol == MapSize.x - 1)
            {
                break;
            }
            if (offset > MapSize.x && offset > MapSize.y)
            {
                Debug.LogError("Fix this up");
                break;
            }

            offset++;
        }

        if (found)
        {
            var tilePosition = Map[row, col].transform.position;
            float closestDistance = Vector3.Distance(paths[0].transform.position, tilePosition);
            for (int i = 1; i < paths.Count; i++)
            {
                var distance = Vector3.Distance(paths[i].transform.position, tilePosition);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                }
            }
            return (int)closestDistance;
        }
        else
        {
            return 5;
        }
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
