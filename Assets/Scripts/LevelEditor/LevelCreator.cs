using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelCreator : MonoBehaviour
{
    [SerializeField]
    GameObject WaveEditor;
    [SerializeField]
    GameObject WaveEditPanel;
    [SerializeField]
    GameObject WaveList;
    [SerializeField]
    GameObject WavePartEditPanel;
    [SerializeField]
    GameObject WavePartList;

    [SerializeField]
    GameObject WavePrefab;

    [SerializeField]
    GameObject EnemyPartPrefab;
    [SerializeField]
    GameObject TimePartPrefab;

    [SerializeField]
    GameObject newLevelPanel;

    Declarations.LevelData currentlyLoadedLevel;

    Wave currectWave;
    List<Wave> loadedWaves;

    public void AddWave()
    {
        var waveToAdd = new Declarations.WaveData(new List<Declarations.WavePart>());
        currentlyLoadedLevel.Waves.Add(waveToAdd);
        var index = WaveList.transform.childCount;
        var waveObject = Instantiate(WavePrefab, WaveList.transform).GetComponent<Wave>();
        waveObject.SetData(index, waveToAdd);
        loadedWaves.Add(waveObject);
    }

    public void AddEnemyPart()
    {
        var wavePart = new Declarations.SpawnWavePart(Def.Instance.EnemyDictionary[0]);
        var index = WavePartList.transform.childCount;
        var wavePartObject = Instantiate(EnemyPartPrefab, WavePartList.transform).GetComponent<WavePart>();
        wavePartObject.SetData(index, currectWave, wavePart);
        currectWave.AddPart(wavePart, wavePartObject);
    }

    public void AddDelayPart()
    {
        var wavePart = new Declarations.DelayWavePart(0);
        var index = WavePartList.transform.childCount;
        var wavePartObject = Instantiate(TimePartPrefab, WavePartList.transform).GetComponent<WavePart>();
        wavePartObject.SetData(index, currectWave, wavePart);
        currectWave.AddPart(wavePart, wavePartObject);
    }

    public void DeleteWave(int index)
    {
        currentlyLoadedLevel.Waves.RemoveAt(index);
        loadedWaves.RemoveAt(index);
        for (int i = index; i < loadedWaves.Count; i++)
        {
            loadedWaves[i].UpdateIndex(i);
        }
    }

    internal void InitWaveEdit(Wave wave, List<Declarations.WavePart> waveParts)
    {
        var wavePartObjects = new List<WavePart>();
        currectWave = wave;
        for (int i = WavePartList.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(WavePartList.transform.GetChild(i).gameObject);
        }
        WavePartEditPanel.SetActive(true);
        WaveEditPanel.SetActive(false);
        for (int i = 0; i < waveParts.Count; i++)
        {
            WavePart wavePartObject;
            if (waveParts[i].Type == Declarations.WavePartType.Spawn)
            {
                wavePartObject = Instantiate(EnemyPartPrefab, WavePartList.transform).GetComponent<WavePart>();
            }
            else
            {
                wavePartObject = Instantiate(TimePartPrefab, WavePartList.transform).GetComponent<WavePart>();
            }
            wavePartObject.SetData(i, wave, waveParts[i]);
            wavePartObjects.Add(wavePartObject);
        }
        currectWave.SetParts(wavePartObjects);
    }

    public void LoadLevelClicked()
    {
        var filter = new SimpleFileBrowser.FileBrowser.Filter("xml", ".xml");
        SimpleFileBrowser.FileBrowser.SetFilters(false, filter);
        SimpleFileBrowser.FileBrowser.ShowLoadDialog(FileLoaded, null, false, Application.dataPath);
    }

    private void FileLoaded(string path)
    {
        var levelFile = XElement.Load(path);
        if (levelFile != null)
        {
            Declarations.LevelData levelData;
            DataReader.ReadLevelData(levelFile, out levelData, true);
            if (levelData != null)
            {
                currentlyLoadedLevel = levelData;
                LoadLevel();
            }
        }
        else
        {
            Debug.Log("Cant load level!");
        }
    }

    private void LoadLevel()
    {
        if (currentlyLoadedLevel != null)
        {
            GameManager.instance.MapGenerator.LeftRightBorderSize = 0;
            GameManager.instance.MapGenerator.TopBorderSize = 0;
            GameManager.instance.MapGenerator.BotBorderSize = 0;
            GameManager.instance.MapGenerator.GenerateMapForEdit(currentlyLoadedLevel.MapSize, currentlyLoadedLevel.Map);
            GameManager.instance.LevelLoaded.Invoke();
        }
    }

    public void SaveLevelClicked()
    {
        if (currentlyLoadedLevel != null)
        {
            currentlyLoadedLevel.Map = GameManager.instance.MapGenerator.GetMap();
            var filter = new SimpleFileBrowser.FileBrowser.Filter("xml", ".xml");
            SimpleFileBrowser.FileBrowser.SetFilters(false, filter);
            SimpleFileBrowser.FileBrowser.ShowSaveDialog(SavePathChosen, null, false, Application.dataPath);
        }
    }

    private void SavePathChosen(string path)
    {
        if (currentlyLoadedLevel != null)
        {
            currentlyLoadedLevel.Name = System.IO.Path.GetFileNameWithoutExtension(path);
            currentlyLoadedLevel.Export().Save(path);
        }
        Debug.Log(path);
    }

    public void CreateLevelClicked()
    {
        var invalidSymbols = new List<char> { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
        //var name = newLevelPanel.transform.GetChild(1).GetComponent<InputField>().text;
        //int width;
        //int height;
        //int startMoney;
        //int startHealth;
        //if (int.TryParse(newLevelPanel.transform.GetChild(2).GetComponent<InputField>().text, out width) &&
        //    int.TryParse(newLevelPanel.transform.GetChild(3).GetComponent<InputField>().text, out height) &&
        //    int.TryParse(newLevelPanel.transform.GetChild(4).GetComponent<InputField>().text, out startMoney) &&
        //    int.TryParse(newLevelPanel.transform.GetChild(5).GetComponent<InputField>().text, out startHealth) &&
        //    width > 0 && height > 0 && startMoney > 0 && startHealth > 0
        //    && !string.IsNullOrEmpty(name) && !name.Any(x => invalidSymbols.Contains(x)))
        //{
        var name = "CreatedLevel";
        int width;
        int height;
        int startMoney = 500;
        int startHealth = 10;
        if (int.TryParse(newLevelPanel.transform.GetChild(1).GetComponent<InputField>().text, out width) &&
            int.TryParse(newLevelPanel.transform.GetChild(2).GetComponent<InputField>().text, out height) &&
            width > 0 && height > 0 && startMoney > 0 && startHealth > 0
            && !string.IsNullOrEmpty(name) && !name.Any(x => invalidSymbols.Contains(x)))
        {
            var map = new Declarations.TileType[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    map[i, j] = Declarations.TileType.Grass;
                }
            }
            currentlyLoadedLevel = new Declarations.LevelData(name, new Declarations.IntVector2(width, height), map, new List<Declarations.WaveData>(), startMoney, startHealth);
            LoadLevel();
            newLevelPanel.SetActive(false);
        }
    }

    public void MainMenuClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void EditWavesClicked()
    {
        loadedWaves = new List<Wave>();
        for (int i = WaveList.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(WaveList.transform.GetChild(i).gameObject);
        }
        WaveEditor.SetActive(true);
        if (currentlyLoadedLevel != null)
        {
            for (int i = 0; i < currentlyLoadedLevel.Waves.Count; i++)
            {
                var waveObject = Instantiate(WavePrefab, WaveList.transform).GetComponent<Wave>();
                waveObject.SetData(i, currentlyLoadedLevel.Waves[i]);
                loadedWaves.Add(waveObject);
            }
        }
    }
}
