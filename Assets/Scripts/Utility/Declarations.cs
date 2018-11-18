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
        Canon,
        Plasma
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

    public class TowerLevelData
    {
        public int Price { get; protected set; }

        public float Range { get; protected set; }

        public float FireRate { get; protected set; }

        public int Damage { get; private set; }
        
        public TowerLevelData(int price, float range, float fireRate, int damage)
        {
            Price = price;
            Range = range;
            FireRate = fireRate;
            Damage = damage;
        }
    }

    public class PlasmaLevelData : TowerLevelData
    {
        public float ExplosionRange { get; protected set; }

        public PlasmaLevelData(int price, float range, float fireRate, int damage, float explosionRange) : base(price, range, fireRate, damage)
        {
            ExplosionRange = explosionRange;
        }
    }

    public abstract class TowerData
    {
        public UnityEvent Upgraded;

        public TowerType Type { get; protected set; }
        public TowerAssetData AssetData;
        
        public TowerLevelData[] Levels { get; protected set; }

        public int CurrentLevel { get; protected set; }

        public int CurrentPrice
        {
            get
            {
                switch (CurrentLevel)
                {
                    case 1:
                        return Levels[0].Price;
                    case 2:
                        return Levels[0].Price + Levels[1].Price;
                    case 3:
                        return Levels[0].Price + Levels[1].Price + Levels[2].Price;
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
                        return Levels[1].Price;
                    case 2:
                        return Levels[2].Price;
                    default:
                        return 0;
                }
            }
        }

        public float CurrentRange
        {
            get
            {
                switch (CurrentLevel)
                {
                    case 1:
                        return Levels[0].Range;
                    case 2:
                        return Levels[1].Range;
                    case 3:
                        return Levels[2].Range;
                    default:
                        return Levels[2].Range;
                }
            }
        }

        public int CurrentDamage
        {
            get
            {
                switch (CurrentLevel)
                {
                    case 1:
                        return Levels[0].Damage;
                    case 2:
                        return Levels[1].Damage;
                    case 3:
                        return Levels[2].Damage;
                    default:
                        return Levels[3].Damage;
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
                        return Levels[0].FireRate;
                    case 2:
                        return Levels[1].FireRate;
                    case 3:
                        return Levels[2].FireRate;
                    default:
                        return Levels[2].FireRate;
                }
            }
        }

        protected TowerData(TowerType type, TowerAssetData assetData, TowerLevelData[] levels)
        {
            Type = type;
            AssetData = assetData;
            CurrentLevel = 1;
            Upgraded = new UnityEvent();

            Levels = levels;
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
        public CanonTower(TowerAssetData assetData, TowerLevelData[] levels) :
            base(TowerType.Canon, assetData, levels)
        {
        }
    }

    public class PlasmaTower : TowerData
    {
        public float CurrentExplosionRange
        {
            get
            {
                switch (CurrentLevel)
                {
                    case 1:
                        return (Levels[0] as PlasmaLevelData).ExplosionRange;
                    case 2:
                        return (Levels[1] as PlasmaLevelData).ExplosionRange;
                    case 3:
                        return (Levels[2] as PlasmaLevelData).ExplosionRange;
                    default:
                        return (Levels[2] as PlasmaLevelData).ExplosionRange;
                }
            }
        }
        public PlasmaTower(TowerAssetData assetData, PlasmaLevelData[] levels) :
            base(TowerType.Plasma, assetData, levels)
        {
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

    public interface IProjectileData
    {

    }

    public class TargetableProjectileData : IProjectileData
    {
        public Enemy Target;

        public TargetableProjectileData(Enemy target)
        {
            Target = target;
        }
    }

    public class CanonBallData : TargetableProjectileData
    {
        public int Damage;

        public CanonBallData(Enemy target, int damage) : base(target)
        {
            Damage = damage;
        }
    }

    public class PlasmaBallData : TargetableProjectileData
    {
        public int ExprosionDamage;
        public float ExplosionRange;

        public PlasmaBallData(Enemy target, int exprosionDamage, float explosionRange) : base(target)
        {
            ExprosionDamage = exprosionDamage;
            ExplosionRange = explosionRange;
        }
    }
}