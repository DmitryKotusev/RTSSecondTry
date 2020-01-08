using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class Unit : MonoBehaviour
{
    [BoxGroup("Colliders")]
    [SerializeField]
    List<ColliderCostPair> hitCollidersCosts;

    public List<ColliderCostPair> GetHitCollidersCosts()
    {
        return new List<ColliderCostPair>(hitCollidersCosts);
    }

    public float GetHitColliderCost(Collider hitCollider)
    {
        ColliderCostPair colliderCostPair = hitCollidersCosts.Find((hitCollidersCost) =>
        {
            return hitCollidersCost.collider == hitCollider;
        });

        if (colliderCostPair != null)
        {
            return colliderCostPair.cost;
        }

        return 1;
    }
}


[Serializable]
public class ColliderCostPair
{
    public Collider collider;
    public float cost;

    public float GetPriority()
    {
        if (collider == null)
        {
            return 0;
        }

        if (cost < 0)
        {
            return 0;
        }

        return cost * collider.bounds.size.magnitude * collider.bounds.size.magnitude;
    }
}
