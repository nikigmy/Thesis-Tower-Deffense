using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaBall : Projectile {
    private bool move = false;
    private Enemy target;
    private float speed = 15;//set in data later
    private float explosionRange;
    private int explosionDamage;
    [SerializeField]
    private GameObject explosionPrefab;

    public override void SetTarget(Declarations.IProjectileData projectileData)
    {
        var data = (Declarations.PlasmaBallData)projectileData;
        this.explosionDamage = data.ExplosionDamage;
        this.target = data.Target;
        this.explosionRange = data.ExplosionRange;
        move = true;
    }

    private void Update()
    {
        if (move)
        {
            if (target != null)
            {
                var targetCenter = target.GetCenter();
                var dir = targetCenter - transform.position;
                var currSpeed = speed * Time.deltaTime;

                if (dir.magnitude <= currSpeed)
                {
                    Explode(target.transform.position, targetCenter);
                }
                else
                {
                    transform.Translate(dir.normalized * currSpeed, Space.World);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private void Explode(Vector3 targetPosition, Vector3 targetCenter)
    {
        Instantiate(explosionPrefab, targetPosition, Quaternion.identity);
        //explosion.SetRadius(explosionRange);PseudoVolumetricExplosion explosion =  .GetComponent<PseudoVolumetricExplosion>()

        for (int i = 0; i < GameManager.instance.SpawnManager.enemies.Count; i++)
        {
            var enemy = GameManager.instance.SpawnManager.enemies[i];
            if (enemy != null)
            {
                var distance = Vector3.Distance(enemy.GetCenter(), targetCenter);
                if (distance < explosionRange)
                {
                    enemy.DealDamage((int)Mathf.Round(explosionDamage * (explosionRange - distance) / explosionRange));
                }
            }
            if(enemy == null)
            {
                i--;
            }
        }
        Destroy(gameObject);
    }
}
