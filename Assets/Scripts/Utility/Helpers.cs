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
            Debug.Log("Enemy type cannon be null or empty");
            enemyType = Declarations.EnemyType.Swordsman;
            return false;
        }
        switch (type)
        {
            case "Swordsman":
                enemyType = Declarations.EnemyType.Swordsman;
                break;
            case "Golem":
                enemyType = Declarations.EnemyType.Golem;
                break;
            case "Dragon":
                enemyType = Declarations.EnemyType.Dragon;
                break;
            default:
                Debug.Log(string.Format("Unknown enemy type: '{0}'", type));
                enemyType = Declarations.EnemyType.Swordsman;
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

    internal static bool IsGroundUnit(Declarations.EnemyType type)
    {
        switch (type)
        {
            case Declarations.EnemyType.Swordsman:
                return true;
            case Declarations.EnemyType.Golem:
                return true;
            case Declarations.EnemyType.Dragon:
                return false;
            default:
                Debug.LogError("Unknown enemy type");
                return true;
        }
    }
}
