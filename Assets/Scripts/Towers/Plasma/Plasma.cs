﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plasma : Tower
{

    [SerializeField]
    private GameObject plasmaBall;
    [SerializeField]
    private bool charging = false;
    [SerializeField]
    private bool coolingOff = false;
    private DateTime timeOfChargeStart;
    private float timeLeftFromCoolOff;
    private float timeLeftFromCharge;

    private ParticleSystem currentFx;

    private void Awake()
    {
        towerData = Def.Instance.TowerDictionary[Declarations.TowerType.Plasma];
    }

    private void Update()
    {
        if (target == null || (target != null && Vector3.Distance(target.transform.position, transform.position) > towerData.CurrentRange))//change target
        {
            target = null;
            if (charging)
            {
                CoolOff();
            }
        }

        if (coolingOff)
        {
            timeLeftFromCoolOff -= Time.deltaTime;
            if (timeLeftFromCoolOff <= 0)
            {
                coolingOff = false;
            }
        }
        if (charging && !coolingOff)
        {
            if (target != null)
            {
                timeLeftFromCharge -= Time.deltaTime;
                if (timeLeftFromCharge <= 0)
                {
                    if (CanShoot())
                    {
                        currentFx.Clear();
                        currentFx.Stop();
                        charging = false;
                        Fire();
                    }
                    else
                    {
                        CoolOff();
                    }
                }
            }
            else
            {
                CoolOff();
            }
        }

        if (target != null)
        {
            LookAtTarget();
            if (CanShoot() && !charging && !coolingOff)
            {
                charging = true;
                timeLeftFromCharge = towerData.CurrentFireRate;
                currentFx.Play(true);
                var main = currentFx.main;
                main.simulationSpeed = 1;
            }
        }
        else
        {
            FindTarget();
        }
    }

    private void CoolOff()
    {
        currentFx.Stop();
        coolingOff = true;
        charging = false;
        var main = currentFx.main;
        main.simulationSpeed = 2;
        timeLeftFromCoolOff = (towerData.CurrentFireRate - timeLeftFromCharge) / 2;
    }

    private void Fire()
    {
        var projectile = Instantiate(plasmaBall, currentFirePoint.position, currentFirePoint.rotation);
        projectile.GetComponent<Projectile>().SetTarget(new Declarations.PlasmaBallData(target, (towerData as Declarations.PlasmaTower).CurrentDamage, (towerData as Declarations.PlasmaTower).CurrentExplosionRange));
    }

    protected override void UpdateGunPartsReferences()
    {
        base.UpdateGunPartsReferences();

        var barrel = currentGunHead.GetChild(0);
        currentFirePoint = barrel.GetChild(0);

        currentFx = currentFirePoint.GetComponent<ParticleSystem>();

        var main = currentFx.main;
        main.duration = towerData.CurrentFireRate;
    }
}
