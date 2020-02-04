using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TanDeathManager : DeathManager
{
    public override void Die()
    {
        GameObject tanParticlesObject = PoolsManager.GetObjectPool(PoolsKeys.tanGuysDeathParticlesPoolKey).GetObject();
        tanParticlesObject.transform.position = transform.position + transform.InverseTransformVector(localOffset);

        Agent agent = GetComponent<Agent>();
        agent?.GetCurrentFormation()?.RemoveAgentFromFormation(agent);

        Destroy(gameObject);
    }
}
