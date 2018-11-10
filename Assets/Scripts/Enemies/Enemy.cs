using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private Transform healthBar;
    private Slider healthSlider;
    private Text healthText;
    private MeshRenderer capsuleRenderer;

    private Declarations.EnemyData enemyData;
    private Tile currentTile;
    private Tile previousTile;
    private int currentHealth;

    private void Awake()
    {
        capsuleRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
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
            if (dir.magnitude < Time.deltaTime * enemyData.Speed)
            {
                transform.position = currentTile.transform.position;
            }
            else
            {
                transform.Translate((currentTile.transform.position - transform.position).normalized * Time.deltaTime * enemyData.Speed);
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
