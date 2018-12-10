using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

public class DataReader
{
    private const string cst_Level1 = "Level1";
    private const string cst_Level2 = "Level2";
    private const string cst_Level3 = "Level3";

    private const string cst_Speed = "Speed"; 
    private const string cst_RunSpeed = "RunSpeed";
    private const string cst_Health = "Health";
    private const string cst_StartHealth = "StartHealth";
    private const string cst_StartMoney = "StartMoney";
    private const string cst_Damage = "Damage";
    private const string cst_FireRate = "FireRate";
    private const string cst_Price = "Price";
    private const string cst_Range = "Range";
    private const string cst_UpgradePrice = "UpgradePrice";
    private const string cst_ExplosionRange = "ExplosionRange";
    private const string cst_SlowEffect = "SlowEffect"; 
    private const string cst_SlowDuration = "SlowDuration"; 
    private const string cst_MaxBounces = "MaxBounces";
    private const string cst_BounceRange = "BounceRange"; 
    private const string cst_Map = "Map";
    private const string cst_SpawnData = "SpawnData";
    private const string cst_Wave = "Wave";
    private const string cst_Enemy = "Enemy";
    private const string cst_Delay = "Delay";
    private const string cst_Time = "Time";
    private const string cst_Type = "Type";
    private const string cst_Award = "Award";


    #region LevelReading
    public static bool ReadLevelData(XElement levelFile, out Declarations.LevelData levelData, bool canHaveEmptyTiles = false, bool readWaves = true)
    {
        var mapElement = levelFile.Element(cst_Map);
        var spawnData = levelFile.Element(cst_SpawnData);
        if (mapElement != null && spawnData != null)
        {
            int startMoney;
            int startHealth;
            Declarations.IntVector2 mapSize;
            Declarations.TileType[,] map;
            List<Declarations.WaveData> waves;
            if (int.TryParse(levelFile.Attribute(cst_StartMoney).Value, out startMoney) && int.TryParse(levelFile.Attribute(cst_StartHealth).Value, out startHealth) && ReadMapData(mapElement, out mapSize, out map, canHaveEmptyTiles))
            {
                waves = null;
                if (!readWaves || ReadWaveData(spawnData, out waves))
                {
                    levelData = new Declarations.LevelData(mapSize, map, waves, startMoney, startHealth);
                    return true;
                }
            }
        }
        levelData = null;
        return false;
    }

