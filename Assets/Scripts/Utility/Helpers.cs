using System;
using UnityEngine;

public class Helpers {

    public static Vector3 GetPositionForTile(int row, int col)
    {
        float h = (float)((Math.Sqrt(3) / 2));
        return new Vector3(col * h * 2 + (row % 2 == 1 ? h : 0), 0, -(row * 1.5f));
    }

    public static bool GetTowerTypeFromString(string type, out Declarations.TowerType towerType)
    {
        if (string.IsNullOrEmpty(type))
        {
            towerType = Declarations.TowerType.Canon;
            return false;
        }
        switch (type)
        {
            case "Canon":
                towerType = Declarations.TowerType.Canon;
                break;
            case "Plasma":
                towerType = Declarations.TowerType.Plasma;
                    break;
            default:
                towerType = Declarations.TowerType.Canon;
                return false;
        }
        return true;
    }
    public static bool GetEnemyTypeFromString(string type, out Declarations.EnemyType enemyType)
    {
        if (string.IsNullOrEmpty(type))
        {
            enemyType = Declarations.EnemyType.Capsule;
            return false;
        }
        switch (type)
        {
            case "Capsule":
                enemyType = Declarations.EnemyType.Capsule;
                break;
            case "ElementalGolem":
                enemyType = Declarations.EnemyType.Golem;
                break;
            default:
                enemyType = Declarations.EnemyType.Capsule;
                return false;
        }
        return true;
    }

    public static bool GetTileType(char tileId, out Declarations.TileType tileType)
    {
        switch (tileId)
        {
            case '/':
                tileType = Declarations.TileType.Grass;
                break;
            case '#':
                tileType = Declarations.TileType.Path;
                break;
            case '$':
                tileType = Declarations.TileType.Objective;
                break;
            case '*':
                tileType = Declarations.TileType.Spawn;
                break;
            case 'x':
                tileType = Declarations.TileType.Environment;
                break;
            default:
                tileType = Declarations.TileType.Unknown;
                break;
        }
        return tileType != Declarations.TileType.Unknown;
    }
}
