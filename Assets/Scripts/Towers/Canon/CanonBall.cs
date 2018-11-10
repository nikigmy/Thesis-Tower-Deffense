using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonBall : Projectile {

    private bool move = false;
    private Enemy target;
    private float speed = 15;//set in data later
    private int damage;

    public override void SetTarget(Declarations.IProjectileData projectileData)
    {
        var data = (Declarations.CanonBallData)projectileData;
        this.damage = data.Damage;
        this.target = data.Target;
        move = true;
    }

    private void Update()
    {
        if(move)
        {
            if(target != null)
            {
                var dir = target.GetCenter() - transform.position;
                var currSpeed = speed * Time.deltaTime;

                if(dir.magnitude <= currSpeed)
                {
                    target.DealDamage(damage);
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