    private static bool ReadWaveData(XElement spawnData, out List<Declarations.WaveData> waves)
    {
        var wavesData = spawnData.Elements(cst_Wave).ToList();
        waves = new List<Declarations.WaveData>();
        for (int i = 0; i < wavesData.Count; i++)
        {
            var wavePartsData = wavesData[i].Elements().ToList();
            List<Declarations.WavePart> waveParts = new List<Declarations.WavePart>();
            for (int j = 0; j < wavePartsData.Count; j++)
            {
                var partName = wavePartsData[j].Name.LocalName;
                if (partName == cst_Enemy)
                {
                    Declarations.EnemyType enemyType;
                    var enemyTypeAttribute = wavePartsData[j].Attribute(cst_Type);
                    if (enemyTypeAttribute != null && !string.IsNullOrEmpty(enemyTypeAttribute.Value) && Helpers.GetEnemyTypeFromString(enemyTypeAttribute.Value, out enemyType))
                    {
                        if (Def.Instance.EnemyDictionary.ContainsKey(enemyType))
                            waveParts.Add(new Declarations.SpawnWavePart(Def.Instance.EnemyDictionary[enemyType]));
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (partName == cst_Delay)
                {
                    float delay;
                    var timeAttribute = wavePartsData[j].Attribute(cst_Time);
                    if (timeAttribute != null && !string.IsNullOrEmpty(timeAttribute.Value) && float.TryParse(timeAttribute.Value, out delay))
                    {
                        waveParts.Add(new Declarations.DelayWavePart(delay));
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            waves.Add(new Declarations.WaveData(waveParts));
        }
        return true;
    }

    private static bool ReadMapData(XElement mapElement, out Declarations.IntVector2 mapSize, out Declarations.TileType[,] map, bool canHaveEmptyTiles)
    {
        var result = true;
        var width = mapElement.Attribute("Width");
        var height = mapElement.Attribute("Height");
        if (width != null && height != null && !string.IsNullOrEmpty(width.Value) && !string.IsNullOrEmpty(height.Value))
        {
            mapSize = new Declarations.IntVector2(int.Parse(width.Value), int.Parse(height.Value));
            map = new Declarations.TileType[mapSize.y, mapSize.x];

            var lines = mapElement.Value.Trim().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Count() == mapSize.y)
            {
                for (int row = 0; row < mapSize.y; row++)
                {
                    var line = lines[row].Trim();
                    if (line.Count() == mapSize.x)
                    {
                        for (int col = 0; col < mapSize.x; col++)
                        {
                            Declarations.TileType tileType;
                            if (Helpers.GetTileType(line[col], out tileType, canHaveEmptyTiles))
                            {
                                map[row, col] = tileType;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        mapSize = null;
        map = null;
        return result;
    }

    #endregion

    #region TowerReading
    public static bool ReadTowerData(XElement towerData, TowerAssetData assetData, Declarations.TowerType towerType, out Declarations.TowerData tower)
    {
        switch (towerType)
        {
            case Declarations.TowerType.Canon:
                return ReadCanonData(towerData, assetData, out tower);
            case Declarations.TowerType.Plasma:
                return ReadPlasmaData(towerData, assetData, out tower);
            case Declarations.TowerType.Crystal:
                return ReadCrystalData(towerData, assetData, out tower);
            case Declarations.TowerType.Tesla:
                return ReadTeslaData(towerData, assetData, out tower);
            case Declarations.TowerType.Laser:
                return ReadLaserData(towerData, assetData, out tower);
            case Declarations.TowerType.Radar:
                return ReadRadarData(towerData, assetData, out tower);
            default:
                Debug.Log("Unknown tower type");
                break;
        }
        tower = null;
        return false;
    }


    private static bool ReadCanonData(XElement towerData, TowerAssetData assetData, out Declarations.TowerData tower)
    {
        var levels = new Declarations.FiringTowerLevelData[3];

        bool failed = false;
        #region Level1
        var level1 = towerData.Element(cst_Level1);
        if (level1 != null)
        {
            levels[0] = new Declarations.FiringTowerLevelData(ReadInt(level1, cst_Price, ref failed),
                                                        ReadFloat(level1, cst_Range, ref failed),
                                                        ReadFloat(level1, cst_FireRate, ref failed),
                                                        ReadInt(level1, cst_Damage, ref failed));
        }
        else
        {
            failed = true;
        }
        #endregion

        #region Level2
        var level2 = towerData.Element(cst_Level2);
        if (level2 != null)
        {
            levels[1] = new Declarations.FiringTowerLevelData(ReadInt(level2, cst_UpgradePrice, ref failed),
                                                        ReadFloat(level2, cst_Range, ref failed),
                                                        ReadFloat(level2, cst_FireRate, ref failed),
                                                        ReadInt(level2, cst_Damage, ref failed));
        }
        else
        {
            failed = true;
        }
        #endregion

        #region Level3
        var level3 = towerData.Element(cst_Level3);
        if (level3 != null)
        {
            levels[2] = new Declarations.FiringTowerLevelData(ReadInt(level3, cst_UpgradePrice, ref failed), 
                                                        ReadFloat(level3, cst_Range, ref failed), 
                                                        ReadFloat(level3, cst_FireRate, ref failed),
                                                        ReadInt(level3, cst_Damage, ref failed));
        }
        #endregion

        if (failed)
        {
            Debug.Log("Filed to read canon tower");
            tower = null;
            return false;
        }
        else
        {
            tower = new Declarations.CanonTower(assetData, levels);
            return true;
        }
    }

    private static bool ReadPlasmaData(XElement towerData, TowerAssetData assetData, out Declarations.TowerData tower)
    {
        var levels = new Declarations.PlasmaLevelData[3];

        bool failed = false;

        #region Level1
        var level1 = towerData.Element(cst_Level1);

        if (level1 != null)
        {
            levels[0] = new Declarations.PlasmaLevelData(ReadInt(level1, cst_Price, ref failed),
                                                        ReadFloat(level1, cst_Range, ref failed),
                                                        ReadFloat(level1, cst_FireRate, ref failed),
                                                        ReadInt(level1, cst_Damage, ref failed),
                                                        ReadFloat(level1, cst_ExplosionRange, ref failed));
        }
        else
        {
            failed = true;
        }
        #endregion

        #region Level2
        var level2 = towerData.Element(cst_Level2);
        if (level2 != null)
        {
            levels[1] = new Declarations.PlasmaLevelData(ReadInt(level2, cst_UpgradePrice, ref failed),
                                                        ReadFloat(level2, cst_Range, ref failed),
                                                        ReadFloat(level2, cst_FireRate, ref failed),
                                                        ReadInt(level2, cst_Damage, ref failed),
                                                        ReadFloat(level2, cst_ExplosionRange, ref failed));
        }
        else
        {
            failed = true;
        }
        #endregion

        #region Level3
        var level3 = towerData.Element(cst_Level3);
        if (level3 != null)
        {
            levels[2] = new Declarations.PlasmaLevelData(ReadInt(level3, cst_UpgradePrice, ref failed),
                                                        ReadFloat(level3, cst_Range, ref failed),
                                                        ReadFloat(level3, cst_FireRate, ref failed),
                                                        ReadInt(level3, cst_Damage, ref failed),
                                                        ReadFloat(level3, cst_ExplosionRange, ref failed));
        }
        #endregion

        if (failed)
        {
            Debug.Log("Filed to read plasma tower");
            tower = null;
            return false;
        }
        else
        {
            tower = new Declarations.PlasmaTower(assetData, levels);
            return true;
        }
    }

    private static bool ReadCrystalData(XElement towerData, TowerAssetData assetData, out Declarations.TowerData tower)
    {
        var levels = new Declarations.CrystalLevelData[3];

        bool failed = false;

        #region Level1
        var level1 = towerData.Element(cst_Level1);

        if (level1 != null)
        {
            levels[0] = new Declarations.CrystalLevelData(ReadInt(level1, cst_Price, ref failed),
                                                        ReadFloat(level1, cst_Range, ref failed),
                                                        ReadFloat(level1, cst_FireRate, ref failed),
                                                        ReadInt(level1, cst_Damage, ref failed),
                                                        ReadInt(level1, cst_SlowEffect, ref failed),
                                                        ReadFloat(level1, cst_SlowDuration, ref failed));
        }
        else
        {
            failed = true;
        }
        #endregion

        #region Level2
        var level2 = towerData.Element(cst_Level2);
        if (level2 != null)
        {
            levels[1] = new Declarations.CrystalLevelData(ReadInt(level2, cst_UpgradePrice, ref failed),
                                                        ReadFloat(level2, cst_Range, ref failed),
                                                        ReadFloat(level2, cst_FireRate, ref failed),
                                                        ReadInt(level2, cst_Damage, ref failed),
                                                        ReadInt(level2, cst_SlowEffect, ref failed),
                                                        ReadFloat(level2, cst_SlowDuration, ref failed));
        }
        else
        {
            failed = true;
        }
        #endregion

        #region Level3
        var level3 = towerData.Element(cst_Level3);
        if (level3 != null)
        {
            levels[2] = new Declarations.CrystalLevelData(ReadInt(level3, cst_UpgradePrice, ref failed),
                                                        ReadFloat(level3, cst_Range, ref failed),
                                                        ReadFloat(level3, cst_FireRate, ref failed),
                                                        ReadInt(level3, cst_Damage, ref failed),
                                                        ReadInt(level3, cst_SlowEffect, ref failed),
                                                        ReadFloat(level3, cst_SlowDuration, ref failed));
        }
        #endregion

        if (failed)
        {
            Debug.Log("Filed to read crystal tower");
            tower = null;
            return false;
        }
        else
        {
            tower = new Declarations.CrystalTower(assetData, levels);
            return true;
        }
    }

    private static bool ReadTeslaData(XElement towerData, TowerAssetData assetData, out Declarations.TowerData tower)
    {
        var levels = new Declarations.TeslaLevelData[3];

        bool failed = false;

        #region Level1
        var level1 = towerData.Element(cst_Level1);

        if (level1 != null)
        {
            levels[0] = new Declarations.TeslaLevelData(ReadInt(level1, cst_Price, ref failed),
                                                        ReadFloat(level1, cst_Range, ref failed),
                                                        ReadFloat(level1, cst_FireRate, ref failed),
                                                        ReadInt(level1, cst_Damage, ref failed),
                                                        ReadInt(level1, cst_MaxBounces, ref failed),
                                                        ReadFloat(level1, cst_BounceRange, ref failed));
        }
        else
        {
            failed = true;
        }
        #endregion

        #region Level2
        var level2 = towerData.Element(cst_Level2);
        if (level2 != null)
        {
            levels[1] = new Declarations.TeslaLevelData(ReadInt(level2, cst_UpgradePrice, ref failed),
                                                        ReadFloat(level2, cst_Range, ref failed),
                                                        ReadFloat(level2, cst_FireRate, ref failed),
                                                        ReadInt(level2, cst_Damage, ref failed),
                                                        ReadInt(level2, cst_MaxBounces, ref failed),
                                                        ReadFloat(level2, cst_BounceRange, ref failed));
        }
        else
        {
            failed = true;
        }
        #endregion

        #region Level3
        var level3 = towerData.Element(cst_Level3);
        if (level3 != null)
        {
            levels[2] = new Declarations.TeslaLevelData(ReadInt(level3, cst_UpgradePrice, ref failed),
                                                        ReadFloat(level3, cst_Range, ref failed),
                                                        ReadFloat(level3, cst_FireRate, ref failed),
                                                        ReadInt(level3, cst_Damage, ref failed),
                                                        ReadInt(level3, cst_MaxBounces, ref failed),
                                                        ReadFloat(level3, cst_BounceRange, ref failed));
        }
        #endregion

        if (failed)
        {
            Debug.Log("Filed to read tesla tower");
            tower = null;
            return false;
        }
        else
        {
            tower = new Declarations.TeslaTower(assetData, levels);
            return true;
        }
    }

    private static bool ReadLaserData(XElement towerData, TowerAssetData assetData, out Declarations.TowerData tower)
    {
        var levels = new Declarations.DamageTowerLevelData[3];

        bool failed = false;
        #region Level1
        var level1 = towerData.Element(cst_Level1);
        if (level1 != null)
        {
            levels[0] = new Declarations.DamageTowerLevelData(ReadInt(level1, cst_Price, ref failed),
                                                        ReadFloat(level1, cst_Range, ref failed),
                                                        ReadInt(level1, cst_Damage, ref failed));
        }
        else
        {
            failed = true;
        }
        #endregion

        #region Level2
        var level2 = towerData.Element(cst_Level2);
        if (level2 != null)
        {
            levels[1] = new Declarations.DamageTowerLevelData(ReadInt(level2, cst_UpgradePrice, ref failed),
                                                        ReadFloat(level2, cst_Range, ref failed),
                                                        ReadInt(level2, cst_Damage, ref failed));
        }
        else
        {
            failed = true;
        }
        #endregion

        #region Level3
        var level3 = towerData.Element(cst_Level3);
        if (level3 != null)
        {
            levels[2] = new Declarations.DamageTowerLevelData(ReadInt(level3, cst_UpgradePrice, ref failed),
                                                        ReadFloat(level3, cst_Range, ref failed),
                                                        ReadInt(level3, cst_Damage, ref failed));
        }
        #endregion

        if (failed)
        {
            Debug.Log("Filed to read laser tower");
            tower = null;
            return false;
        }
        else
        {
            tower = new Declarations.LaserTower(assetData, levels);
            return true;
        }
    }

    private static bool ReadRadarData(XElement towerData, TowerAssetData assetData, out Declarations.TowerData tower)
    {
        var levels = new Declarations.TowerLevelData[3];

        bool failed = false;
        #region Level1
        var level1 = towerData.Element(cst_Level1);
        if (level1 != null)
        {
            levels[0] = new Declarations.TowerLevelData(ReadInt(level1, cst_Price, ref failed),
                                                        ReadFloat(level1, cst_Range, ref failed));
        }
        else
        {
            failed = true;
        }
        #endregion

        #region Level2
        var level2 = towerData.Element(cst_Level2);
        if (level2 != null)
        {
            levels[1] = new Declarations.TowerLevelData(ReadInt(level2, cst_UpgradePrice, ref failed),
                                                        ReadFloat(level2, cst_Range, ref failed));
        }
        else
        {
            failed = true;
        }
        #endregion

        #region Level3
        var level3 = towerData.Element(cst_Level3);
        if (level3 != null)
        {
            levels[2] = new Declarations.TowerLevelData(ReadInt(level3, cst_UpgradePrice, ref failed),
                                                        ReadFloat(level3, cst_Range, ref failed));
        }
        #endregion

        if (failed)
        {
            Debug.Log("Filed to read radar tower");
            tower = null;
            return false;
        }
        else
        {
            tower = new Declarations.RadarTower(assetData, levels);
            return true;
        }
    }

    private static float ReadFloat(XElement element, string attributeName, ref bool failed)
    {
        var result = 0.0f;
        var attribute = element.Attribute(attributeName);
        if (!(attribute != null && !string.IsNullOrEmpty(attribute.Value) && float.TryParse(attribute.Value, out result)))
        {
            failed = true;
        }
        return result;
    }

    private static int ReadInt(XElement element, string attributeName, ref bool failed)
    {
        int result = 0;
        var attribute = element.Attribute(attributeName);
        if (!(attribute != null && !string.IsNullOrEmpty(attribute.Value) && int.TryParse(attribute.Value, out result)))
        {
            failed = true;
        }
        return result;
    }
    #endregion

    #region EnemyReading
    internal static bool ReadEnemyData(XElement enemyData, EnemyAssetData assetData, Declarations.EnemyType enemyType, out Declarations.EnemyData enemy)
    {
        switch (enemyType)
        {
            case Declarations.EnemyType.Swordsman:
                return ReadBaseEnemyData(enemyData, assetData, out enemy);
            case Declarations.EnemyType.Golem:
                return ReadBaseEnemyData(enemyData, assetData, out enemy);
            case Declarations.EnemyType.Dragon:
                return ReadBaseEnemyData(enemyData, assetData, out enemy);
            case Declarations.EnemyType.Rogue:
                return ReadRogueData(enemyData, assetData, out enemy);
            default:
                Debug.Log("Unknown enemy type");
                break;
        }
        enemy = null;
        return false;
    }

    private static bool ReadRogueData(XElement enemyData, EnemyAssetData assetData, out Declarations.EnemyData enemy)
    {
        bool failed = false;

        int health = ReadInt(enemyData, cst_Health, ref failed);
        float speed = ReadFloat(enemyData, cst_Speed, ref failed);
        float runSpeed = ReadFloat(enemyData, cst_RunSpeed, ref failed);
        int damage = ReadInt(enemyData, cst_Damage, ref failed);
        int award = ReadInt(enemyData, cst_Award, ref failed);

        if (failed)
        {
            enemy = null;
            return false;
        }
        else
        {
            enemy = new Declarations.RogueData(assetData, health, speed, runSpeed, damage, award);
            return true;
        }
    }

    private static bool ReadBaseEnemyData(XElement enemyData, EnemyAssetData assetData, out Declarations.EnemyData enemy)
    {
        bool failed = false;

        int health = ReadInt(enemyData, cst_Health, ref failed);
        float speed = ReadFloat(enemyData, cst_Speed, ref failed);
        int damage = ReadInt(enemyData, cst_Damage, ref failed);
        int award = ReadInt(enemyData, cst_Award, ref failed);
        
        if (failed)
        {
            enemy = null;
            return false;
        }
        else
        {
            enemy = new Declarations.EnemyData(assetData, health, speed, damage, award);
            return true;
        }
    }

    #endregion
}
