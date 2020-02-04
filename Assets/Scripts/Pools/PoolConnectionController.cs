using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[ExecuteInEditMode]
[RequireComponent(typeof(PooledObject))]
public class PoolConnectionController : MonoBehaviour
{
    [SerializeField]
    [Required]
    PooledObject pooledObject;

    [SerializeField]
    [Tooltip("Key of the pool in the pool collection")]
    string poolKey = "";

    void Start()
    {
        ObjectPool objectPool = PoolsManager.GetObjectPool(poolKey);

        if (objectPool != null)
        {
            pooledObject.pool = objectPool;
            Debug.Log("Successfully found pool for gameobject " + name);
        }
        else
        {
            Debug.Log("Could not find pool for gameobject " + name);
        }
    }
}
