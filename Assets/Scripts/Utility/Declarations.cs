﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Declarations
{
    public enum EffectType
    {
        Speed,
        PhysicalResistance,
        FireResistance,
        FrostResistance,
        Slow,
        Stun
    }

    public enum TileType
    {
        Grass,
        Path,
        Objective,
        Spawn,
        Environment,
        Empty,
        Unknown
    }

    public enum TowerType
    {
        Canon,
        Plasma,
        Crystal,
        Tesla
    }

    public enum EnemyType
    {
        Swordsman,
        Golem,
        Dragon
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

    #region Levels
    public class LevelData
    {
        public IntVector2 MapSize { get; private set; }
        public TileType[,] Map { get; set; }

        public int StartMoney { get; private set; }
        public int StartHealth { get; private set; }
        public List<WaveData> Waves { get; private set; }
        public Sprite previewSprite { get; private set; }

        public LevelData(IntVector2 mapSize, TileType[,] map, List<WaveData> waves, int startMoney, int startHealth)
        {
            MapSize = mapSize;
            Map = map;
            StartMoney = startMoney;
            StartHealth = startHealth;
            Waves = waves;
            previewSprite = TextureGenerator.GetTextureForLevel(this);
        }

        public XElement Export()
        {
            var level = new XElement("Level", new object[] { new XAttribute("StartMoney", StartMoney), new XAttribute("StartHealth", StartHealth) });
            StringBuilder mapString = new StringBuilder("\n");
            for (int i = 0; i < MapSize.y; i++)
            {
                for (int j = 0; j < MapSize.x; j++)
                {
                    mapString.Append(Helpers.GetTileTypeChar(Map[i, j]));
                }
                mapString.Append("\n");
            }
            var mapElement = new XElement("Map", new object[] { new XAttribute("Width", MapSize.x), new XAttribute("Height", MapSize.y), mapString.ToString() });

            var spawnData = new XElement("SpawnData");
            foreach (var wave in Waves)
            {
                var waveElement = new XElement("Wave");
                foreach (var wavePart in wave.WaveParts)
                {
                    if(wavePart.Type == WavePartType.Spawn)
                    {
                        waveElement.Add(new XElement("Enemy", new object[] { new XAttribute("Type", ((SpawnWavePart)wavePart).EnemyToSpawn.Type.ToString()) }));
                    }
                    else
                    {
                        waveElement.Add(new XElement("Delay", new object[] { new XAttribute("Time", ((DelayWavePart)wavePart).Delay.ToString()) }));
                    }
                }
                spawnData.Add(waveElement);
            }

            level.Add(mapElement);
            level.Add(spawnData);

            return level;
        }
    }

    public class WaveData
    {
        public List<WavePart> WaveParts { get; private set; }

        public WaveData(List<WavePart> waveParts)
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
        public EnemyData EnemyToSpawn { get; set; }

        public SpawnWavePart(EnemyData enemyToSpawn)
        {
            Type = WavePartType.Spawn;
            EnemyToSpawn = enemyToSpawn;
        }
    }

    public class DelayWavePart : WavePart
    {
        public float Delay { get;  set; }

        public DelayWavePart(float delay)
        {
            Type = WavePartType.Delay;
            Delay = delay;
        }
    }
    #endregion Levels

    #region Towers
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

    public class CrystalLevelData : TowerLevelData
    {
        public int SlowEffect { get; protected set; }
        public float SlowDuration { get; protected set; }

        public CrystalLevelData(int price, float range, float fireRate, int damage, int slowEffect, float slowDuration) : base(price, range, fireRate, damage)
        {
            SlowEffect = slowEffect;
            SlowDuration = slowDuration;
        }
    }

    public class TeslaLevelData : TowerLevelData
    {
        public int MaxBounces { get; protected set; }
        public float BounceRange { get; protected set; }

        public TeslaLevelData(int price, float range, float fireRate, int damage, int maxBounces, float bounceRange) : base(price, range, fireRate, damage)
        {
            MaxBounces = maxBounces;
            BounceRange = bounceRange;
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
                        return Levels[1].Price * (10 + GameManager.instance.BuildManager.GetBuiltTowersCount(Type));
                    case 2:
                        return Levels[2].Price * (10 + GameManager.instance.BuildManager.GetBuiltTowersCount(Type));
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
            if (GameManager.instance.Money >= CurrentUpgradePrice)
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

        internal void ResetLevel()
        {
            CurrentLevel = 1;
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

    public class CrystalTower : TowerData
    {
        public int CurrentSlowEffect
        {
            get
            {
                switch (CurrentLevel)
                {
                    case 1:
                        return (Levels[0] as CrystalLevelData).SlowEffect;
                    case 2:
                        return (Levels[1] as CrystalLevelData).SlowEffect;
                    case 3:
                        return (Levels[2] as CrystalLevelData).SlowEffect;
                    default:
                        return (Levels[2] as CrystalLevelData).SlowEffect;
                }
            }
        }

        public float CurrentSlowDuration
        {
            get
            {
                switch (CurrentLevel)
                {
                    case 1:
                        return (Levels[0] as CrystalLevelData).SlowDuration;
                    case 2:
                        return (Levels[1] as CrystalLevelData).SlowDuration;
                    case 3:
                        return (Levels[2] as CrystalLevelData).SlowDuration;
                    default:
                        return (Levels[2] as CrystalLevelData).SlowDuration;
                }
            }
        }

        public CrystalTower(TowerAssetData assetData, CrystalLevelData[] levels) :
            base(TowerType.Crystal, assetData, levels)
        {
        }
    }

    public class TeslaTower : TowerData
    {
        public int CurrentMaxBounces
        {
            get
            {
                switch (CurrentLevel)
                {
                    case 1:
                        return (Levels[0] as TeslaLevelData).MaxBounces;
                    case 2:
                        return (Levels[1] as TeslaLevelData).MaxBounces;
                    case 3:
                        return (Levels[2] as TeslaLevelData).MaxBounces;
                    default:
                        return (Levels[2] as TeslaLevelData).MaxBounces;
                }
            }
        }

        public float CurrentBounceRange
        {
            get
            {
                switch (CurrentLevel)
                {
                    case 1:
                        return (Levels[0] as TeslaLevelData).BounceRange;
                    case 2:
                        return (Levels[1] as TeslaLevelData).BounceRange;
                    case 3:
                        return (Levels[2] as TeslaLevelData).BounceRange;
                    default:
                        return (Levels[2] as TeslaLevelData).BounceRange;
                }
            }
        }

        public TeslaTower(TowerAssetData assetData, TeslaLevelData[] levels) :
            base(TowerType.Tesla, assetData, levels)
        {
        }
    }
    #endregion Towers

    #region Enemies
    public class EnemyData
    {
        public EnemyType Type { get; set; }
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
    #endregion Enemies

    #region Projectiles
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
        public int ExplosionDamage;
        public float ExplosionRange;

        public PlasmaBallData(Enemy target, int exprosionDamage, float explosionRange) : base(target)
        {
            ExplosionDamage = exprosionDamage;
            ExplosionRange = explosionRange;
        }
    }

    public class LightningBoltData : TargetableProjectileData
    {
        public int Damage;
        public int MaxBounces;
        public float BounceRange;
        public GameObject StartPosition;

        public LightningBoltData(Enemy target, GameObject startPosition, int damage, int maxBounces, float bounceRange) : base(target)
        {
            MaxBounces = maxBounces;
            BounceRange = bounceRange;
            StartPosition = startPosition;
            Damage = damage;
        }
    }

    public class IceMissileData : TargetableProjectileData
    {
        public int Damage;
        public int SlowEfect;
        public float SlowDuration;

        public IceMissileData(Enemy target, int damage, int slowEfect, float slowDuration) : base(target)
        {
            SlowDuration = slowDuration;
            SlowEfect = slowEfect;
            Damage = damage;
        }
    }

    #endregion Projectiles

    public class Effect
    {
        public EffectType Type;
        public float Duration;
        public float Value;

        public Effect(EffectType type, float duration, float value)
        {
            Type = type;
            Duration = duration;
            Value = value;
        }

        public Effect(Effect from)
        {
            Type = from.Type;
            Duration = from.Duration;
            Value = from.Value;
        }
    }
}