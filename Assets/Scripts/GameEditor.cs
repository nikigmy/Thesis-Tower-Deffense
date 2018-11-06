using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Xml.Linq;

public class GameEditor : Editor
{

    [MenuItem("Tools/MapGenerator/GenerateMap")]

    private static void GenerateMap()
    {
        var mapGenerator = FindObjectOfType<MapGenerator>();
        var levels = Resources.LoadAll<TextAsset>("Levels");
        Declarations.LevelData levelData;
        DataReader.ReadLevelData(XElement.Parse(levels.ElementAt(0).text), out levelData, false);
        mapGenerator.GenerateTileMap();
        mapGenerator.GenerateMap(levelData);
    }

    [MenuItem("Tools/MapGenerator/ClearMap")]
    private static void ClearMap()
    {
        var mapGenerator = FindObjectOfType<MapGenerator>();
        mapGenerator.ClearMap();
    }
}
