using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenDeathManager : DeathManager
{
    public override void Die()
    {
        GameObject greenParticlesObject = PoolsManager.GetObjectPool(PoolsKeys.greenGuysDeathParticlesPoolKey).GetObject();
        greenParticlesObject.transform.position = transform.position + transform.InverseTransformVector(localOffset);

        Agent agent = GetComponent<Agent>();
        agent?.GetCurrentFormation()?.RemoveAgentFromFormation(agent);

        if (pooledObject.pool != null)
        {
            pooledObject.pool.ReturnObject(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
