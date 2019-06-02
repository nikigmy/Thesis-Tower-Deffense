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

    [SerializeField]
    CanvasGroup UpperPanelGroup;
    [SerializeField]
    CanvasGroup LowerPanelGroup;

    Declarations.LevelData currentlyLoadedLevel;
    [SerializeField]
    Sprite TextFieldNormal;
    [SerializeField]
    Sprite TextFieldError;

    Wave currectWave;
    List<Wave> loadedWaves;

    public void AddWave()
    {
        if (currentlyLoadedLevel != null)
        {
            var waveToAdd = new Declarations.WaveData(new List<Declarations.WavePart>());
            currentlyLoadedLevel.Waves.Add(waveToAdd);
            var index = WaveList.transform.childCount - 1;
            var waveObject = Instantiate(WavePrefab, WaveList.transform).GetComponent<Wave>();
            waveObject.SetData(index, waveToAdd);
            waveObject.transform.SetSiblingIndex(index);
            loadedWaves.Add(waveObject);
        }
    }

    public void AddEnemyPart()
    {
        var wavePart = new Declarations.SpawnWavePart(Def.Instance.EnemyDictionary[0]);
        var index = WavePartList.transform.childCount - 1;
        var wavePartObject = Instantiate(EnemyPartPrefab, WavePartList.transform).GetComponent<WavePart>();
        wavePartObject.SetData(index, currectWave, wavePart);
        wavePartObject.transform.SetSiblingIndex(index);
        currectWave.AddPart(wavePart, wavePartObject);
    }

    public void AddDelayPart()
    {
        var wavePart = new Declarations.DelayWavePart(1);
        var index = WavePartList.transform.childCount - 1;
        var wavePartObject = Instantiate(TimePartPrefab, WavePartList.transform).GetComponent<WavePart>();
        wavePartObject.SetData(index, currectWave, wavePart);
        wavePartObject.transform.SetSiblingIndex(index);
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
        for (int i = WavePartList.transform.childCount - 2; i >= 0; i--)
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
            wavePartObject.transform.SetSiblingIndex(i);
            wavePartObjects.Add(wavePartObject);
        }
        currectWave.SetParts(wavePartObjects);
    }

    public void LoadLevelClicked()
    {
        var filter = new SimpleFileBrowser.FileBrowser.Filter("xml", ".xml");
        SimpleFileBrowser.FileBrowser.SetFilters(false, filter);
        SimpleFileBrowser.FileBrowser.ShowLoadDialog(FileLoaded, Canceled, false, Application.dataPath);
        EnableDisablePanels(false);
    }

    private void Canceled()
    {
        EnableDisablePanels(true);
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
        EnableDisablePanels(true);
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
            SimpleFileBrowser.FileBrowser.ShowSaveDialog(SavePathChosen, Canceled, false, Application.dataPath);
            EnableDisablePanels(false);
        }
    }

    private void SavePathChosen(string path)
    {
        if (currentlyLoadedLevel != null)
        {
            currentlyLoadedLevel.Name = System.IO.Path.GetFileNameWithoutExtension(path);
            currentlyLoadedLevel.Export().Save(path);
        }
        EnableDisablePanels(true);
        Debug.Log(path);
    }

    public void CreateLevelClicked()
    {
        bool error = false;
        var invalidSymbols = new List<char> { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
        var name = newLevelPanel.transform.GetChild(2).GetChild(1).GetComponent<InputField>().text;
        int width;
        int height;
        int startMoney;
        int startHealth;
        if (string.IsNullOrEmpty(name) || name.Any(x => invalidSymbols.Contains(x)))
        {
            error = true;
            newLevelPanel.transform.GetChild(2).GetChild(1).GetComponent<Image>().sprite = TextFieldError;
        }
        else
        {
            newLevelPanel.transform.GetChild(2).GetChild(1).GetComponent<Image>().sprite = TextFieldNormal;
        }
        if (!int.TryParse(newLevelPanel.transform.GetChild(3).GetChild(1).GetComponent<InputField>().text, out startMoney) || startMoney <= 0)
        {
            error = true;
            newLevelPanel.transform.GetChild(3).GetChild(1).GetComponent<Image>().sprite = TextFieldError;
        }
        else
        {
            newLevelPanel.transform.GetChild(3).GetChild(1).GetComponent<Image>().sprite = TextFieldNormal;
        }
        if (!int.TryParse(newLevelPanel.transform.GetChild(4).GetChild(1).GetComponent<InputField>().text, out startHealth) || startHealth <= 0)
        {
            error = true;
            newLevelPanel.transform.GetChild(4).GetChild(1).GetComponent<Image>().sprite = TextFieldError;
        }
        else
        {
            newLevelPanel.transform.GetChild(4).GetChild(1).GetComponent<Image>().sprite = TextFieldNormal;
        }
        if (!int.TryParse(newLevelPanel.transform.GetChild(5).GetChild(1).GetComponent<InputField>().text, out width) || width < 10 || width > 50)
        {
            error = true;
            newLevelPanel.transform.GetChild(5).GetChild(1).GetComponent<Image>().sprite = TextFieldError;
        }
        else
        {
            newLevelPanel.transform.GetChild(5).GetChild(1).GetComponent<Image>().sprite = TextFieldNormal;
        }
        if (!int.TryParse(newLevelPanel.transform.GetChild(6).GetChild(1).GetComponent<InputField>().text, out height) || height < 10 || height > 50)
        {
            error = true;
            newLevelPanel.transform.GetChild(6).GetChild(1).GetComponent<Image>().sprite = TextFieldError;
        }
        else
        {
            newLevelPanel.transform.GetChild(6).GetChild(1).GetComponent<Image>().sprite = TextFieldNormal;
        }
        if (!error)
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
            EnableDisablePanels(true);
        }
    }

    public void MainMenuClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void EditWavesClicked()
    {
        loadedWaves = new List<Wave>();
        for (int i = WaveList.transform.childCount - 2; i >= 0; i--)
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
                waveObject.transform.SetSiblingIndex(i);
                loadedWaves.Add(waveObject);
            }
        }
        EnableDisablePanels(false);
    }

    public void EnableDisablePanels(bool enable)
    {
        UpperPanelGroup.interactable = enable;
        UpperPanelGroup.blocksRaycasts = enable;
        LowerPanelGroup.interactable = enable;
        LowerPanelGroup.blocksRaycasts = enable;
    }
}
