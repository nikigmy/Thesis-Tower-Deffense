using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBolt : Projectile
{

    public GameObject lightningPrefab;
    int bouncesLeft;
    Enemy currentTarget;
    float bouceRange;
    int damage;
    List<Enemy> enemiesHit;
    Vector3 lastPosition;


    public override void SetTarget(Declarations.IProjectileData projectileData)
    {
        enemiesHit = new List<Enemy>();
        var data = (Declarations.LightningBoltData)projectileData;
        bouncesLeft = data.MaxBounces;
        bouceRange = data.BounceRange;
        currentTarget = data.Target;
        damage = data.Damage;
        GenerateLightnings(data.StartPosition, data.Target);
        Invoke("FindNextTarget", 0.1f);
    }

    void FindNextTarget()
    {
        Enemy nextTarget = null;
        var distanceToClosestTarget = float.MaxValue;
        var currentPos = Vector3.zero;
        if(currentTarget != null)
        {
            currentPos = currentTarget.GetCenter();
        }
        else//target died
        {
            currentPos = lastPosition;
        }
        foreach (var enemy in GameManager.instance.SpawnManager.enemies)
        {
            if (enemy != currentTarget)
            {
                var targetDir = enemy.GetCenter() - currentPos;
                if (targetDir.magnitude < bouceRange && !enemiesHit.Contains(enemy) && targetDir.magnitude < distanceToClosestTarget)
                {
                    nextTarget = enemy;
                    distanceToClosestTarget = targetDir.magnitude;
                }
            }
        }
        if (nextTarget != null)
        {
            GenerateLightnings(currentTarget.gameObject, nextTarget);
            bouncesLeft--;
            if (bouncesLeft == 0)
            {
                Destroy(gameObject);
            }
            else
            {
                Invoke("FindNextTarget", 0.1f);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void GenerateLightnings(GameObject start, Enemy target)
    {
        var numberOfLightnings = Random.Range(5, 7);
        for (int i = 0; i < numberOfLightnings; i++)
        {
            var lightning = Instantiate(lightningPrefab).GetComponent<Lightning>();
            lightning.GenerateLightning(start, target);
        }
        enemiesHit.Add(target);
        currentTarget = target;
        lastPosition = target.GetCenter();
        target.DealDamage(damage);
    }
}
