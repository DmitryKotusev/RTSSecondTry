using UnityEngine;
using Sirenix.OdinInspector;

public abstract class Gun : Item
{
    [BoxGroup("Gun Info")]
    [SerializeField]
    [Required]
    protected GunInfo gunInfo;

    [SerializeField]
    [Required]
    protected Transform roundEmitter;

    [SerializeField]
    protected int currentClipRoundsLeft = -1;

    protected float timeTillNextShot = 0f;

    protected Vector3 lastFrameEmitterPosition;

    protected float currentSpread;

    public GunInfo GunInfo
    {
        get
        {
            return gunInfo;
        }
    }

    public float ProjectileSpeed
    {
        get
        {
            return gunInfo.ProjectileSpeed;
        }
    }

    public Transform RoundEmitter
    {
        get
        {
            return roundEmitter;
        }
    }

    public float CurrentSpread
    {
        get
        {
            return currentSpread;
        }
    }

    public bool IsReloadRequired()
    {
        return currentClipRoundsLeft == 0;
    }

    public abstract void Fire();

    public float GetCurrentSpreadPercent()
    {
        float delta = gunInfo.MaxSpread - gunInfo.Spread;

        float currentDelta = currentSpread - gunInfo.Spread;

        return currentDelta / delta * 100;
    }

    public int CurrentClipRoundsLeft
    {
        get => currentClipRoundsLeft;
        set
        {
            currentClipRoundsLeft = Mathf.Clamp(value, 0, gunInfo.BulletsPerClip);
        }
    }

    protected virtual void Awake()
    {
        InitClipRoundsLeft();
    }

    protected virtual void InitClipRoundsLeft()
    {
        if (currentClipRoundsLeft == -1)
        {
            currentClipRoundsLeft = gunInfo.BulletsPerClip;
        }
    }
}
