using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    protected const float distanceFromFrontEnemy = 1.1f;
    protected const float rotationSpeed = 4;
    [SerializeField]
    protected HealthBar healthBar;
    [SerializeField]
    protected Renderer rend;
    [SerializeField]
    protected ParticleSystem slowEffect;

    protected Animator anim;
    protected Declarations.EnemyData enemyData;
    protected Tile currentTile;
    protected Tile previousTile;
    protected float currentHealth;
    public float CurrentSpeed;
    public bool Alive = true;
    public bool Moving = true;
    public bool Attacking = false;
    public bool Visible = true;
    public Declarations.EnemyType Type { get { return enemyData.Type; } }
    public List<Declarations.Effect> Effects;

    protected void Init()
    {
        Effects = new List<Declarations.Effect>();
        currentHealth = enemyData.Health;
        Visible = true;
    }

    public void SetData(Tile startTile)
    {
        currentTile = startTile;
        FindNextTile();
        Rotate(true);
    }

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public Vector3 GetCenter()
    {
        return rend.bounds.center;
    }

    public void AddEffect(Declarations.Effect effect)
    {
        Effects.Add(effect);
    }

    protected void UpdateUI()
    {
        if (!healthBar.gameObject.activeSelf)
        {
            healthBar.gameObject.SetActive(true);
        }
        healthBar.SetBar(currentHealth / enemyData.Health);

        healthBar.transform.LookAt(Camera.main.transform);
    }

    private void Update()
    {
        UpdateEnemy();
    }

    protected void UpdateEnemy()
    {
        UpdateUI();
        if (Alive)
        {
            if ((currentTile.transform.position - transform.position).magnitude <= 0.3)
            {
                FindNextTile();
            }
            if (currentTile != null)
            {
                if (currentTile.Type == Declarations.TileType.Objective)
                {
                    Attack();
                    return;
                }
                if (Moving)
                {
                    Rotate();
                    UpdateSpeed();
                    Move();
                }
            }
            UpdateEffects();
        }
    }

    private void UpdateEffects()
    {
        for (int i = 0; i < Effects.Count; i++)
        {
            var curreEffect = Effects[i];
            curreEffect.Duration -= Time.deltaTime;
            if (curreEffect.Duration <= 0)
            {
                RemoveEffect(curreEffect);
            }
        }
    }

    private void RemoveEffect(Declarations.Effect curreEffect)
    {
        Effects.Remove(curreEffect);
        if (curreEffect.Type == Declarations.EffectType.Slow && Effects.All(x => x.Type != Declarations.EffectType.Slow))
        {
            slowEffect.Stop();
            slowEffect.Clear();
        }
    }

    protected virtual void Attack()
    {
        GameManager.instance.DealDamage(enemyData.Damage);
        GameManager.instance.SpawnManager.EnemyDestroyed(this);
        Destroy(gameObject);
    }

    protected virtual void Move()
    {
        var dir = currentTile.transform.position - transform.position;
        if (dir.magnitude < Time.deltaTime * CurrentSpeed)
        {
            transform.position = currentTile.transform.position;
        }
        else
        {
            transform.Translate(dir.normalized * Time.deltaTime * CurrentSpeed, Space.World);
        }
    }

    protected virtual void Rotate(bool instant = false)
    {
        var dir = currentTile.transform.position - transform.position;
        var baseLookRotation = Quaternion.LookRotation(dir);
        if (instant)
        {
            transform.rotation = Quaternion.Euler(0, baseLookRotation.eulerAngles.y, 0);
        }
        else
        {
            var baseRotation = Quaternion.Lerp(transform.rotation, baseLookRotation, Time.deltaTime * rotationSpeed).eulerAngles;
            transform.rotation = Quaternion.Euler(0, baseRotation.y, 0);
        }
    }

    protected virtual void UpdateSpeed()
    {
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
                    if (CurrentSpeed < enemyData.Speed)
                    {
                        CurrentSpeed = enemyData.Speed;
                    }
                }
            }
            else
            {
                if (CurrentSpeed < enemyData.Speed)
                {
                    CurrentSpeed = enemyData.Speed;
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
            anim.speed = CurrentSpeed / enemyData.Speed;//keep the leg movement consistent
        }
    }

    internal virtual void DealDamage(float damage, Declarations.Effect effect = null)
    {
        if (Alive)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Died();
                return;
            }
            if (effect != null)
            {
                Effects.Add(effect);
                if (effect.Type == Declarations.EffectType.Slow)
                {
                    slowEffect.Play();
                }
            }
            UpdateUI();
        }
    }

    private void FindNextTile()
    {
        var neibourTiles = GameManager.instance.MapGenerator.GetNeibourCells(currentTile.Row, currentTile.Col);
        var objectiveTile = neibourTiles.FirstOrDefault(x => x.Type == Declarations.TileType.Objective);
        if (objectiveTile != null)
        {
            currentTile = objectiveTile;
        }
        else
        {
            var pathTiles = neibourTiles.Where(x =>
             {
                 if (x.Type == Declarations.TileType.Path)
                 {
                     if (previousTile != null)
                     {
                         if (x.Row == previousTile.Row && x.Col == previousTile.Col)
                         {
                             return false;
                         }
                         else
                         {
                             return true;
                         }
                     }
                     else
                     {
                         return true;
                     }
                 }
                 return false;
             }).ToList();

            if (pathTiles.Count > 0)
            {
                var nextTileIndex = UnityEngine.Random.Range(0, pathTiles.Count);
                previousTile = currentTile;
                currentTile = pathTiles[nextTileIndex];
            }
            else//dead end
            {
                var temp = currentTile;
                currentTile = previousTile;
                previousTile = temp;
            }
        }
    }

    protected virtual void Died()
    {
        Alive = false;
        slowEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        anim.speed = 1;
        GameManager.instance.SpawnManager.EnemyDestroyed(this);
        GameManager.instance.AddMoney(enemyData.Award);
    }
}
