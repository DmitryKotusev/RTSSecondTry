using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    [Tooltip("Life time in seconds")]
    public float lifeTime = 2f;

    private void Awake()
    {
        StartCoroutine(ReturnToPool());
    }

    IEnumerator ReturnToPool()
    {
        yield return new WaitForSeconds(lifeTime);

        PooledObject pooledObject = GetComponent<PooledObject>();
        pooledObject.pool.ReturnObject(gameObject);
    }
}
