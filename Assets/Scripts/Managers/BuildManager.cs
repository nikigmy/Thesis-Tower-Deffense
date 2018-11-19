using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour {
    public List<Tower> CurrentTowers = new List<Tower>();
    public Declarations.TowerData CurrentTower;
    public bool SellClicked;
    public bool Building = false;
    public bool Selling = false;

    public void Update()
    {
        if((Building || Selling) && Input.GetMouseButtonUp(0))
        {
            Building = false;
            Selling = false;
        }
    }

    public void DestroyTower(Tower tower)
    {
        CurrentTowers.Remove(tower);
        tower.Destroy();
    }

    public Tower BuildTower(Tile tile)
    {
        if (CurrentTower != null)
        {
            var createdTower = Instantiate(CurrentTower.AssetData.Prefab, tile.transform.position, Quaternion.identity, tile.transform).GetComponent<Tower>();
            CurrentTowers.Add(createdTower);
            GameManager.instance.SubstractMoney(CurrentTower.CurrentPrice);
            return createdTower;
        }
        else
        {
            Debug.Log("No selected tower");
        }
        return null;
    }

    public int GetBuiltTowersCount(Declarations.TowerType towerType)
    {
        int count = 0;
        foreach (var tower in CurrentTowers)
        {
            if(tower.Type == towerType)
            {
                count++;
            }
        }
        return count;
    }
}
