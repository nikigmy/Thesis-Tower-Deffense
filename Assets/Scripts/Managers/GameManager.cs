using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    [Header("Setup")]
    public BuildManager BuildManager;
    public MapGenerator MapGenerator;
    public SpawnManager SpawnManager;
    public UIManager UIManager;
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
        DontDestroyOnLoad(this);
        instance = this;
        Def.Instance.LoadData(towersAssetData, enemyAssetData);
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void SceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        LooeadDependencies();

        MapGenerator.GenerateMap(CurrentLevel);
        SpawnManager.SpawnWaves(CurrentLevel.Waves);

        UIManager.InitUI();
        SpawnManager.LevelCompleted.AddListener(LevelCompleted);

        LevelLoaded.Invoke();
    }

    void LoadLevel(int index)
    {
        CurrentLevel = Def.Instance.Levels[index];
        Health = CurrentLevel.StartHealth;
        Money = CurrentLevel.StartMoney;

        Time.timeScale = 1;
        GameSpedUp = false;
        Paused = false;

        SceneManager.LoadScene(2, LoadSceneMode.Single);
    }

    private void LooeadDependencies()
    {
        BuildManager = FindObjectOfType<BuildManager>();
        MapGenerator = FindObjectOfType<MapGenerator>();
        SpawnManager = FindObjectOfType<SpawnManager>();
        UIManager = FindObjectOfType<UIManager>();
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
        Health -= value;
        if (Health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

    private void LevelCompleted()
    {
        UIManager.ShowVictoryScreen();
    }
}
