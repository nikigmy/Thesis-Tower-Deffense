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
    private Text towerDescription;

    [SerializeField]
    private Button upgradeButton;

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
        towerLevel.text = "Level: " + currentTower.CurrentLevel;
        towerDescription.text = currentTower.AssetData.Description;
        UpdateUpgradeButton();

        upgradeButton.onClick.AddListener(UpgradeTowerClicked);
    }

    private void UpgradeTowerClicked()
    {
        currentTower.Upgrade();
        towerLevel.text = "Level: " + currentTower.CurrentLevel;
        UpdateUpgradeButton();
    }

    void UpdateUpgradeButton()
    {
        var currentMoney = GameManager.instance.Money;

        if (currentTower.CurrentUpgradePrice == 0)
        {
            upgradeButton.image.color = Color.grey;
            upgradeButtonText.text = "Tower can't be upgraded further!";
            upgradeButton.enabled = false;
        }
        else
        {
            if (currentTower.CurrentUpgradePrice <= currentMoney)
            {
                upgradeButton.image.color = Color.green;
                upgradeButton.enabled = true;
            }
            else
            {
                upgradeButton.image.color = Color.red;
                upgradeButton.enabled = false;
            }
            upgradeButtonText.text = string.Format("Upgrade for {0}?", currentTower.CurrentUpgradePrice);
        }
    }
}
