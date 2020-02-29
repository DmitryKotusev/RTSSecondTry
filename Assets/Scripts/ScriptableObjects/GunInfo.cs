using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Assets/Prefabs/Scriptables/GunInfo/GunInfo", menuName = "CustomScriptables/GunInfo")]
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

    //[BoxGroup("Gun Info")]
    //[SerializeField]
    //[Tooltip("Weapon's spread")]
    //protected float spread = 0.1f;

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
}
