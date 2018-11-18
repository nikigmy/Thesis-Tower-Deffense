using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plasma : Tower
{

    [SerializeField]
    private GameObject plasmaBall;
    [SerializeField]
    private bool charging = false;
    Coroutine currentCharge;

    private ParticleSystem currentFx;

    private void Awake()
    {
        towerData = Def.Instance.TowerDictionary[Declarations.TowerType.Plasma];
        //Invoke("StartCharge", 2);
    }

    //private void StartCharge()
    //{
    //    Debug.Log("Started");
    //    StartCoroutine(Charge());
    //}

    private void Update()
    {
        if (target != null && Vector3.Distance(target.transform.position, transform.position) > towerData.CurrentRange)
        {
            charging = false;
            if(currentCharge != null)
            {
                StopCoroutine(currentCharge);
                currentFx.Stop();
                target = null;
            }
        }

        if (target != null)
        {
            LookAtTarget();
            if (CanShoot() && !charging)
            {
                currentCharge = StartCoroutine(Charge());
            }
        }
        else
        {
            FindTarget();
        }
    }

    IEnumerator Charge()
    {
        Debug.Log("Plasma gun '" + gameObject.name + "' charging");
        charging = true;
        currentFx.Play(true);
        yield return new WaitForSeconds(towerData.CurrentFireRate);

        if (charging)
        {
            currentFx.Stop();
            if (CanShoot())
            {
                Fire();
            }
            else
            {
                Debug.Log("Plasma gun '" + gameObject.name + "' cant shoot at target");
            }
        }
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
