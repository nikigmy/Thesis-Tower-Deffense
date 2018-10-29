using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canon : Tower {

    [SerializeField]
    private GameObject cannonBall;
    private float timeFromPreviousShot;

    private void Awake()
    {
        towerData = Def.Instance.TowerDictionary[Declarations.TowerType.Canon];
        timeFromPreviousShot = 0;
    }

    private void Update()
    {
        timeFromPreviousShot += Time.deltaTime;
        if(target != null && (target.transform.position - transform.position).magnitude > towerData.CurrentRange)
        {
            target = null;
        }

        if(target != null)
        {
            LookAtTarget();
            if (CanShoot() && timeFromPreviousShot >= (towerData as Declarations.CanonTower).CurrentFireRate)
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

    private void Fire()
    {
        var projectile = Instantiate(cannonBall, currentFirePoint.position, currentFirePoint.rotation);
        projectile.GetComponent<CanonBall>().SetTarget(target, (towerData as Declarations.CanonTower).CurrentDamage);
        Debug.Log("Tower" + name + "shot");
    }
}
