using System;
using UnityEngine;
using UnityEngine.UI;

public class InformationPanel : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField]
    private Text towerName;

    [SerializeField]
    private Text towerLevel;

    [SerializeField]
    private Image image;

    [SerializeField]
    private GameObject[] statObjects;

    [SerializeField]
    private Button upgradeButton;

    [SerializeField]
    private Sprite greenSprite;
    [SerializeField]
    private Sprite redSprite;

    [SerializeField]
    private Text upgradeButtonText;

    private Declarations.TowerData currentTower;

    private void Awake()
    {
        GameManager.instance.MoneyChanged.AddListener(UpdateUpgradeButton);
    }

    public void LoadTowerData(Declarations.TowerData towerData)
    {
        currentTower = towerData;
        towerName.text = currentTower.Type.ToString();
        UpdateInfoPanel();

        upgradeButton.onClick.AddListener(UpgradeTowerClicked);
    }

    private void UpgradeTowerClicked()
    {
        currentTower.Upgrade();

        UpdateInfoPanel();
    }

    void UpdateInfoPanel()
    {
        towerLevel.text = "Lvl. " + currentTower.CurrentLevel;
        image.sprite = currentTower.CurrentSprite;
        
        var index = 0;
        foreach (var stat in currentTower.GetStatDictionary())
        {
            statObjects[index].transform.GetChild(0).GetComponent<Text>().text = stat.Key;
            statObjects[index].transform.GetChild(1).GetComponent<Text>().text = stat.Value;
            index++;
        }
        for (; index < 5; index++)
        {
            statObjects[index].transform.GetChild(0).GetComponent<Text>().text = "";
            statObjects[index].transform.GetChild(1).GetComponent<Text>().text = "";
        }

        UpdateUpgradeButton();
    }

    private void UpdateUpgradeButton()
    {
        var currentMoney = GameManager.instance.Money;

        if (currentTower.CurrentUpgradePrice == 0)
        {
            upgradeButton.image.sprite = redSprite;
            upgradeButtonText.text = "Max Level";
            upgradeButton.enabled = false;
        }
        else
        {
            if (currentTower.CurrentUpgradePrice <= currentMoney)
            {
                upgradeButton.image.sprite = greenSprite;
                SetUpgradeActive(true);
            }
            else
            {
                upgradeButton.image.sprite = redSprite;
                SetUpgradeActive(false);
            }
            //upgradeButtonText.text = string.Format("Upgrade for:\n{0}?", currentTower.CurrentUpgradePrice);
            upgradeButtonText.text = currentTower.CurrentUpgradePrice.ToString();
        }
    }

    public void SetUpgradeActive(bool active)
    {
        if (active && currentTower.CurrentUpgradePrice <= GameManager.instance.Money)
        {
            upgradeButton.enabled = true;
        }
        else
        {
            upgradeButton.enabled = false;
        }
    }
}
