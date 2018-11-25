using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class UIManager : MonoBehaviour {

    [Header("UI")]
    [SerializeField]
    private GameObject inGameMenuPanel;
    [SerializeField]
    private Shop shop;
    [SerializeField]
    private InformationPanel informationPanel;
    [SerializeField]
    private UnityEngine.UI.Text moneyText;
    [SerializeField]
    private UnityEngine.UI.Text healthText;
    [SerializeField]
    private GameObject victoryText;
    [SerializeField]
    private GameObject defeatText;

    public void InitUI()
    {
        musicSprite = musicImage.sprite;
        soundSprite = soundImage.sprite;
        pauseImage = pauseButtonImage.sprite;

        moneyText.text = GameManager.instance.Money.ToString();
        healthText.text = GameManager.instance.Health.ToString();
        
        shop.GenerateShop();
        GameManager.instance.MoneyChanged.AddListener(UpdateMoneyText);
        GameManager.instance.HealthChanged.AddListener(UpdateHealthText);
    }

    public void UpdateMoneyText()
    {
        moneyText.text = GameManager.instance.Money.ToString();
    }

    public void UpdateHealthText()
    {
        healthText.text = GameManager.instance.Health.ToString();
    }

    public void ShopItemClicked(Declarations.TowerData towerData)
    {
        informationPanel.gameObject.SetActive(true);
        informationPanel.LoadTowerData(towerData);
        GameManager.instance.BuildManager.CurrentTower = towerData;
        GameManager.instance.BuildManager.Selling = false;
        GameManager.instance.BuildManager.SellClicked = false;
    }

    public void SellItemClicked()
    {
        informationPanel.gameObject.SetActive(false);
        GameManager.instance.BuildManager.CurrentTower = null;
        GameManager.instance.BuildManager.SellClicked = true;
    }

    internal void ShowVictoryScreen()
    {
        victoryText.SetActive(true);
        Invoke("LoadMainMenu", 3);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void AddCheatMoney()
    {
        GameManager.instance.AddMoney(10000);
    }
    #region InGameMenu
    [Header("In-Game Menu")]

    [SerializeField]
    private Image musicImage;
    [SerializeField]
    private Image soundImage;

    [SerializeField]
    private Sprite muteMusicSprite;
    [SerializeField]
    private Sprite muteSoundSprite;

    private Sprite musicSprite;
    private Sprite soundSprite;

    private bool wasPaused;

    public bool isInGameMenuActiveActive;

    public void MusicClicked()
    {
        GameManager.instance.MuteUnmuteMusic();
        if (GameManager.instance.MusicMuted)
        {
            musicImage.sprite = muteMusicSprite;
        }
        else
        {
            musicImage.sprite = musicSprite;
        }
    }

    public void SoundClicked()
    {
        GameManager.instance.MuteUnmuteSound();
        if (GameManager.instance.SoundMuted)
        {
            soundImage.sprite = muteSoundSprite;
        }
        else
        {
            soundImage.sprite = soundSprite;
        }
    }

    internal void ShowDefeatScreen()
    {
        defeatText.SetActive(true);
        Invoke("LoadMainMenu", 3);
    }

    public void DeactivateInGameMenu()
    {
        isInGameMenuActiveActive = false;
        if (!wasPaused)
        {
            PauseClicked();
        }

        inGameMenuPanel.SetActive(false);
    }
    #endregion

    #region Control Panel
    [Header("Control Panel")]

    [SerializeField]
    private Image pauseButtonImage;
    [SerializeField]
    private Sprite playImage;
    private Sprite pauseImage;

    [SerializeField]
    private Image speedUpButtonImage;
    
    public void PauseClicked()
    {
        if (isInGameMenuActiveActive)
        {
            return;
        }
        GameManager.instance.PausePlayGame();
        if (GameManager.instance.Paused)
        {
            pauseButtonImage.sprite = playImage;
        }
        else
        {
            pauseButtonImage.sprite = pauseImage;
        }
    }

    public void SpeedUpClicked()
    {
        if (isInGameMenuActiveActive)
        {
            return;
        }
        GameManager.instance.SpeedUpGame();
        if (GameManager.instance.GameSpedUp)
        {
            speedUpButtonImage.color = Color.green;
        }
        else
        {
            speedUpButtonImage.color = Color.white;
        }
    }

    public void SettingsClicked()
    {
        if (!isInGameMenuActiveActive)
        {
            wasPaused = GameManager.instance.Paused;
            if (!GameManager.instance.Paused)
            {
                PauseClicked();
            }
            isInGameMenuActiveActive = true;
            inGameMenuPanel.SetActive(true);
        }
        else
        {
            DeactivateInGameMenu();
        }
    }
    #endregion
}
