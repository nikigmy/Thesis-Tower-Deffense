using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private Transform healthBar;
    private Slider healthSlider;
    private Text healthText;
    private Renderer capsuleRenderer;

    private Declarations.EnemyData enemyData;
    private Tile currentTile;
    private Tile previousTile;
    private int currentHealth;
    public float CurrentSpeed;
    private const float distanceFromFrontEnemy = 1.1f;

    private void Awake()
    {
        capsuleRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        if(capsuleRenderer == null)
        {
            capsuleRenderer = transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
        }
        healthBar = transform.GetChild(1);
        healthSlider = healthBar.transform.GetChild(0).GetComponent<Slider>();
        healthText = healthBar.transform.GetChild(1).GetComponent<Text>();
    }

    public void SetData(Declarations.EnemyData enemyData, Tile currentTile)
    {
        this.enemyData = enemyData;
        this.currentTile = currentTile;
        this.currentHealth = enemyData.Health;
        UpdateUI();
    }

    public Vector3 GetCenter()
    {
        return capsuleRenderer.bounds.center;
    }

    private void UpdateUI()
    {
        healthText.text = currentHealth.ToString();
        healthSlider.value = (float)currentHealth / enemyData.Health;
    }

    private void Update()
    {
        if ((currentTile.transform.position - transform.position).magnitude <= 0.3)
        {
            FindNextTile();
        }
        if (currentTile != null)
        {
            if(currentTile.Type == Declarations.TileType.Objective)
            {
                GameManager.instance.DealDamage(enemyData.Damage);
                GameManager.instance.SpawnManager.EnemyDestroyed(this);
                DestroyImmediate(gameObject);
                return;
            }
            var dir = currentTile.transform.position - transform.position;
            
            var baseLookRotation = Quaternion.LookRotation(dir);
            var baseRotation = Quaternion.Lerp(transform.rotation, baseLookRotation, Time.deltaTime * 4).eulerAngles;
            transform.rotation = Quaternion.Euler(0, baseRotation.y, 0);

            var speed = enemyData.Speed;
            var enemyInFront = GameManager.instance.SpawnManager.GetEnemyInFront(this);
            if(enemyInFront != null)
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
                    if(CurrentSpeed < enemyData.Speed)
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

            if (dir.magnitude < Time.deltaTime * CurrentSpeed)
            {
                transform.position = currentTile.transform.position;
            }
            else
            {
                transform.Translate(dir.normalized * Time.deltaTime * CurrentSpeed, Space.World);
            }
        }

        healthBar.LookAt(Camera.main.transform);
    }

    internal void DealDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            GameManager.instance.SpawnManager.EnemyDestroyed(this);
            GameManager.instance.AddMoney(enemyData.Award);
            DestroyImmediate(gameObject);
        }
        else
        {
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
                var nextTileIndex = UnityEngine.Random.Range(0, pathTiles.Count - 1);
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
}
