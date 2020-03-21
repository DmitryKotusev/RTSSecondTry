using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "CustomScriptables/GunInfo")]
public class GunInfo : ScriptableObject
{
    [BoxGroup("Gun Info")]
    [SerializeField]
    [Tooltip("Shots per minute")]
    protected float rateOfFire;

    [BoxGroup("Gun Info")]
    [SerializeField]
    [Tooltip("Gun cooldown time in seconds")]
    protected float cooldown;

    [BoxGroup("Gun Info")]
    [SerializeField]
    [Tooltip("Gun cooldown preparation ratio in seconds")]
    [Range(0.05f, 0.4f)]
    protected float cooldownPreparationRatio;

    [BoxGroup("Gun Info")]
    [SerializeField]
    [Tooltip("Gun cooldown finish ratio in seconds")]
    [Range(0.05f, 0.4f)]
    protected float cooldownFinishRatio;

    [BoxGroup("Gun Info")]
    [SerializeField]
    [Tooltip("Gun's damage")]
    protected float basicDamagePerShot = 10f;

    [BoxGroup("Gun Info")]
    [SerializeField]
    [Tooltip("Projectile speed in toys meters per second")]
    protected float projectileSpeed = 300f;

    [BoxGroup("Gun Info")]
    [SerializeField]
    [Tooltip("Bullets per clip")]
    protected int bulletsPerClip = 30;

    [BoxGroup("Gun Info")]
    [SerializeField]
    [Tooltip("Gun fire distance (in meters)")]
    protected int fireDistance = 60;

    [BoxGroup("Gun Info")]
    [SerializeField]
    [Tooltip("Weapon's spread")]
    protected float spread = 0.1f;

    [BoxGroup("Gun Info")]
    [SerializeField]
    [Tooltip("Max weapon's spread")]
    protected float maxSpread = 1.5f;

    [BoxGroup("Gun Info")]
    [SerializeField]
    [Tooltip("Weapon's spread decrease speed")]
    protected float spreadDeacreseSpeed = 0.05f;

    [BoxGroup("Gun Info")]
    [SerializeField]
    [Tooltip("Weapon's spread increase speed (per shot)")]
    protected float spreadIncreseSpeedForShot = 0.1f;

    [BoxGroup("Gun Info")]
    [SerializeField]
    [Tooltip("Weapon's spread increase speed (per move)")]
    protected float spreadIncreseSpeedForMove = 10f;

    [SerializeField]
    [Required]
    private AnimationClip reloadAnimation;

    [SerializeField]
    private ClipHandInfo usualClipHandInfo = new ClipHandInfo();

    public float RateOfFire
    {
        get
        {
            return rateOfFire;
        }
    }

    public float Cooldown
    {
        get
        {
            return cooldown;
        }
    }

    public float CooldownPreparationRatio
    {
        get
        {
            return cooldownPreparationRatio;
        }
    }

    public float CooldownFinishRatio
    {
        get
        {
            return cooldownFinishRatio;
        }
    }

    public float BasicDamagePerShot
    {
        get
        {
            return basicDamagePerShot;
        }
    }

    public float ProjectileSpeed
    {
        get
        {
            return projectileSpeed;
        }
    }

    public int BulletsPerClip
    {
        get
        {
            return bulletsPerClip;
        }
    }

    public int FireDistance
    {
        get
        {
            return fireDistance;
        }
    }

    public float Spread
    {
        get
        {
            return spread;
        }
    }

    public float MaxSpread
    {
        get
        {
            return maxSpread;
        }
    }

    public float SpreadDeacreseSpeed
    {
        get
        {
            return spreadDeacreseSpeed;
        }
    }

    public float SpreadIncreseSpeedForShot
    {
        get
        {
            return spreadIncreseSpeedForShot;
        }
    }

    public float SpreadIncreseSpeedForMove
    {
        get
        {
            return spreadIncreseSpeedForMove;
        }
    }

    public AnimationClip ReloadAnimation
    {
        get
        {
            return reloadAnimation;
        }
    }

    public ClipHandInfo UsualClipHandInfo
    {
        get
        {
            return usualClipHandInfo;
        }
    }
}
