using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : Tower {
    
    [SerializeField]
    private GameObject iceMisile;
    private float timeFromPreviousShot;

    private void Awake()
    {
        towerData = Def.Instance.TowerDictionary[Declarations.TowerType.Crystal];
        timeFromPreviousShot = (towerData as Declarations.CrystalTower).CurrentFireRate;
    }

    private void Update()
    {
        timeFromPreviousShot += Time.deltaTime;
        if (LostTarget())
        {
            target = null;
        }

        if (target != null)
        {
            if (timeFromPreviousShot >= (towerData as Declarations.CrystalTower).CurrentFireRate)
            {
                Fire();
                timeFromPreviousShot = 0;
            }
        }
        else
        {
            FindTarget();
        }
    }

    protected override void UpdateGunPartsReferences()
    {
        base.UpdateGunPartsReferences();

        currentFirePoint = currentGunHead.GetChild(0);
    }

    private void Fire()
    {
        var projectile = Instantiate(iceMisile, currentFirePoint.position, currentFirePoint.rotation);
        projectile.GetComponent<Projectile>().SetTarget(new Declarations.IceMissileData(target, (towerData as Declarations.CrystalTower).CurrentDamage, (towerData as Declarations.CrystalTower).CurrentSlowEffect, (towerData as Declarations.CrystalTower).CurrentSlowDuration));
    }
}
