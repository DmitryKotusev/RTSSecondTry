using UnityEngine;
using Sirenix.OdinInspector;

public abstract class Gun : Item
{
    [BoxGroup("Gun Info")]
    [SerializeField]
    protected GunInfo gunInfo;

    public float ProjectileSpeed
    {
        get
        {
            return gunInfo.ProjectileSpeed;
        }
    }

    [SerializeField]
    protected Transform roundEmitter;
    public Transform RoundEmitter
    {
        get
        {
            return roundEmitter;
        }
    }

    protected float timeTillNextShot = 0f;

    public abstract void Fire();
}
