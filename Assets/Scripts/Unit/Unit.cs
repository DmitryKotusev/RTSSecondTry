using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Unit : MonoBehaviour
{
    [BoxGroup("Colliders")]
    [SerializeField]
    List<ColliderCostPair> hitCollidersCosts;

    public List<ColliderCostPair> GetHitCollidersCosts()
    {
        return new List<ColliderCostPair>(hitCollidersCosts);
    }
}
