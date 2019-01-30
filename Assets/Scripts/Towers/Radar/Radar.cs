using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : Tower
{
    private void Awake()
    {
        TowerData = Def.Instance.TowerDictionary[Declarations.TowerType.Radar];
    }

    protected override void UpdateGunPartsReferences()
    {
        //no references needed
    }
}
