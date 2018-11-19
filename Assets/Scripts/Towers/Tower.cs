using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class Tower : MonoBehaviour
{
    protected Declarations.TowerData towerData;
    [SerializeField]
    protected Enemy target;

    [SerializeField]
    protected GameObject Level1Gun;
    [SerializeField]
    protected GameObject Level2Gun;
    [SerializeField]
    protected GameObject Level3Gun;

    protected GameObject currentGun;
    protected Transform currentGunBase;
    protected Transform currentGunHead;
    protected Transform currentFirePoint;

    public Declarations.TowerType Type
    {
        get
        {
            if(towerData != null)
            {
                return towerData.Type;
            }
            else
            {
                return Declarations.TowerType.Canon;
            }
        }
    }

    private void Start()
    {
        currentGun = Level1Gun;
        if (towerData.CurrentLevel != 1)
        {
            UpgradeTower();
        }
        else
        {
            UpdateGunPartsReferences();
        }
        towerData.Upgraded.AddListener(UpgradeTower);
    }
    
    protected virtual void UpdateGunPartsReferences()
    {
        var nextGunBase = currentGun.transform.GetChild(0);
        if (currentGunBase != null)
            nextGunBase.transform.localRotation = currentGunBase.localRotation;
        currentGunBase = nextGunBase;

        var nextGunHead = currentGunBase.GetChild(0);
        if (currentGunHead != null)
            nextGunHead.transform.localRotation = currentGunHead.transform.localRotation;
        currentGunHead = nextGunHead;
    }

    private void UpgradeTower()
    {
        switch (towerData.CurrentLevel)
        {
            case 2:
                currentGun.SetActive(false);
                Level2Gun.SetActive(true);
                currentGun = Level2Gun;
                break;
            case 3:
                currentGun.SetActive(false);
                Level3Gun.SetActive(true);
                currentGun = Level3Gun;
                break;
            default:
                Debug.Log("There are no more than 3 levels");
                break;
        }
        UpdateGunPartsReferences();
    }

    protected void FindTarget()
    {
        var allEnemies = GameManager.instance.SpawnManager.enemies;
        Enemy closestEnemy = null;
        var distanceToClosestEnemy = float.MaxValue;
        for (int i = 0; i < allEnemies.Count; i++)
        {
            var distanceToEnemy = Vector3.Distance(allEnemies[i].transform.position, transform.position);
            if (distanceToEnemy <= towerData.CurrentRange && distanceToEnemy < distanceToClosestEnemy)
            {
                closestEnemy = allEnemies[i];
                distanceToClosestEnemy = distanceToEnemy;
            }
        }
        if (closestEnemy != null)
        {
            target = closestEnemy;
        }
    }

    protected virtual void LookAtTarget()
    {
        if (target != null)
        {
            var baseDir = target.transform.position - currentGunBase.position;
            var baseLookRotation = Quaternion.LookRotation(baseDir);
            var baseRotation = Quaternion.Lerp(currentGunBase.rotation, baseLookRotation, Time.deltaTime * 4).eulerAngles;
            currentGunBase.localRotation = Quaternion.Euler(0, baseRotation.y, 0);

            var headDir = target.transform.position - currentGunHead.position;
            var headLookRotation = Quaternion.LookRotation(headDir);
            var headRotation = Quaternion.Lerp(currentGunHead.rotation, headLookRotation, Time.deltaTime * 4).eulerAngles;
            currentGunHead.localRotation = Quaternion.Euler(headRotation.x, 0, 0);
        }
    }

    protected bool CanShoot()
    {
        if (target != null)
        {
            var baseLookOffset = currentGunBase.rotation.eulerAngles.y - Quaternion.LookRotation(target.transform.position - currentGunBase.position).eulerAngles.y;
            var towerLookOffset = currentGunHead.rotation.eulerAngles.x - Quaternion.LookRotation(target.transform.position - currentGunHead.position).eulerAngles.x;
            return Math.Abs(baseLookOffset) + Math.Abs(towerLookOffset) <= 30;//good enough
        }
        else
        {
            return false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (towerData != null)
        {
            Gizmos.DrawWireSphere(transform.position, towerData.CurrentRange);
            if (target != null)
            {
                Gizmos.DrawLine(currentFirePoint.transform.position, target.transform.position);

            }
        }
    }

    public void Destroy()
    {
        GameManager.instance.AddMoney(towerData.CurrentPrice / 2);
        Destroy(gameObject);
    }
}
