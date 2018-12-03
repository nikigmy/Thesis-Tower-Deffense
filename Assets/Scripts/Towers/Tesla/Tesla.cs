using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tesla : Tower
{
    [SerializeField]
    private GameObject lightningBolt;
    private float timeFromPreviousShot;

    private void Awake()
    {
        towerData = Def.Instance.TowerDictionary[Declarations.TowerType.Tesla];
        timeFromPreviousShot = towerData.CurrentFireRate;
    }

    private void Update()
    {
        timeFromPreviousShot += Time.deltaTime;
        FindTarget();

        if (target != null)
        {
            if (timeFromPreviousShot >= towerData.CurrentFireRate)
            {
                Fire();
                timeFromPreviousShot = 0;
            }
        }
    }

    protected override void UpdateGunPartsReferences()
    {
        currentFirePoint = currentGun.transform.GetChild(0);
    }

    private void Fire()
    {
        var projectile = Instantiate(lightningBolt, currentFirePoint.position, currentFirePoint.rotation);
        projectile.GetComponent<Projectile>().SetTarget(new Declarations.LightningBoltData(target, currentFirePoint.gameObject, towerData.CurrentDamage, (towerData as Declarations.TeslaTower).CurrentMaxBounces, (towerData as Declarations.TeslaTower).CurrentBounceRange));
    }
}
