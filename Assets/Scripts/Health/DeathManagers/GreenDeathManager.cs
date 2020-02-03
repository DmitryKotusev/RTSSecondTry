using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenDeathManager : DeathManager
{
    public override void Die()
    {
        GameObject greenParticlesObject = PoolsManager.GetObjectPool(Poolskeys.greenGuysDeathParticlesPoolKey).GetObject();
        greenParticlesObject.transform.position = transform.position + transform.InverseTransformVector(localOffset);

        Agent agent = GetComponent<Agent>();
        agent?.GetCurrentFormation()?.RemoveAgentFromFormation(agent);

        Destroy(gameObject);
    }
}
