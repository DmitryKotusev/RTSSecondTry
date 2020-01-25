using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PoolsManager : SerializedMonoBehaviour
{
    [Tooltip("Collection of pools")]
    [SerializeField]
    Dictionary<string, ObjectPool> poolsCollection = new Dictionary<string, ObjectPool>()
    {
        { Poolskeys.m16BulletsPoolKey, null },
        { Poolskeys.m16ShotEffectsPoolKey, null },
        { Poolskeys.clickEffectsPoolKey, null },
        { Poolskeys.greenGuysDeathParticlesPoolKey, null },
        { Poolskeys.tanGuysDeathParticlesPoolKey, null },
        { Poolskeys.attackClickEffectsPoolKey, null }
    };

    static PoolsManager instance = null;

    public PoolsManager()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        if (instance != this)
        {
            Destroy(this);
        }
    }

    static public ObjectPool GetObjectPool(string poolsName)
    {
        if (instance != null)
        {
            return instance.poolsCollection[poolsName];
        }

        return null;
    }
}

static public class Poolskeys
{
    static public readonly string m16BulletsPoolKey = "M16BulletsPoolKey";
    static public readonly string m16ShotEffectsPoolKey = "M16ShotEffectsPoolKey";
    static public readonly string clickEffectsPoolKey = "ClickEffectsPoolKey";
    static public readonly string attackClickEffectsPoolKey = "AttackClickEffectsPoolKey";
    static public readonly string greenGuysDeathParticlesPoolKey = "GreenGuysDeathParticlesPoolKey";
    static public readonly string tanGuysDeathParticlesPoolKey = "TanGuysDeathParticlesPoolKey";
}
