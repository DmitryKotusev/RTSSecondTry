using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PoolsManager : SerializedMonoBehaviour
{
    [Tooltip("Collection of pools")]
    [SerializeField]
    Dictionary<string, ObjectPool> poolsCollection = new Dictionary<string, ObjectPool>()
    {
        { PoolsKeys.m16BulletsPoolKey, null },
        { PoolsKeys.m16ShotEffectsPoolKey, null },
        { PoolsKeys.clickEffectsPoolKey, null },
        { PoolsKeys.greenGuysDeathParticlesPoolKey, null },
        { PoolsKeys.tanGuysDeathParticlesPoolKey, null },
        { PoolsKeys.attackClickEffectsPoolKey, null },
        { PoolsKeys.greenRifleManPoolKey, null },
        { PoolsKeys.tanRifleManPoolKey, null },
        { PoolsKeys.m16RifleGreenSoldierHitEffectPoolKey, null },
        { PoolsKeys.m16RifleTanSoldierHitEffectPoolKey, null },
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
            if (instance.poolsCollection.ContainsKey(poolsName))
            {
                return instance.poolsCollection[poolsName];
            }
        }

        return null;
    }
}
