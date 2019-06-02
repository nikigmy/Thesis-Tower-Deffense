using System;
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
    [SerializeField]
    AudioClip chargeSound;
    private DateTime timeOfChargeStart;
    private float timeLeftFromCoolOff;
    private float timeLeftFromCharge;

    private ParticleSystem currentFx;

    private void Awake()
    {
        TowerData = Def.Instance.TowerDictionary[Declarations.TowerType.Plasma];
    }

    private void Update()
    {
        if (LostTarget())//change target
        {
            target = null;
        }
        if (target == null && charging)
        {
            CoolOff();
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
                        audioSource.Stop();
                        Fire();
                    }
                    CoolOff();
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
                audioSource.clip = chargeSound;
                audioSource.Play();
                charging = true;
                timeLeftFromCharge = (TowerData as Declarations.PlasmaTower).CurrentFireRate;
                currentFx.Play(true);
                var main = currentFx.main;
                main.simulationSpeed = 1;
            }
        }
        else
        {
            FindTarget(true);
        }
    }

    private void CoolOff()
    {
        currentFx.Stop();
        coolingOff = true;
        charging = false;
        audioSource.Stop();
        var main = currentFx.main;
        main.simulationSpeed = 2;
        timeLeftFromCoolOff = ((TowerData as Declarations.PlasmaTower).CurrentFireRate - timeLeftFromCharge) / 2;
    }

    private void Fire()
    {
        audioSource.Stop();
        var projectile = Instantiate(plasmaBall, currentFirePoint.position, currentFirePoint.rotation);
        projectile.GetComponent<Projectile>().SetTarget(new Declarations.PlasmaBallData(target, (TowerData as Declarations.PlasmaTower).CurrentDamage, (TowerData as Declarations.PlasmaTower).CurrentExplosionRange));
    }

    protected override void UpdateGunPartsReferences()
    {
        base.UpdateGunPartsReferences();

        var barrel = currentGunHead.GetChild(0);
        currentFirePoint = barrel.GetChild(0);

        currentFx = currentFirePoint.GetComponent<ParticleSystem>();

        var main = currentFx.main;
        main.duration = (TowerData as Declarations.PlasmaTower).CurrentFireRate;
    }
}
