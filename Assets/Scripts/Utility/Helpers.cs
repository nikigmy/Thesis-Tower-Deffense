using System;
using System.IO;
using UnityEngine;

public class Helpers
{

    public static Vector3 GetPositionForTile(int row, int col)
    {
        var rowAbs = Math.Abs(row);
        float h = (float)((Math.Sqrt(3) / 2));
        return new Vector3(col * h * 2 + (rowAbs % 2 == 1 ? h : 0), 0, -(row * 1.5f));
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
            case "Crystal":
                towerType = Declarations.TowerType.Crystal;
                break;
            case "Tesla":
                towerType = Declarations.TowerType.Tesla;
                break;
            case "Laser":
                towerType = Declarations.TowerType.Laser;
                break;
            case "Radar":
                towerType = Declarations.TowerType.Radar;
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
            case "Rogue":
                enemyType = Declarations.EnemyType.Rogue;
                break;
            case "Boss":
                enemyType = Declarations.EnemyType.Boss;
                break;
            default:
                Debug.Log(string.Format("Unknown enemy type: '{0}'", type));
                enemyType = Declarations.EnemyType.Swordsman;
                return false;
        }
        return true;
    }

    internal static void SaveAndSetSettings()
    {
        Def.Instance.Settings.Export().Save(Path.Combine(Application.dataPath + "/../", Constants.cst_Config + Constants.cst_Xml));
        
        Screen.SetResolution((int)Def.Instance.Settings.Resolution.x, (int)Def.Instance.Settings.Resolution.y, Def.Instance.Settings.Fullscreen);
        QualitySettings.SetQualityLevel(Def.Instance.Settings.QualityLevel, true);
    }

    public static bool GetTileType(char tileId, out Declarations.TileType tileType, bool canHaveEmpty)
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

    internal static string GetTileTypeChar(Declarations.TileType tileType)
    {
        switch (tileType)
        {
            case Declarations.TileType.Grass:
                return "/";
            case Declarations.TileType.Path:
                return "#";
            case Declarations.TileType.Objective:
                return "$";
            case Declarations.TileType.Spawn:
                return "*";
            case Declarations.TileType.Environment:
                return "x";
            default:
                Debug.Log("Unknown tile");
                return "";
        }
    }

    internal static bool IsGroundUnit(Declarations.EnemyType type)
    {
        switch (type)
        {
            case Declarations.EnemyType.Swordsman:
            case Declarations.EnemyType.Golem:
            case Declarations.EnemyType.Rogue:
            case Declarations.EnemyType.Boss:
                return true;
            case Declarations.EnemyType.Dragon:
                return false;
            default:
                Debug.LogError("Unknown enemy type");
                return true;
        }
    }
}