using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class Tower : MonoBehaviour
{
    protected Declarations.TowerData towerData;
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

    private void UpdateGunPartsReferences()
    {
        currentGunBase = currentGun.transform.GetChild(0);
        currentGunHead = currentGunBase.GetChild(0);
        currentFirePoint = currentGunHead.GetChild(0);
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

    protected void LookAtTarget()
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
        var baseLookOffset = currentGunBase.rotation.eulerAngles.y - Quaternion.LookRotation(target.transform.position - currentGunBase.position).eulerAngles.y;
        var towerLookOffset = currentGunHead.rotation.eulerAngles.x - Quaternion.LookRotation(target.transform.position - currentGunHead.position).eulerAngles.x;
        return Math.Abs(baseLookOffset) + Math.Abs(towerLookOffset) <= 30;//good enough
    }

    private void OnDrawGizmos()
    {
        if (towerData != null)
        {
            Gizmos.DrawWireSphere(transform.position, towerData.CurrentRange);
        }
    }

    public void Destroy()
    {
        GameManager.instance.AddMoney(towerData.CurrentPrice / 2);
        Destroy(gameObject);
    }
}
