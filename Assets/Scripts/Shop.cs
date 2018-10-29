using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour {

    [SerializeField]
    private GameObject shopItemPrefab;

    public void GenerateShop()
    {
        var towers = Def.Instance.TowerDictionary.OrderBy(x => (int)x.Key).ToList();

        for (int i = 0; i < towers.Count; i++)
        {
            var shopItem = Instantiate(shopItemPrefab, transform);
            shopItem.GetComponent<ShopItem>().Setup(towers[i].Value);
        }

    }
}
