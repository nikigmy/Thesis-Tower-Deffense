﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
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

    private Declarations.WaveData[] currentWaves;
    private Declarations.WaveData currentWave;

    private void Start()
    {
        enemies = new List<Enemy>();
    }

    public void SpawnWaves(Declarations.WaveData[] waves)
    {
        currentWaves = waves;
        currentWaveIndex = 0;
        StartCoroutine(WaitBetweenWaves());
    }

    private void SpawnWave()
    {
        currentWave = currentWaves[currentWaveIndex];
        currentWavePartIndex = 0;
        NextWavePart();
    }

    private void NextWavePart()
    {
        if (currentWavePartIndex < currentWave.WaveParts.Length)
        {
            if (currentWave.WaveParts[currentWavePartIndex].Type == Declarations.WavePartType.Delay)
            {
                StartCoroutine(WaitForDelay());
            }
            else
            {
                var enemyData = (currentWave.WaveParts[currentWavePartIndex] as Declarations.SpawnWavePart).EnemyToSpawn;
                if (SpawnTile != null && enemyData != null)
                {
                    var enemy = Instantiate(enemyData.AssetData.Prefab, SpawnTile.transform.position, Quaternion.identity).GetComponent<Enemy>();
                    enemy.SetData(enemyData, SpawnTile);
                    enemies.Add(enemy);

                    currentWavePartIndex++;
                    NextWavePart();
                    Debug.Log("Spawned enemy");
                }
                else
                {
                    Debug.Log("Cant find spawn tile");
                }
            }
        }
        else
        {
            if (currentWaveIndex < currentWaves.Length - 1)
            {
                currentWaveIndex++;
                StartCoroutine(WaitBetweenWaves());
            }
        }
    }

    internal void EnemyDestroyed(Enemy enemy)
    {
        enemies.Remove(enemy);
    }

    IEnumerator WaitForDelay()
    {
        yield return new WaitForSeconds((currentWave.WaveParts[currentWavePartIndex] as Declarations.DelayWavePart).Delay);
        currentWavePartIndex++;
        NextWavePart();
    }
    IEnumerator WaitBetweenWaves()
    {
        yield return new WaitForSeconds(delayBetweenWaves);
        SpawnWave();
    }
}
