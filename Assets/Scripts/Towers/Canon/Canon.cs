using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canon : Tower
{
    [SerializeField]
    private GameObject cannonBall;
    private float timeFromPreviousShot;

    private void Awake()
    {
        TowerData = Def.Instance.TowerDictionary[Declarations.TowerType.Canon];
        timeFromPreviousShot = (TowerData as Declarations.FiringTowerData).CurrentFireRate;
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
            LookAtTarget();
            if (CanShoot() && timeFromPreviousShot >= (TowerData as Declarations.FiringTowerData).CurrentFireRate)
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
        var projectile = Instantiate(cannonBall, currentFirePoint.position, currentFirePoint.rotation);
        projectile.GetComponent<Projectile>().SetTarget(new Declarations.CanonBallData(target, (TowerData as Declarations.CanonTower).CurrentDamage));
    }
}
