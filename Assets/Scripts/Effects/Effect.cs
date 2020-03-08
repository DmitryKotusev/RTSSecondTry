using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect : MonoBehaviour
{
    public abstract void ShowEffect();

    protected IEnumerator SelfDestroy(List<float> durations)
    {
        float maxDuration = Mathf.Max(durations.ToArray());
        yield return new WaitForSeconds(maxDuration);

        PooledObject pooledObject = GetComponent<PooledObject>();
        pooledObject.pool.ReturnObject(gameObject);
    }
}
