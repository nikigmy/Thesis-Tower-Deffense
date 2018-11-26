using System;
using UnityEngine;
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

    public Declarations.LevelData CurrentLevel;

    public UnityEvent MoneyChanged;
    public UnityEvent HealthChanged;
    public UnityEvent LevelLoaded;
    public bool Paused = false;
    public bool GameSpedUp = false;

    public int Health;
    public int Money;

    private void Awake()
    {
        if (instance == null)
        {
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
        if (scene.name == "Level")
        {
            LooeadDependencies(scene.name);

            MapGenerator.GenerateMap(CurrentLevel);
            SpawnManager.SpawnWaves(CurrentLevel.Waves);

            UIManager.InitUI();
            SpawnManager.LevelCompleted.AddListener(LevelCompleted);

            LevelLoaded.Invoke();
            Def.Instance.ResetTowerLevel();
        }
        else if (scene.name == "MainMenu")
        {
            LooeadDependencies(scene.name);
            Def.Instance.LoadData(towersAssetData, enemyAssetData);

            MainMenu.LoadLevelPreviews();
        }
        else if (scene.name == "LevelCreator")
        {
            LooeadDependencies(scene.name);
        }
    }

    public void LoadLevel(int index)
    {
        CurrentLevel = Def.Instance.Levels[index];
        Health = CurrentLevel.StartHealth;
        Money = CurrentLevel.StartMoney;

        Time.timeScale = 1;
        GameSpedUp = false;
        Paused = false;

        SceneManager.LoadScene("Level", LoadSceneMode.Single);
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
        else if(sceneName == "MainMenu")
        {
            MainMenu = FindObjectOfType<MainMenu>();
        }
        else if(sceneName == "LevelCreator")
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

    public void DealDamage(int value)
    {
        Debug.Log("Dealed damage" + value);
        Health -= value;
        if (Health <= 0)
        {
            UIManager.ShowDefeatScreen();
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
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = GameSpedUp ? 2 : 1;
        }
    }

    public void MuteUnmuteMusic()
    {
        MusicMuted = !MusicMuted;
        if (MusicMuted)
        {

        }
        else
        {

        }
    }

    public void MuteUnmuteSound()
    {
        SoundMuted = !SoundMuted;
        if (SoundMuted)
        {

        }
        else
        {

        }
    }

    private void LevelCompleted()
    {
        UIManager.ShowVictoryScreen();
    }
}
