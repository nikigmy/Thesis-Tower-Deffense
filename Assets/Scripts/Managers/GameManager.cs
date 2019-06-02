using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool MainMenuLoaded = false;
    public bool MusicMuted;
    public bool SoundMuted;
    public MainMenu MainMenu;
    public static GameManager instance;
    [Header("Setup")]
    public BuildManager BuildManager;
    public MapGenerator MapGenerator;
    public SpawnManager SpawnManager;
    public UIManager UIManager;
    public PaintManager PaintManager;
    public LevelCreator LevelCreator;
    [SerializeField]
    private TowerAssetData[] towersAssetData;
    [SerializeField]
    private EnemyAssetData[] enemyAssetData;
    [SerializeField]
    public AudioMixer AudioMixer;

    public Declarations.LevelData CurrentLevel;

    public UnityEvent MoneyChanged;
    public UnityEvent HealthChanged;
    public UnityEvent LevelLoaded;
    public bool Paused = false;
    public bool GameSpedUp = false;

    public int Health;
    private int AddedHealth;
    public int Money;
    public bool DeveloperMode;
    float timeOfStart;
    public List<int> stepsMade;

    private List<AudioSource> pausedAudios;

    private void Awake()
    {
        if (instance == null)
        {
            stepsMade = new List<int>();
            DontDestroyOnLoad(this);
            instance = this;
            SceneManager.sceneLoaded += SceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        bool.TryParse(PlayerPrefs.GetString(Constants.cst_DeveloperMode, "false"), out DeveloperMode);
        if (scene.name == "Level")
        {
            Def.Instance.ResetTowerLevel();
            timeOfStart = Time.realtimeSinceStartup;
            LooeadDependencies(scene.name);

            MapGenerator.GenerateMap(CurrentLevel);
            SpawnManager.SpawnWaves(CurrentLevel.Waves);

            UIManager.InitUI();
            SpawnManager.LevelCompleted.AddListener(LevelCompleted);

            LevelLoaded.Invoke();
            AddedHealth = 0;
        }
        else if (scene.name == "MainMenu")
        {
            LooeadDependencies(scene.name);
            Def.Instance.LoadData(towersAssetData, enemyAssetData);
            MainMenu.SetButtonActivity();
        }
        else if (scene.name == "LevelCreator")
        {
            LooeadDependencies(scene.name);
        }
    }

    internal void SetDevMode(bool isSet)
    {
        DeveloperMode = isSet;
        PlayerPrefs.SetString(Constants.cst_DeveloperMode, isSet.ToString());
    }

    public void LoadLevel(int index)
    {
        LoadLevel(Def.Instance.Levels[index]);
    }

    public void LoadLevel(Declarations.LevelData level)
    {
        CurrentLevel = level;
        Health = level.StartHealth;
        Money = level.StartMoney;

        Time.timeScale = 1;
        GameSpedUp = false;
        Paused = false;

        SceneManager.LoadScene("Level", LoadSceneMode.Single);
    }

    public void LoadNextLevel()
    {
        var nextLevelIndex = Def.Instance.Levels.IndexOf(CurrentLevel) + 1;
        if(nextLevelIndex < Def.Instance.Levels.Count)
        {
            LoadLevel(nextLevelIndex);
        }
    }
    
    private void LooeadDependencies(string sceneName)
    {
        if (sceneName == "Level")
        {
            BuildManager = FindObjectOfType<BuildManager>();
            MapGenerator = FindObjectOfType<MapGenerator>();
            SpawnManager = FindObjectOfType<SpawnManager>();
            UIManager = FindObjectOfType<UIManager>();
        }
        else if (sceneName == "MainMenu")
        {
            MainMenu = FindObjectOfType<MainMenu>();
        }
        else if (sceneName == "LevelCreator")
        {
            MapGenerator = FindObjectOfType<MapGenerator>();
            PaintManager = FindObjectOfType<PaintManager>();
            LevelCreator = FindObjectOfType<LevelCreator>();
        }
    }

    public void SubstractMoney(int value)
    {
        Money -= value;
        MoneyChanged.Invoke();
    }

    public void AddMoney(int value)
    {
        Money += value;
        MoneyChanged.Invoke();
    }

    public void AddHealth(int value)
    {
        AddedHealth += value;
        Health += value;
        HealthChanged.Invoke();
    }

    public void DealDamage(int value)
    {
        Health -= value;
        if (Health <= 0)
        {
            Health = 0;
            HealthChanged.Invoke();
            UIManager.ShowDefeatScreen();

            var totalSeconds = Time.realtimeSinceStartup - timeOfStart;
            int minutes = (int)totalSeconds / 60;
            int hours = minutes / 60;
            Debug.Log("Hours: " + hours + " Minutes: " + minutes % 60 + " Seconds: " + (int)totalSeconds % 60 + " Total Seconds: " + totalSeconds);
            Debug.Log("Average Steps: " + stepsMade.Average());
            Debug.Log("Total Steps: " + stepsMade.Sum());
        }
        else
        {
            HealthChanged.Invoke();
        }
    }

    public void SpeedUpGame()
    {
        GameSpedUp = !GameSpedUp;
        if (!Paused)
        {
            Time.timeScale = GameSpedUp ? 2 : 1;
        }
    }

    public void PausePlayGame()
    {
        Paused = !Paused;
        if (Paused)
        {
            pausedAudios = FindObjectsOfType<AudioSource>().Where(x => x.outputAudioMixerGroup.name == "SFX").ToList();
            foreach (var audioToPause in pausedAudios)
            {
                audioToPause.Pause();
            }
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = GameSpedUp ? 2 : 1;
            foreach (var audioToPause in pausedAudios.Where(x => x != null))
            {
                audioToPause.Play();
            }
        }
    }
    
    private void LevelCompleted()
    {
        var currentLevelIndex = Def.Instance.Levels.IndexOf(CurrentLevel);
        var isLastLevel = currentLevelIndex == Def.Instance.Levels.Count - 1;
        var stars = CalculateStars();
        if (CurrentLevel.Unlocked)
        {
            bool shouldUpdateSave = false;
            if (stars > CurrentLevel.Stars)
            {
                CurrentLevel.Stars = stars;
                shouldUpdateSave = true;
            }
            if (isLastLevel)
            {
                SetDevMode(true);
            }
            else if (!Def.Instance.Levels[currentLevelIndex + 1].Unlocked)
            {
                Def.Instance.Levels[currentLevelIndex + 1].Unlocked = true;
                shouldUpdateSave = true;
            }
            if (shouldUpdateSave)
            {
                UpdateSave();
            }
        }
        UIManager.ShowVictoryScreen(stars, !isLastLevel);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void UpdateSave()
    {
        StringBuilder result = new StringBuilder();
        foreach (var level in Def.Instance.Levels)
        {
            if (level.Unlocked)
            {
                result.Append(level.Name + ":" + level.Stars + "\\");
            }
        }
        PlayerPrefs.SetString(Constants.cst_LevelData, result.ToString());
    }

    private int CalculateStars()
    {
        var health = Health - AddedHealth;
        if (health == CurrentLevel.StartHealth)
        {
            return 3;
        }
        else if (health >= (CurrentLevel.StartHealth / 3) * 2)
        {
            return 2;
        }
        else if (health >= CurrentLevel.StartHealth / 3)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}
