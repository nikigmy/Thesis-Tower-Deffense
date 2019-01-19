using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    [SerializeField]
    GameObject demonObject;
    [SerializeField]
    GameObject golemObject;

    Animator explosionAnim;

    bool transformed;

    private void Start()
    {
        anim = demonObject.GetComponent<Animator>();
        slowEffect = demonObject.transform.GetChild(0).GetComponent<ParticleSystem>();
        rend = demonObject.transform.GetChild(1).GetComponentInChildren<Renderer>();
        healthBar = demonObject.transform.GetChild(2).GetComponent<HealthBar>();
    }

    private void Awake()
    {
        enemyData = Def.Instance.EnemyDictionary[Declarations.EnemyType.Boss];
        explosionAnim = GetComponent<Animator>();
        Init();
        transformed = false;
    }

    protected override void Died()
    {
        if (!transformed)
        {
            Alive = false;
            slowEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            anim.speed = 0;
            explosionAnim.SetTrigger("Explode");
        }
        else
        {
            base.Died();
            anim.SetTrigger("Died");
        }
    }

    public void Transform()
    {
        demonObject.SetActive(false);
        golemObject.SetActive(true);

        anim = golemObject.GetComponent<Animator>();
        slowEffect = golemObject.transform.GetChild(0).GetComponent<ParticleSystem>();
        rend = golemObject.transform.GetChild(1).GetComponentInChildren<Renderer>();
        healthBar = golemObject.transform.GetChild(2).GetComponent<HealthBar>();

        currentHealth = (enemyData as Declarations.BossData).GolemHealth;
        Alive = true;
        anim.speed = 1;
        transformed = true;
    }

    protected override float GetCurrentNormalSpeed()
    {
        if (!transformed)
        {
            return enemyData.Speed;
        }
        else
        {
            return (enemyData as Declarations.BossData).GolemSpeed;
        }
    }

    protected override int GetCurrentMaxHealth()
    {
        if (!transformed)
        {
            return enemyData.Health;
        }
        else
        {
            return (enemyData as Declarations.BossData).GolemHealth;
        }
    }
}
