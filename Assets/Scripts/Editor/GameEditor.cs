using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Xml.Linq;

public class GameEditor : EditorWindow
{
    int levelIndex = -1;
    public bool devMode = false;
    [MenuItem("Window/EditorTools")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        GameEditor window = (GameEditor)EditorWindow.GetWindow(typeof(GameEditor));
        window.devMode = bool.Parse(PlayerPrefs.GetString(Constants.cst_DeveloperMode, "false"));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("EditorTools", EditorStyles.boldLabel);
        var temp = EditorGUILayout.Toggle("Developer Mode", devMode);
        if (temp != devMode)
        {
            devMode = temp;
            PlayerPrefs.SetString(Constants.cst_DeveloperMode, devMode.ToString());
        }
        if (GUILayout.Button("Reset Level Data"))
        {
            PlayerPrefs.DeleteKey(Constants.cst_LevelData);
        }
        levelIndex = EditorGUILayout.IntField("Level index", levelIndex);

        if (GUILayout.Button("Generate Map"))
        {
            var mapGenerator = FindObjectOfType<MapGenerator>();
            if (mapGenerator == null)
            {
                Debug.Log("Cant find map generator");
                return;
            }
            var levels = Resources.LoadAll<TextAsset>("Levels");
            if (levels == null || levels.Count() == 0)
            {
                Debug.Log("Cant find or invalid levels file");
            }
            if (levelIndex >= 0 && levelIndex < levels.Count())
            {
                Declarations.LevelData levelData;
                DataReader.ReadLevelData(XElement.Parse(levels.ElementAt(levelIndex).text), out levelData, false, false);
                if (levelData != null)
                {
                    mapGenerator.GenerateTileMap();
                    mapGenerator.GenerateMap(levelData);
                }
                else
                {
                    Debug.Log("Cant parse level");
                }
            }
            else
            {
                Debug.Log("Invalid level index");
            }
        }

        if (GUILayout.Button("Clear Map"))
        {
            var mapGenerator = FindObjectOfType<MapGenerator>();
            if (mapGenerator != null)
            {
                mapGenerator.ClearMap();
            }
            else
            {
                Debug.Log("Cant find map generator");
                return;
            }
        }
    }
}
