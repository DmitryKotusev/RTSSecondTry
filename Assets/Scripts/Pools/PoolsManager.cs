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
        { Poolskeys.m16ShotEffectsPoolKey, null }
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
}
