using UnityEngine;
using Sirenix.OdinInspector;

public abstract class Gun : Item
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
