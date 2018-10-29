using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonBall : MonoBehaviour {

    private bool move = false;
    private Enemy target;
    private float speed = 10;//set in data later
    private int damage;

    public void SetTarget(Enemy target, int damage)
    {
        this.damage = damage;
        this.target = target;
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
