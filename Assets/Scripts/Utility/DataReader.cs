using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

public class DataReader
{
    #region LevelReading
    public static bool ReadLevelData(XElement levelFile, out Declarations.LevelData levelData, bool canHaveEmptyTiles = false, bool readWaves = true)
    {
        var mapElement = levelFile.Element(Constants.cst_Map);
        var spawnData = levelFile.Element(Constants.cst_SpawnData);
        if (mapElement != null && spawnData != null)
        {
            int startMoney;
            int startHealth;
            string name;
            Declarations.IntVector2 mapSize;
            Declarations.TileType[,] map;
            List<Declarations.WaveData> waves;
            if (levelFile.Attribute(Constants.cst_StartMoney) != null && int.TryParse(levelFile.Attribute(Constants.cst_StartMoney).Value, out startMoney) &&
                levelFile.Attribute(Constants.cst_StartHealth) != null && int.TryParse(levelFile.Attribute(Constants.cst_StartHealth).Value, out startHealth) &&
                levelFile.Attribute(Constants.cst_Name) != null &&
                ReadMapData(mapElement, out mapSize, out map, canHaveEmptyTiles))
            {
                name = levelFile.Attribute(Constants.cst_Name).Value;
                waves = null;
                if (!readWaves || ReadWaveData(spawnData, out waves))
                {
                    levelData = new Declarations.LevelData(name, mapSize, map, waves, startMoney, startHealth);
                    return true;
                }
            }
        }
        levelData = null;
        return false;
    }

