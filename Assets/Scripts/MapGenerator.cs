using System;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
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

    public Dictionary<Declarations.TileType, GameObject[]> TilesMap;
    public Tile[,] Map;

    private void Awake()
    {
        GenerateTileMap();
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

    public void GenerateMap(Declarations.LevelData levelData)
    {
        ClearMap();
        Map = new Tile[levelData.MapSize.y, levelData.MapSize.x];
        for (int row = 0; row < levelData.MapSize.y; row++)
        {
            for (int col = 0; col < levelData.MapSize.x; col++)
            {
                var tile = GetObjectByTileType(levelData.Map[row, col]);
                Vector3 tilePosition = Helpers.GetPositionForTile(row, col);

                var tileObj = Instantiate(tile, tilePosition, Quaternion.identity, transform).GetComponent<Tile>();
                tileObj.SetData(row, col, levelData.Map[row, col]);
                Map[row, col] = tileObj;
            }
        }
    }

    public void ClearMap()
    {
        if(Map != null)
        {
            for (int row = 0; row < Map.GetLength(0); row++)
            {
                for (int col = 0; col < Map.GetLength(1); col++)
                {
                    if(Map[row, col] != null)
                    {
                        DestroyImmediate(Map[row, col].gameObject);
                    }
                }
            }
        }
    }

    public List<Tile> GetNeibourCells(int row, int col)
    {
        List<Tile> tiles = new List<Tile>();

        if(row > 0)//top
        {
            tiles.Add(Map[row - 1, col]);
            if (row % 2 == 0)
            {
                if (col - 1 > 0)
                {
                    tiles.Add(Map[row - 1, col - 1]);
                }
            }
            else
            {
                if (col + 1 < Map.GetLength(1))
                {
                    tiles.Add(Map[row - 1, col + 1]);
                }
            }
        }
        if (col - 1 > 0)//left
        {
            tiles.Add(Map[row, col - 1]);
        }
        if (col + 1 < Map.GetLength(1))//right
        {
            tiles.Add(Map[row, col + 1]);
        }
        if (row < Map.GetLength(0))//bot
        {
            tiles.Add(Map[row + 1, col]);
            if (row % 2 == 0)
            {
                if (col - 1 > 0)
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
                if(Map[i, j].Type == Declarations.TileType.Spawn)
                {
                    return Map[i, j];
                }
            }
        }
        return null;
    }
}
