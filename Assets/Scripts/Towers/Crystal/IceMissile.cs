using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceMissile : Projectile
{
    private bool move = false;
    private Enemy target;
    private float speed = 8;
    private int damage;
    private int slowEffect;
    private float slowDuration;


    public override void SetTarget(Declarations.IProjectileData projectileData)
    {
        var data = (Declarations.IceMissileData)projectileData;
        damage = data.Damage;
        target = data.Target;
        slowEffect = data.SlowEfect;
        slowDuration = data.SlowDuration;
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
                    target.DealDamage(damage, new Declarations.Effect(Declarations.EffectType.Slow, slowDuration, slowEffect));
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
