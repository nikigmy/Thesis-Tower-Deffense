using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    public UnityEvent LevelCompleted;
    public List<Enemy> enemies;

    private Tile spawnTile;
    private Tile SpawnTile
    {
        get
        {
            if (spawnTile == null)
            {
                spawnTile = GameManager.instance.MapGenerator.GetSpawnTile();
            }
            return spawnTile;
        }
    }

    private const float delayBetweenWaves = 5;

    private int currentWaveIndex;
    private int currentWavePartIndex;

    private List<Declarations.WaveData> currentWaves;
    private Declarations.WaveData currentWave;

    private void Start()
    {
        enemies = new List<Enemy>();
    }

    public void SpawnWaves(List<Declarations.WaveData> waves)
    {
        currentWaves = waves;
        currentWaveIndex = 0;
        StartCoroutine(WaitBetweenWaves(delayBetweenWaves));
    }

    private void SpawnWave()
    {
        currentWave = currentWaves[currentWaveIndex];
        NextWavePart();
    }

    private void NextWavePart()
    {
        if (currentWavePartIndex < currentWave.WaveParts.Count)
        {
            if (currentWave.WaveParts[currentWavePartIndex].Type == Declarations.WavePartType.Delay)
            {
                StartCoroutine(WaitForDelay((currentWave.WaveParts[currentWavePartIndex] as Declarations.DelayWavePart).Delay));
            }
            else
            {
                var enemyData = (currentWave.WaveParts[currentWavePartIndex] as Declarations.SpawnWavePart).EnemyToSpawn;
                if (SpawnTile != null && enemyData != null)
                {
                    var enemy = Instantiate(enemyData.AssetData.Prefab, SpawnTile.transform.position, Quaternion.identity).GetComponent<Enemy>();
                    enemy.SetData(SpawnTile);
                    enemies.Add(enemy);

                    currentWavePartIndex++;
                    NextWavePart();
                }
                else
                {
                    Debug.Log("Cant find spawn tile");
                }
            }
        }
        else
        {
            if (currentWaveIndex < currentWaves.Count - 1)
            {
                currentWavePartIndex = 0;
                currentWaveIndex++;
                StartCoroutine(WaitBetweenWaves(delayBetweenWaves));
            }
        }
    }

    internal Enemy GetEnemyInFront(Enemy enemy)
    {
        var enemyIndex = enemies.IndexOf(enemy);
        if (enemyIndex >= 1)
        {
            var enemiesInFront = enemies.Take(enemyIndex);
            var isGroundUnit = Helpers.IsGroundUnit(enemy.Type);
            for (int i = enemiesInFront.Count() - 1; i >= 0; i--)
            {
                if (Helpers.IsGroundUnit(enemiesInFront.ElementAt(i).Type) == isGroundUnit)
                {
                    return enemiesInFront.ElementAt(i);
                }
            }
        }
        return null;
    }

    internal void EnemyDestroyed(Enemy enemy)
    {
        enemies.Remove(enemy);
        if (currentWaveIndex >= currentWaves.Count - 1 && currentWavePartIndex >= currentWave.WaveParts.Count - 1 && enemies.Count == 0)
        {
            if (GameManager.instance.Health > 0)
            {
                LevelCompleted.Invoke();
            }
        }
    }

    IEnumerator WaitForDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentWavePartIndex++;
        NextWavePart();
    }

    IEnumerator WaitBetweenWaves(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnWave();
    }
}
