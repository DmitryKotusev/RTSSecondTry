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

    protected Vector3 lastFrameEmitterPosition;

    protected float currentSpread;
    public float CurrentSpread
    {
        get
        {
            return currentSpread;
        }
    }

    public abstract void Fire();

    public float GetCurrentSpreadPercent()
    {
        float delta = gunInfo.MaxSpread - gunInfo.Spread;

        float currentDelta = currentSpread - gunInfo.Spread;

        return currentDelta / delta * 100;
    }
}
