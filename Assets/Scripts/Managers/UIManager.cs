using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    [Header("UI")]
    [SerializeField]
    private Shop shop;
    [SerializeField]
    private InformationPanel informationPanel;
    [SerializeField]
    private UnityEngine.UI.Text moneyText;
    [SerializeField]
    private UnityEngine.UI.Text healthText;
    
    public void InitUI()
    {
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
        moneyText.text = GameManager.instance.Health.ToString();
    }

    public void ShopItemClicked(Declarations.TowerData towerData)
    {
        informationPanel.gameObject.SetActive(true);
        informationPanel.LoadTowerData(towerData);
        GameManager.instance.BuildManager.CurrentTower = towerData;
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
        throw new NotImplementedException();
    }

    public void AddCheatMoney()
    {
        GameManager.instance.AddMoney(10000);
    }
}
