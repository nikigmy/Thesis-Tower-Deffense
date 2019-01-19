using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

public class Def
{
    private static Def instance;
    public static Def Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Def();
            }

            return instance;
        }
    }

    public Dictionary<Declarations.TowerType, Declarations.TowerData> TowerDictionary;
    public Dictionary<Declarations.EnemyType, Declarations.EnemyData> EnemyDictionary;

    public List<Declarations.LevelData> Levels;
    public Declarations.Settings Settings;

    public void LoadData(TowerAssetData[] towersAssetData, EnemyAssetData[] enemyAssetData)
    {
        TowerDictionary = new Dictionary<Declarations.TowerType, Declarations.TowerData>();
        EnemyDictionary = new Dictionary<Declarations.EnemyType, Declarations.EnemyData>();
        LoadAllData(towersAssetData, enemyAssetData);
    }

    private void LoadAllData(TowerAssetData[] towersAssetData, EnemyAssetData[] enemyAssetData)
    {
        var setupFile = LoadSetupFile(Application.platform != RuntimePlatform.WindowsEditor);
        LoadSetup(towersAssetData, enemyAssetData, setupFile);
        var levels = LoadLevelFiles(Application.platform != RuntimePlatform.WindowsEditor);
        LoadLevels(levels);
        if (DataReader.ReadConfig(LoadConfigFile(), out Settings))
        {
            Helpers.SaveAndSetSettings();
        }
    }

    internal void ResetTowerLevel()
    {
        foreach (var tower in TowerDictionary)
        {
            tower.Value.ResetLevel();
        }
    }

    private List<XElement> LoadLevelFiles(bool developerMode)
    {
        if (!developerMode)
        {
            return Resources.LoadAll<TextAsset>(Constants.cst_Levels).Select(x => XElement.Parse(x.text)).ToList();
        }
        else
        {
            var levelFiles = Directory.GetFiles(Path.Combine(Application.dataPath + "/../", Constants.cst_Levels)).Where(x => x.EndsWith(".xml"));
            if (levelFiles != null && levelFiles.Any())
            {
                return levelFiles.Select(x => XElement.Load(x)).ToList();
            }
            else
            {
                return new List<XElement>();
            }
        }
    }

    private XElement LoadSetupFile(bool developerMode)
    {
        if (!developerMode)
        {
            return XElement.Parse(Resources.Load<TextAsset>(Constants.cst_Setup).text);
        }
        else
        {
            var setupPath = Path.Combine(Application.dataPath + "/../", Constants.cst_Setup + Constants.cst_Xml);
            if (File.Exists(setupPath))
            {
                return XElement.Load(setupPath);
            }
            else
            {
                var result = XElement.Parse(Resources.Load<TextAsset>(Constants.cst_Setup).text);
                result.Save(setupPath);
                return result;
            }
        }
    }

    private XElement LoadConfigFile()
    {
        var configPath = Path.Combine(Application.dataPath + "/../", Constants.cst_Config + Constants.cst_Xml);
        if (File.Exists(configPath))
        {
            return XElement.Load(configPath);
        }
        else
        {
            return null;
        }
    }

    private void LoadSetup(TowerAssetData[] towersAssetData, EnemyAssetData[] enemyAssetData, XElement setup)
    {
        var towers = setup.Element(Constants.cst_TowerData).Elements().ToList();
        for (int i = 0; i < towers.Count; i++)
        {
            Declarations.TowerType towerType;
            var parseResult = Helpers.GetTowerTypeFromString(towers[i].Name.LocalName, out towerType);
            if (parseResult && !TowerDictionary.ContainsKey(towerType))
            {
                var assetData = towersAssetData.FirstOrDefault(x => x.Type == towerType);
                if (assetData != null)
                {
                    Declarations.TowerData tower;
                    if (DataReader.ReadTowerData(towers[i], assetData, towerType, out tower))
                    {
                        TowerDictionary.Add(towerType, tower);
                    }
                }
                else
                {
                    Debug.Log("No tower asset data for tower:" + towerType);
                }
            }
        }
        var enemies = setup.Element(Constants.cst_EnemyData).Elements().ToList();
        for (int i = 0; i < enemies.Count; i++)
        {
            Declarations.EnemyType enemyType;
            var parseResult = Helpers.GetEnemyTypeFromString(enemies[i].Name.LocalName, out enemyType);
            if (parseResult && !EnemyDictionary.ContainsKey(enemyType))
            {
                var assetData = enemyAssetData.FirstOrDefault(x => x.Type == enemyType);
                if (assetData != null)
                {
                    Declarations.EnemyData enemy;
                    if (DataReader.ReadEnemyData(enemies[i], assetData, enemyType, out enemy))
                    {
                        enemy.Type = enemyType;
                        EnemyDictionary.Add(enemyType, enemy);
                    }
                    else
                    {
                        Debug.Log(string.Format("Cant read enemy data for enemy type: '{0}'", enemyType.ToString()));
                    }
                }
                else
                {
                    Debug.Log(string.Format("No asset data for enemy type: '{0}'", enemyType.ToString()));
                }
            }
            else if (parseResult)
            {
                Debug.Log(string.Format("Duplicating data for enemy type: '{0}' data will be ignorred!", enemyType.ToString()));
            }
        }
    }

    public void LoadLevels(List<XElement> levels)
    {
        Levels = new List<Declarations.LevelData>();
        for (int i = 0; i < levels.Count; i++)
        {
            Declarations.LevelData level;
            if (DataReader.ReadLevelData(levels[i], out level))
            {
                Levels.Add(level);
            }
            else
            {
                Debug.Log("Invalid level file");
            }
        }
        bool atleastOneUnlocked = false;
        if (PlayerPrefs.HasKey(Constants.cst_LevelData))
        {
            var levelData = PlayerPrefs.GetString(Constants.cst_LevelData, "");
            if (!string.IsNullOrEmpty(levelData))
            {
                var unlockedLevels = levelData.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var unlockedLevel in unlockedLevels)
                {
                    var splittedData = unlockedLevel.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (splittedData.Any())
                    {
                        var level = Levels.FirstOrDefault(x => x.Name == splittedData[0]);
                        if (level != null)
                        {
                            atleastOneUnlocked = true;
                            level.Unlocked = true;
                            int stars;
                            if (int.TryParse(splittedData[1], out stars))
                            {
                                level.Stars = stars;
                            }
                        }
                    }
                }
            }
        }
        if(!atleastOneUnlocked)
        {
            if (levels.Count > 0)
            {
                Levels[0].Unlocked = true;
            }
        }
    }
}
