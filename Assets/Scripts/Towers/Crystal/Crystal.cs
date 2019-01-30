using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : Tower {
    
    [SerializeField]
    private GameObject iceMisile;
    private float timeFromPreviousShot;

    private void Awake()
    {
        TowerData = Def.Instance.TowerDictionary[Declarations.TowerType.Crystal];
        timeFromPreviousShot = (TowerData as Declarations.CrystalTower).CurrentFireRate;
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
            if (timeFromPreviousShot >= (TowerData as Declarations.CrystalTower).CurrentFireRate)
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
        projectile.GetComponent<Projectile>().SetTarget(new Declarations.IceMissileData(target, (TowerData as Declarations.CrystalTower).CurrentDamage, (TowerData as Declarations.CrystalTower).CurrentSlowEffect, (TowerData as Declarations.CrystalTower).CurrentSlowDuration));
    }
}