    private static bool ReadWaveData(XElement spawnData, out List<Declarations.WaveData> waves)
    {
        var wavesData = spawnData.Elements(Constants.cst_Wave).ToList();
        waves = new List<Declarations.WaveData>();
        for (int i = 0; i < wavesData.Count; i++)
        {
            var wavePartsData = wavesData[i].Elements().ToList();
            List<Declarations.WavePart> waveParts = new List<Declarations.WavePart>();
            for (int j = 0; j < wavePartsData.Count; j++)
            {
                var partName = wavePartsData[j].Name.LocalName;
                if (partName == Constants.cst_Enemy)
                {
                    Declarations.EnemyType enemyType;
                    var enemyTypeAttribute = wavePartsData[j].Attribute(Constants.cst_Type);
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
                else if (partName == Constants.cst_Delay)
                {
                    float delay;
                    var timeAttribute = wavePartsData[j].Attribute(Constants.cst_Time);
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
        var width = mapElement.Attribute(Constants.cst_Width);
        var height = mapElement.Attribute(Constants.cst_Height);
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
        var level1 = towerData.Element(Constants.cst_Level1);
        if (level1 != null)
        {
            levels[0] = new Declarations.FiringTowerLevelData(ReadInt(level1, Constants.cst_Price, ref failed),
                                                        ReadFloat(level1, Constants.cst_Range, ref failed),
                                                        ReadFloat(level1, Constants.cst_FireRate, ref failed),
                                                        ReadInt(level1, Constants.cst_Damage, ref failed));
        }
        else
        {
            failed = true;
        }
        #endregion

        #region Level2
        var level2 = towerData.Element(Constants.cst_Level2);
        if (level2 != null)
        {
            levels[1] = new Declarations.FiringTowerLevelData(ReadInt(level2, Constants.cst_UpgradePrice, ref failed),
                                                        ReadFloat(level2, Constants.cst_Range, ref failed),
                                                        ReadFloat(level2, Constants.cst_FireRate, ref failed),
                                                        ReadInt(level2, Constants.cst_Damage, ref failed));
        }
        else
        {
            failed = true;
        }
        #endregion

        #region Level3
        var level3 = towerData.Element(Constants.cst_Level3);
        if (level3 != null)
        {
            levels[2] = new Declarations.FiringTowerLevelData(ReadInt(level3, Constants.cst_UpgradePrice, ref failed),
                                                        ReadFloat(level3, Constants.cst_Range, ref failed),
                                                        ReadFloat(level3, Constants.cst_FireRate, ref failed),
                                                        ReadInt(level3, Constants.cst_Damage, ref failed));
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
        var level1 = towerData.Element(Constants.cst_Level1);

        if (level1 != null)
        {
            levels[0] = new Declarations.PlasmaLevelData(ReadInt(level1, Constants.cst_Price, ref failed),
                                                        ReadFloat(level1, Constants.cst_Range, ref failed),
                                                        ReadFloat(level1, Constants.cst_FireRate, ref failed),
                                                        ReadInt(level1, Constants.cst_Damage, ref failed),
                                                        ReadFloat(level1, Constants.cst_ExplosionRange, ref failed));
        }
        else
        {
            failed = true;
        }
        #endregion

        #region Level2
        var level2 = towerData.Element(Constants.cst_Level2);
        if (level2 != null)
        {
            levels[1] = new Declarations.PlasmaLevelData(ReadInt(level2, Constants.cst_UpgradePrice, ref failed),
                                                        ReadFloat(level2, Constants.cst_Range, ref failed),
                                                        ReadFloat(level2, Constants.cst_FireRate, ref failed),
                                                        ReadInt(level2, Constants.cst_Damage, ref failed),
                                                        ReadFloat(level2, Constants.cst_ExplosionRange, ref failed));
        }
        else
        {
            failed = true;
        }
        #endregion

        #region Level3
        var level3 = towerData.Element(Constants.cst_Level3);
        if (level3 != null)
        {
            levels[2] = new Declarations.PlasmaLevelData(ReadInt(level3, Constants.cst_UpgradePrice, ref failed),
                                                        ReadFloat(level3, Constants.cst_Range, ref failed),
                                                        ReadFloat(level3, Constants.cst_FireRate, ref failed),
                                                        ReadInt(level3, Constants.cst_Damage, ref failed),
                                                        ReadFloat(level3, Constants.cst_ExplosionRange, ref failed));
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
        var level1 = towerData.Element(Constants.cst_Level1);

        if (level1 != null)
        {
            levels[0] = new Declarations.CrystalLevelData(ReadInt(level1, Constants.cst_Price, ref failed),
                                                        ReadFloat(level1, Constants.cst_Range, ref failed),
                                                        ReadFloat(level1, Constants.cst_FireRate, ref failed),
                                                        ReadInt(level1, Constants.cst_Damage, ref failed),
                                                        ReadInt(level1, Constants.cst_SlowEffect, ref failed),
                                                        ReadFloat(level1, Constants.cst_SlowDuration, ref failed));
        }
        else
        {
            failed = true;
        }
        #endregion

        #region Level2
        var level2 = towerData.Element(Constants.cst_Level2);
        if (level2 != null)
        {
            levels[1] = new Declarations.CrystalLevelData(ReadInt(level2, Constants.cst_UpgradePrice, ref failed),
                                                        ReadFloat(level2, Constants.cst_Range, ref failed),
                                                        ReadFloat(level2, Constants.cst_FireRate, ref failed),
                                                        ReadInt(level2, Constants.cst_Damage, ref failed),
                                                        ReadInt(level2, Constants.cst_SlowEffect, ref failed),
                                                        ReadFloat(level2, Constants.cst_SlowDuration, ref failed));
        }
        else
        {
            failed = true;
        }
        #endregion

        #region Level3
        var level3 = towerData.Element(Constants.cst_Level3);
        if (level3 != null)
        {
            levels[2] = new Declarations.CrystalLevelData(ReadInt(level3, Constants.cst_UpgradePrice, ref failed),
                                                        ReadFloat(level3, Constants.cst_Range, ref failed),
                                                        ReadFloat(level3, Constants.cst_FireRate, ref failed),
                                                        ReadInt(level3, Constants.cst_Damage, ref failed),
                                                        ReadInt(level3, Constants.cst_SlowEffect, ref failed),
                                                        ReadFloat(level3, Constants.cst_SlowDuration, ref failed));
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
        var level1 = towerData.Element(Constants.cst_Level1);

        if (level1 != null)
        {
            levels[0] = new Declarations.TeslaLevelData(ReadInt(level1, Constants.cst_Price, ref failed),
                                                        ReadFloat(level1, Constants.cst_Range, ref failed),
                                                        ReadFloat(level1, Constants.cst_FireRate, ref failed),
                                                        ReadInt(level1, Constants.cst_Damage, ref failed),
                                                        ReadInt(level1, Constants.cst_MaxBounces, ref failed),
                                                        ReadFloat(level1, Constants.cst_BounceRange, ref failed));
        }
        else
        {
            failed = true;
        }
        #endregion

        #region Level2
        var level2 = towerData.Element(Constants.cst_Level2);
        if (level2 != null)
        {
            levels[1] = new Declarations.TeslaLevelData(ReadInt(level2, Constants.cst_UpgradePrice, ref failed),
                                                        ReadFloat(level2, Constants.cst_Range, ref failed),
                                                        ReadFloat(level2, Constants.cst_FireRate, ref failed),
                                                        ReadInt(level2, Constants.cst_Damage, ref failed),
                                                        ReadInt(level2, Constants.cst_MaxBounces, ref failed),
                                                        ReadFloat(level2, Constants.cst_BounceRange, ref failed));
        }
        else
        {
            failed = true;
        }
        #endregion

        #region Level3
        var level3 = towerData.Element(Constants.cst_Level3);
        if (level3 != null)
        {
            levels[2] = new Declarations.TeslaLevelData(ReadInt(level3, Constants.cst_UpgradePrice, ref failed),
                                                        ReadFloat(level3, Constants.cst_Range, ref failed),
                                                        ReadFloat(level3, Constants.cst_FireRate, ref failed),
                                                        ReadInt(level3, Constants.cst_Damage, ref failed),
                                                        ReadInt(level3, Constants.cst_MaxBounces, ref failed),
                                                        ReadFloat(level3, Constants.cst_BounceRange, ref failed));
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
        var level1 = towerData.Element(Constants.cst_Level1);
        if (level1 != null)
        {
            levels[0] = new Declarations.DamageTowerLevelData(ReadInt(level1, Constants.cst_Price, ref failed),
                                                        ReadFloat(level1, Constants.cst_Range, ref failed),
                                                        ReadInt(level1, Constants.cst_Damage, ref failed));
        }
        else
        {
            failed = true;
        }
        #endregion

        #region Level2
        var level2 = towerData.Element(Constants.cst_Level2);
        if (level2 != null)
        {
            levels[1] = new Declarations.DamageTowerLevelData(ReadInt(level2, Constants.cst_UpgradePrice, ref failed),
                                                        ReadFloat(level2, Constants.cst_Range, ref failed),
                                                        ReadInt(level2, Constants.cst_Damage, ref failed));
        }
        else
        {
            failed = true;
        }
        #endregion

        #region Level3
        var level3 = towerData.Element(Constants.cst_Level3);
        if (level3 != null)
        {
            levels[2] = new Declarations.DamageTowerLevelData(ReadInt(level3, Constants.cst_UpgradePrice, ref failed),
                                                        ReadFloat(level3, Constants.cst_Range, ref failed),
                                                        ReadInt(level3, Constants.cst_Damage, ref failed));
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
        var level1 = towerData.Element(Constants.cst_Level1);
        if (level1 != null)
        {
            levels[0] = new Declarations.TowerLevelData(ReadInt(level1, Constants.cst_Price, ref failed),
                                                        ReadFloat(level1, Constants.cst_Range, ref failed));
        }
        else
        {
            failed = true;
        }
        #endregion

        #region Level2
        var level2 = towerData.Element(Constants.cst_Level2);
        if (level2 != null)
        {
            levels[1] = new Declarations.TowerLevelData(ReadInt(level2, Constants.cst_UpgradePrice, ref failed),
                                                        ReadFloat(level2, Constants.cst_Range, ref failed));
        }
        else
        {
            failed = true;
        }
        #endregion

        #region Level3
        var level3 = towerData.Element(Constants.cst_Level3);
        if (level3 != null)
        {
            levels[2] = new Declarations.TowerLevelData(ReadInt(level3, Constants.cst_UpgradePrice, ref failed),
                                                        ReadFloat(level3, Constants.cst_Range, ref failed));
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
            case Declarations.EnemyType.Boss:
                return ReadBossData(enemyData, assetData, out enemy);
            default:
                Debug.Log("Unknown enemy type");
                break;
        }
        enemy = null;
        return false;
    }

    private static bool ReadBossData(XElement enemyData, EnemyAssetData assetData, out Declarations.EnemyData enemy)
    {
        bool failed = false;

        int health = ReadInt(enemyData, Constants.cst_Health, ref failed);
        float speed = ReadFloat(enemyData, Constants.cst_Speed, ref failed);
        int damage = ReadInt(enemyData, Constants.cst_Damage, ref failed);
        int award = ReadInt(enemyData, Constants.cst_Award, ref failed);
        int golemHealth = ReadInt(enemyData, Constants.cst_GolemHealth, ref failed);
        float golemSpeed = ReadFloat(enemyData, Constants.cst_GolemSpeed, ref failed);
        if (failed)
        {
            enemy = null;
            return false;
        }
        else
        {
            enemy = new Declarations.BossData(assetData, health, speed, damage, award, golemHealth, golemSpeed);
            return true;
        }
    }

    private static bool ReadRogueData(XElement enemyData, EnemyAssetData assetData, out Declarations.EnemyData enemy)
    {
        bool failed = false;

        int health = ReadInt(enemyData, Constants.cst_Health, ref failed);
        float speed = ReadFloat(enemyData, Constants.cst_Speed, ref failed);
        float runSpeed = ReadFloat(enemyData, Constants.cst_RunSpeed, ref failed);
        int damage = ReadInt(enemyData, Constants.cst_Damage, ref failed);
        int award = ReadInt(enemyData, Constants.cst_Award, ref failed);

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

        int health = ReadInt(enemyData, Constants.cst_Health, ref failed);
        float speed = ReadFloat(enemyData, Constants.cst_Speed, ref failed);
        int damage = ReadInt(enemyData, Constants.cst_Damage, ref failed);
        int award = ReadInt(enemyData, Constants.cst_Award, ref failed);

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

    #region SettingsReading
    internal static bool ReadConfig(XElement config, out Declarations.Settings settings)
    {
        const int cameraMoveDefVal = 30;
        const int cameraZoomDefVal = 40;
        const int musicDefVal = -35;
        const int sfxDefVal = -35;
        const bool fullscreenDefVal = true;
        const int qualityDefVal = 5;
        const bool fastBuildDefVal = true;
        bool failed = false;
        if (config != null)
        {
            int cameraMoveSpeed;
            int cameraZoomSpeed;
            int musicLevel;
            int sfxLevel;
            bool fullscreen;
            int quality;
            Vector2 resolution = new Vector2(-1,-1);
            bool fastBuilding;
            var allSettings = config.Elements(Constants.cst_Setting).Where(x => x.Attribute(Constants.cst_Name) != null && !string.IsNullOrEmpty(x.Attribute(Constants.cst_Name).Value));

            cameraMoveSpeed = ReadInt(allSettings.FirstOrDefault(x => x.Attribute(Constants.cst_Name).Value == Constants.cst_CameraMoveSpeed), Constants.cst_Value, ref failed, cameraMoveDefVal);
            cameraZoomSpeed = ReadInt(allSettings.FirstOrDefault(x => x.Attribute(Constants.cst_Name).Value == Constants.cst_CameraZoomSpeed), Constants.cst_Value, ref failed, cameraZoomDefVal);
            musicLevel = ReadInt(allSettings.FirstOrDefault(x => x.Attribute(Constants.cst_Name).Value == Constants.cst_MusicLevel), Constants.cst_Value, ref failed, musicDefVal);
            if (musicLevel > -10 && musicLevel < -80)
            {
                musicLevel = musicDefVal;
            }
            sfxLevel = ReadInt(allSettings.FirstOrDefault(x => x.Attribute(Constants.cst_Name).Value == Constants.cst_SFXLevel), Constants.cst_Value, ref failed, musicDefVal);
            if (sfxLevel > -10 && sfxLevel < -80)
            {
                sfxLevel = musicDefVal;
            }

            fullscreen = ReadBool(allSettings.FirstOrDefault(x => x.Attribute(Constants.cst_Name).Value == Constants.cst_Fullscreen), Constants.cst_Value, ref failed, fullscreenDefVal);
            quality = ReadInt(allSettings.FirstOrDefault(x => x.Attribute(Constants.cst_Name).Value == Constants.cst_Quality), Constants.cst_Value, ref failed, qualityDefVal);
            if(quality < 0 || quality > 5)
            {
                quality = qualityDefVal;
            }
            var tempResolution = ReadString(allSettings.FirstOrDefault(x => x.Attribute(Constants.cst_Name).Value == Constants.cst_Resolution), Constants.cst_Value, ref failed, "");
            var splittedResolution = tempResolution.Split(new char[] { 'x' }, StringSplitOptions.RemoveEmptyEntries);
            bool validRes = false;
            if(splittedResolution.Length == 2)
            {
                int width;
                int height;
                if(int.TryParse(splittedResolution[0], out width) && int.TryParse(splittedResolution[1], out height))
                {
                    if(width >= 640 && height >= 400)
                    {
                        resolution = new Vector2(width, height);
                        validRes = true;
                        if(width != Screen.width || height != Screen.height)
                        {
                            failed = true;
                        }
                    }
                }
            }
            if (!validRes)
            {
                resolution = new Vector2(Screen.width, Screen.height);
                failed = true;
            }
            fastBuilding = ReadBool(allSettings.FirstOrDefault(x => x.Attribute(Constants.cst_Name).Value == Constants.cst_FastBuilding), Constants.cst_Value, ref failed, fastBuildDefVal); ;

            settings = new Declarations.Settings(fullscreen, resolution, quality, cameraMoveSpeed, cameraZoomSpeed, musicLevel, sfxLevel, fastBuilding);
        }
        else
        {
            settings = new Declarations.Settings(fullscreenDefVal, new Vector2(Screen.width, Screen.height), qualityDefVal, cameraMoveDefVal, cameraZoomDefVal, musicDefVal, sfxDefVal, fastBuildDefVal);
            failed = true;
        }
        return failed;
    }

    #endregion SettingsReading

    #region Helpers
    private static float ReadFloat(XElement element, string attributeName, ref bool failed, float defaultValue = -1.0f)
    {
        float result = defaultValue;
        if (element != null)
        {
            var attribute = element.Attribute(attributeName);
            if (!(attribute != null && !string.IsNullOrEmpty(attribute.Value) && float.TryParse(attribute.Value, out result)))
            {
                failed = true;
                return defaultValue;
            }
        }
        else
        {
            failed = true;
        }
        return result;
    }

    private static int ReadInt(XElement element, string attributeName, ref bool failed, int defaultValue = -1)
    {
        int result = defaultValue;
        if (element != null)
        {
            var attribute = element.Attribute(attributeName);
            if (!(attribute != null && !string.IsNullOrEmpty(attribute.Value) && int.TryParse(attribute.Value, out result)))
            {
                failed = true;
                return defaultValue;
            }
        }
        else
        {
            failed = true;
        }
        return result;
    }

    private static bool ReadBool(XElement element, string attributeName, ref bool failed, bool defaultValue = false)
    {
        bool result = defaultValue;
        if (element != null)
        {
            var attribute = element.Attribute(attributeName);
            if (!(attribute != null && !string.IsNullOrEmpty(attribute.Value) && bool.TryParse(attribute.Value, out result)))
            {
                failed = true;
                return defaultValue;
            }
        }
        else
        {
            failed = true;
        }
        return result;
    }

    private static string ReadString(XElement element, string attributeName, ref bool failed, string defaultValue = "")
    {
        string result = defaultValue;
        if (element != null)
        {
            var attribute = element.Attribute(attributeName);
            if (!(attribute != null && !string.IsNullOrEmpty(attribute.Value)))
            {
                failed = true;
                return defaultValue;
            }
            else
            {
                result = attribute.Value;
            }
        }
        else
        {
            failed = true;
        }
        return result;
    }
    #endregion Helpers
}
