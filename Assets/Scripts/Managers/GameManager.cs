using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance;
    [Header("Setup")]
    public BuildManager BuildManager;
    public MapGenerator MapGenerator;
    public SpawnManager SpawnManager;
    [SerializeField]
    private TowerAssetData[] towersAssetData;
    [SerializeField]
    private EnemyAssetData[] enemyAssetData;
    [Header("UI")]
    [SerializeField]
    private Shop shop;
    [SerializeField]
    private InformationPanel informationPanel;
    [SerializeField]
    private UnityEngine.UI.Text moneyText;
    [SerializeField]
    private UnityEngine.UI.Text healthText;

    public Declarations.LevelData CurrentLevel;
    public int Health;
    public int Money;
    public UnityEvent MoneyChanged;

    private void Awake()
    {
        instance = this;
        Def.Instance.LoadData(towersAssetData, enemyAssetData);
        CurrentLevel = Def.Instance.Levels[0];
        Health = CurrentLevel.StartHealth;
        Money = CurrentLevel.StartMoney;
        moneyText.text = "Gold:" + Money;
        healthText.text = "Health:" + Health;
    }

    // Use this for initialization
    void Start () {
        MapGenerator.GenerateMap(CurrentLevel);
        shop.GenerateShop();
        SpawnManager.SpawnWaves(CurrentLevel.Waves);
    }

    public void SubstractMoney(int value)
    {
        Money -= value;
        MoneyChanged.Invoke();
        moneyText.text = "Gold:" + Money;
    }
    public void AddMoney(int value)
    {
        Money += value;
        MoneyChanged.Invoke();
        moneyText.text = "Gold:" + Money;
    }

    public void ShopItemClicked(Declarations.TowerData towerData)
    {
        informationPanel.gameObject.SetActive(true);
        informationPanel.LoadTowerData(towerData);
        BuildManager.CurrentTower = towerData;
        BuildManager.SellClicked = false;
    }

    public void SellItemClicked()
    {
        informationPanel.gameObject.SetActive(false);
        BuildManager.CurrentTower = null;
        BuildManager.SellClicked = true;
    }

    public void DealDamage(int value)
    {
        Health -= value;
        if(Health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            healthText.text = "Health:" + Health;
        }
    }
}
