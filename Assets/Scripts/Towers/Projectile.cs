using UnityEngine;

public abstract class Projectile : MonoBehaviour {
    public abstract void SetTarget(Declarations.IProjectileData projectileData);
}
