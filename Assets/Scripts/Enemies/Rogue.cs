using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rogue : Enemy
{
    private void Awake()
    {
        enemyData = Def.Instance.EnemyDictionary[Declarations.EnemyType.Rogue];
        Init();
        Visible = false;
    }

    private void Update()
    {
        UpdateVisibility();
        UpdateEnemy();
    }

    private void UpdateVisibility()
    {
        var radarTowers = GameManager.instance.BuildManager.GetBuiltTowersOfType(Declarations.TowerType.Radar);
        bool visible = false;
        var currRadarRange = Def.Instance.TowerDictionary[Declarations.TowerType.Radar].CurrentRange;
        for (int i = 0; i < radarTowers.Length; i++)
        {
            if (Vector3.Distance(radarTowers[i].transform.position, GetCenter()) <= currRadarRange)
            {
                visible = true;
                break;
            }
        }
        if(visible != Visible)
        {
            Visible = visible;
            anim.SetBool("Running", visible);
        }
    }

    protected override void UpdateSpeed()
    {
        var normalSpeed = 0.0f;
        if (Visible)
        {
            normalSpeed = (enemyData as Declarations.RogueData).RunSpeed;
        }
        else
        {
            normalSpeed = enemyData.Speed;
        }

        if (Effects.Any(x => x.Type == Declarations.EffectType.Stun))
        {
            CurrentSpeed = 0;
        }
        else
        {
            var enemyInFront = GameManager.instance.SpawnManager.GetEnemyInFront(this);
            if (enemyInFront != null)
            {
                if (Vector3.Distance(transform.position, enemyInFront.transform.position) <= distanceFromFrontEnemy)
                {
                    if (enemyInFront.CurrentSpeed < CurrentSpeed)
                    {
                        CurrentSpeed = enemyInFront.CurrentSpeed;
                    }
                }
                else
                {
                    if (CurrentSpeed != normalSpeed)
                    {
                        CurrentSpeed = normalSpeed;
                    }
                }
            }
            else
            {
                if (CurrentSpeed != normalSpeed)
                {
                    CurrentSpeed = normalSpeed;
                }
            }
            var slow = Effects.FirstOrDefault(x => x.Type == Declarations.EffectType.Slow);
            var speedUp = Effects.FirstOrDefault(x => x.Type == Declarations.EffectType.Speed);
            if (speedUp != null)
            {
                CurrentSpeed *= speedUp.Value;
            }
            else if (slow != null)
            {
                CurrentSpeed -= CurrentSpeed * (slow.Value / 100);
            }

        }
        if (Moving)
        {
            anim.speed = CurrentSpeed / normalSpeed;//keep the leg movement consistent
        }
    }

    protected override void Died()
    {
        base.Died();
        anim.SetTrigger("Died");
    }

    public void DieAnimationEnded()
    {
        anim.speed = 0;
        Destroy(gameObject);
    }
}
