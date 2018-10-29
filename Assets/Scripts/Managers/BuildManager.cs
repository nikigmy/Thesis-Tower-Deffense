using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour {
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

}
