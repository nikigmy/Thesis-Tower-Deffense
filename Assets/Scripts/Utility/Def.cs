﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

public class Def
{
    private const string cst_levels = "Levels";
    private const string cst_towerData = "TowerData";
    private const string cst_enemyData = "EnemyData";
    private const string cst_setup = "Setup";

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

    public Declarations.LevelData[] Levels;
    public float CameraMoveSpeed = 30;//read this
    public float CameraZoomSpeed = 40;//read this
    public float MaxCameraHeight = 60;//read this

    public void LoadData(TowerAssetData[] towersAssetData, EnemyAssetData[] enemyAssetData)
    {
        TowerDictionary = new Dictionary<Declarations.TowerType, Declarations.TowerData>();
        EnemyDictionary = new Dictionary<Declarations.EnemyType, Declarations.EnemyData>();
        LoadAllData(towersAssetData, enemyAssetData);
    }

    private void LoadAllData(TowerAssetData[] towersAssetData, EnemyAssetData[] enemyAssetData)
    {
        var setupFile = LoadSetupFile(Application.platform == RuntimePlatform.WindowsEditor);
        LoadSetup(towersAssetData, enemyAssetData, setupFile);
        var levels = LoadLevelFiles(Application.platform == RuntimePlatform.WindowsEditor);
        LoadLevels(levels);
    }

    internal void ResetTowerLevel()
    {
        foreach (var tower in TowerDictionary)
        {
            tower.Value.ResetLevel();
        }
    }

    private List<XElement> LoadLevelFiles(bool fromResources)
    {
        if (fromResources)
        {
            return Resources.LoadAll<TextAsset>(cst_levels).Select(x => XElement.Parse(x.text)).ToList();
        }
        else
        {
            return Directory.GetFiles(Path.Combine(Application.dataPath + "/../", "Levels")).Where(x => x.EndsWith(".xml")).Select(x => XElement.Load(x)).ToList();
        }
    }

    private XElement LoadSetupFile(bool fromResources)
    {
        if (fromResources)
        {
            return XElement.Parse(Resources.Load<TextAsset>(cst_setup).text);
        }
        else
        {
            return XElement.Load(Path.Combine(Application.dataPath + "/../", "Setup.xml"));
        }
    }

    private void LoadSetup(TowerAssetData[] towersAssetData, EnemyAssetData[] enemyAssetData, XElement setup)
    {
        var towers = setup.Element(cst_towerData).Elements().ToList();
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
        var enemies = setup.Element(cst_enemyData).Elements().ToList();
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
        Levels = new Declarations.LevelData[levels.Count];
        for (int i = 0; i < levels.Count; i++)
        {
            Declarations.LevelData level;
            if (DataReader.ReadLevelData(levels[i], out level))
            {
                Levels[i] = level;
            }
            else
            {
                Debug.Log("Invalid level file");
            }
        }
    }
}
