using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plasma : Tower
{

    [SerializeField]
    private GameObject plasmaBall;
    [SerializeField]
    private float chargeTime = 5;
    private bool charging = false;

    private ParticleSystem currentFx;

    private void Awake()
    {
        towerData = Def.Instance.TowerDictionary[Declarations.TowerType.Plasma];
    }

    private void Update()
    {
        if (target != null && (target.transform.position - transform.position).magnitude > towerData.CurrentRange)
        {
            charging = false;
            currentFx.Stop();
            target = null;
        }

        if (target != null)
        {
            LookAtTarget();
            if (CanShoot())
            {
                StartCoroutine(Charge());
            }
        }
        else
        {
            FindTarget();
        }
    }

    IEnumerator Charge()
    {
        charging = true;
        var main = currentFx.main;
        main.duration = chargeTime;
        currentFx.Simulate(0, true, true);
        yield return new WaitForSeconds(chargeTime);
        
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
        //var projectile = Instantiate(plasmaBall, currentFirePoint.position, currentFirePoint.rotation);
        //projectile.GetComponent<Projectile>().SetTarget(new Declarations.PlasmaBallData(target, (towerData as Declarations.plas).CurrentDamage));
    }

    protected override void UpdateGunPartsReferences()
    {
        base.UpdateGunPartsReferences();

        var barrel = currentGunHead.GetChild(0);
        currentFirePoint = barrel.GetChild(0);

        currentFx = currentFirePoint.GetComponent<ParticleSystem>();
    }
}
