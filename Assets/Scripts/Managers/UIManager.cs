using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] DevModeObjects;
    [SerializeField]
    GameObject[] NonDevModeObjects;

    [SerializeField]
    private SettingsMenu SettingsPanel;
    [SerializeField]
    private CanvasGroup UpperPanelGroup;
    [SerializeField]
    private CanvasGroup ShopPanelGroup;

    [SerializeField]
    private GameObject DefeatPanel;
    [SerializeField]
    private GameObject VictoryPanel;
    [SerializeField]
    private Sprite FullStarSprite;
    [SerializeField]
    private Sprite EmptyStarSprite;

    [SerializeField]
    private Shop shop;
    [SerializeField]
    private InformationPanel informationPanel;
    [SerializeField]
    private GameObject confirmationPanel;
    [SerializeField]
    private UnityEngine.UI.Text moneyText;
    [SerializeField]
    private UnityEngine.UI.Text healthText;

    private bool isConfirmingRestart;
    public void InitUI()
    {
        pauseImage = pauseButtonImage.sprite;

        moneyText.text = GameManager.instance.Money.ToString();
        healthText.text = GameManager.instance.Health.ToString();

        shop.GenerateShop();
        GameManager.instance.MoneyChanged.AddListener(UpdateMoneyText);
        GameManager.instance.HealthChanged.AddListener(UpdateHealthText);

        SetObjectActivity();
    }

    private void SetObjectActivity()
    {
        bool isInDevMode = GameManager.instance.DeveloperMode;
        foreach (var obj in DevModeObjects)
        {
            obj.SetActive(isInDevMode);
        }
        foreach (var obj in NonDevModeObjects)
        {
            obj.SetActive(!isInDevMode);
        }
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

    internal void ShowVictoryScreen(int stars, bool hasNextLevel)
    {
        Debug.Log(stars);
        var starParent = VictoryPanel.transform.GetChild(1);
        for (int i = 0; i < 3; i++)
        {
            var star = starParent.GetChild(i).GetComponent<Image>();
            if(i + 1 <= stars)
            {
                star.color = Color.white;
                star.sprite = FullStarSprite;
            }
            else
            {
                star.color = new Color(51, 46, 39);
                star.sprite = EmptyStarSprite;
            }
        }
        VictoryPanel.SetActive(true);
        SetMenusActive(false);
    }

    public void RestartLevel(bool withConfirmation)
    {
        if (withConfirmation)
        {
            isConfirmingRestart = true;
            confirmationPanel.transform.GetChild(0).GetComponent<Text>().text = "Do you want to restart the current level?";
            confirmationPanel.SetActive(true);
            wasPaused = GameManager.instance.Paused;
            if (!GameManager.instance.Paused)
            {
                PauseClicked();
            }
            SetMenusActive(false);
        }
        else
        {
            GameManager.instance.LoadLevel(GameManager.instance.CurrentLevel);
        }
    }

    public void LoadNextLevel()
    {
        GameManager.instance.LoadNextLevel();
    }

    public void AddMoney()
    {
        GameManager.instance.AddMoney(10000);
    }

    public void AddHealth()
    {
        GameManager.instance.AddHealth(10);
    }
    #region InGameMenus

    private bool wasPaused;

    internal void ShowDefeatScreen()
    {
        DefeatPanel.SetActive(true);
        SetMenusActive(false);
    }

    public void HomeSelected(bool withConfirmation)
    {
        if (withConfirmation)
        {
            isConfirmingRestart = false;
            confirmationPanel.transform.GetChild(0).GetComponent<Text>().text = "Do you want to quit to main menu?";
            confirmationPanel.SetActive(true);
            wasPaused = GameManager.instance.Paused;
            if (!GameManager.instance.Paused)
            {
                PauseClicked();
            }
            SetMenusActive(false);
        }
        else
        {
            GameManager.instance.LoadMainMenu();
        }
    }

    public void ConfirmationCanceled()
    {
        if (!wasPaused)
        {
            PauseClicked();
        }
        SetMenusActive(true);
    }

    public void ConfirmationCompleted()
    {
        if (isConfirmingRestart)
        {
            RestartLevel(false);
        }
        else
        {
            HomeSelected(false);
        }
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
        wasPaused = GameManager.instance.Paused;
        if (!GameManager.instance.Paused)
        {
            PauseClicked();
        }
        SettingsPanel.LoadSettings();
        SetMenusActive(false);
    }

    public void SettingsClosed()
    {
        if (!wasPaused)
        {
            PauseClicked();
        }
        SetMenusActive(true);
    }
    
    private void SetMenusActive(bool activate)
    {
        UpperPanelGroup.interactable = activate;
        UpperPanelGroup.blocksRaycasts = activate;
        ShopPanelGroup.interactable = activate;
        ShopPanelGroup.interactable = activate;
        informationPanel.SetUpgradeActive(false);
    }
    #endregion
}
