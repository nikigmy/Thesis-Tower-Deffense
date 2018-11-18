using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaBall : Projectile {
    private bool move = false;
    private Enemy target;
    private float speed = 15;//set in data later
    private float explosionRange;
    private int explosionDamage;

    public override void SetTarget(Declarations.IProjectileData projectileData)
    {
        var data = (Declarations.PlasmaBallData)projectileData;
        this.explosionDamage = data.ExprosionDamage;
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
                var dir = target.GetCenter() - transform.position;
                var currSpeed = speed * Time.deltaTime;

                if (dir.magnitude <= currSpeed)
                {
                    for (int i = 0; i < GameManager.instance.SpawnManager.enemies.Count; i++)
                    {
                        var enemy = GameManager.instance.SpawnManager.enemies[i];
                        var distance = Vector3.Distance(enemy.transform.position, target.transform.position);
                        if (distance < explosionRange)
                        {
                            enemy.DealDamage((int)Mathf.Round(explosionDamage * (explosionRange - distance) / explosionRange));
                        }
                    }
                    Destroy(gameObject);
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
}
