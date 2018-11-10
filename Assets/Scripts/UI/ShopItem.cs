using System;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour {

    [Header("Setup")]
    [SerializeField]
    private Image image;
    [SerializeField]
    private Text price;
    
    private Declarations.TowerData towerData;

    public void Setup(Declarations.TowerData towerData)
    {
        this.towerData = towerData;
        image.sprite = towerData.AssetData.Level1Sprite;
        price.text = towerData.CurrentPrice.ToString();
        UpdateTextColor();

        towerData.Upgraded.AddListener(UpdateMenu);
        var button = GetComponent<Button>();
        button.onClick.AddListener(delegate { GameManager.instance.ShopItemClicked(towerData); });
        GameManager.instance.MoneyChanged.AddListener(UpdateTextColor);
    }

    private void UpdateTextColor()
    {
        if(towerData.CurrentPrice <= GameManager.instance.Money)
        {
            price.color = Color.green;
        }
        else
        {
            price.color = Color.red;
        }
    }

    private void UpdateMenu()
    {
        switch (towerData.CurrentLevel)
        {
            case 2:
                image.sprite = towerData.AssetData.Level2Sprite;
                break;
            case 3:
                image.sprite = towerData.AssetData.Level3Sprite;
                break;
        }
        price.text = towerData.CurrentPrice.ToString();
        UpdateTextColor();
    }
}
