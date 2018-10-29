using System;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Declarations
{
    public enum TileType
    {
        Spawn,
        Objective,
        Grass,
        Path,
        Environment,
        Unknown
    }

    public enum TowerType
    {
        Canon
    }
    public enum EnemyType
    {
        Capsule
    }

    public enum WavePartType
    {
        Delay,
        Spawn
    }

    public class IntVector2
    {
        public int x { get; set; }
        public int y { get; set; }

        public IntVector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class LevelData
    {
        public IntVector2 MapSize { get; private set; }
        public TileType[,] Map { get; private set; }

        public int StartMoney { get; private set; }
        public int StartHealth { get; private set; }
        public WaveData[] Waves { get; private set; }

        public LevelData(IntVector2 mapSize, TileType[,] map, WaveData[] waves, int startMoney, int startHealth)
        {
            MapSize = mapSize;
            Map = map;
            StartMoney = startMoney;
            StartHealth = startHealth;
            Waves = waves;
        }
    }

    public class WaveData
    {
        public WavePart[] WaveParts { get; private set; }

        public WaveData(WavePart[] waveParts)
        {
            WaveParts = waveParts;
        }
    }

    public abstract class WavePart
    {
        public WavePartType Type { get; protected set; }
    }

    public class SpawnWavePart : WavePart
    {
        public EnemyData EnemyToSpawn { get; private set; }

        public SpawnWavePart(EnemyData enemyToSpawn)
        {
            Type = WavePartType.Spawn;
            EnemyToSpawn = enemyToSpawn;
        }
    }

    public class DelayWavePart : WavePart
    {
        public float Delay { get; private set; }

        public DelayWavePart(float delay)
        {
            Type = WavePartType.Delay;
            Delay = delay;
        }
    }

    public abstract class TowerData
    {
        public UnityEvent Upgraded;

        public TowerType Type { get; protected set; }
        public TowerAssetData AssetData;

        public int Price { get; protected set; }
        public int Level2UpgradePrice { get; protected set; }
        public int Level3UpgradePrice { get; protected set; }

        public int Level1Range { get; protected set; }
        public int Level2Range { get; protected set; }
        public int Level3Range { get; protected set; }

        public int CurrentLevel { get; protected set; }
        public int CurrentPrice
        {
            get
            {
                switch (CurrentLevel)
                {
                    case 1:
                        return Price;
                    case 2:
                        return Price + Level2UpgradePrice;
                    case 3:
                        return Price + Level2UpgradePrice + Level3UpgradePrice;
                    default:
                        return -1;
                }
            }
        }
        public int CurrentUpgradePrice
        {
            get
            {
                switch (CurrentLevel)
                {
                    case 1:
                        return Level2UpgradePrice;
                    case 2:
                        return Level3UpgradePrice;
                    default:
                        return 0;
                }
            }
        }
        public int CurrentRange
        {
            get
            {
                switch (CurrentLevel)
                {
                    case 1:
                        return Level1Range;
                    case 2:
                        return Level2Range;
                    case 3:
                        return Level3Range;
                    default:
                        return Level3Range;
                }
            }
        }

        protected TowerData(TowerType type, TowerAssetData assetData, int price, int level2UpgradePrice, int level3UpgradePrice, int level1Range, int level2Range, int level3Range)
        {
            Type = type;
            AssetData = assetData;
            CurrentLevel = 1;
            Upgraded = new UnityEvent();

            Price = price;
            Level2UpgradePrice = level2UpgradePrice;
            Level3UpgradePrice = level3UpgradePrice;

            Level1Range = level1Range;
            Level2Range = level2Range;
            Level3Range = level3Range;
        }

        public TowerData(int level1Range, int level2Range, int level3Range)
        {
        }

        public void Upgrade()
        {
            if (CurrentLevel <= 2)
            {
                GameManager.instance.SubstractMoney(CurrentUpgradePrice);
                CurrentLevel++;
                if (Upgraded != null)
                {
                    Upgraded.Invoke();
                }
            }
            else
            {
                Debug.Log("Cant upgrade tower further");
            }
        }
    }

    public class CanonTower : TowerData
    {
        public int Level1Damage { get; private set; }
        public float Level1FireRate { get; private set; }

        public int Level2Damage { get; private set; }
        public float Level2FireRate { get; private set; }

        public int Level3Damage { get; private set; }
        public float Level3FireRate { get; private set; }

        public int CurrentDamage
        {
            get
            {
                switch (CurrentLevel)
                {
                    case 1:
                        return Level1Damage;
                    case 2:
                        return Level2Damage;
                    case 3:
                        return Level3Damage;
                    default:
                        return Level3Damage;
                }
            }
        }
        public float CurrentFireRate
        {
            get
            {
                switch (CurrentLevel)
                {
                    case 1:
                        return Level1FireRate;
                    case 2:
                        return Level2FireRate;
                    case 3:
                        return Level3FireRate;
                    default:
                        return Level3FireRate;
                }
            }
        }
        public CanonTower(int price, int level2UpgradePrice, int level3UpgradePrice, int level1Damage, float level1FireRate, int level2Damage, float level2FireRate, int level3Damage, float level3FireRate, int level1Range, int level2Range, int level3Range, TowerAssetData assetData) :
            base(TowerType.Canon, assetData, price, level2UpgradePrice, level3UpgradePrice, level1Range, level2Range, level3Range)
        {
            Level1Damage = level1Damage;
            Level1FireRate = level1FireRate;
            Level2Damage = level2Damage;
            Level2FireRate = level2FireRate;
            Level3Damage = level3Damage;
            Level3FireRate = level3FireRate;
        }
    }

    public class EnemyData
    {
        public EnemyType Type { get; protected set; }
        public EnemyAssetData AssetData;

        public int Health { get; protected set; }
        public float Speed { get; protected set; }
        public int Damage { get; protected set; }
        public int Award { get; internal set; }

        public EnemyData(EnemyAssetData assetData, int health, float speed, int damage, int award)
        {
            AssetData = assetData;
            Health = health;
            Speed = speed;
            Damage = damage;
            Award = award;
        }
    }
}