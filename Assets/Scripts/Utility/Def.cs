using System.Collections.Generic;
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
            if(instance == null)
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
        var setup = XElement.Parse(Resources.Load<TextAsset>(cst_setup).text);
        LoadSetup(towersAssetData, enemyAssetData, setup);

        var levels = Resources.LoadAll<TextAsset>(cst_levels);
        LoadLevels(levels);

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
                        EnemyDictionary.Add(enemyType, enemy);
                    }
                }
            }
        }
    }

    private void LoadLevels(TextAsset[] levels)
    {
        Levels = new Declarations.LevelData[levels.Length];
        for (int i = 0; i < levels.Length; i++)
        {
            Declarations.LevelData level;
            if (DataReader.ReadLevelData(XElement.Parse(levels[i].text), out level))
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
