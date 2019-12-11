using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SoldierBasic : MonoBehaviour
{
    [BoxGroup("Bones")]
    [SerializeField]
    Transform rootBone;

    [BoxGroup("Bones")]
    [SerializeField]
    Transform headBone;

    [BoxGroup("Bones")]
    [SerializeField]
    Transform leftHandPoser;
    public Transform LeftHandPoser
    {
        get
        {
            return leftHandPoser;
        }
    }

    [BoxGroup("Bones")]
    [SerializeField]
    Transform rightHandPoser;
    public Transform RightHandPoser
    {
        get
        {
            return rightHandPoser;
        }
    }

    [BoxGroup("Colliders")]
    [SerializeField]
    List<ColliderCostPair> hitCollidersCosts;

    [SerializeField]
    Transform helmetHolder;
    public Transform HelmetHolder
    {
        get
        {
            return helmetHolder;
        }
    }

    [SerializeField]
    Transform weaponHolder;
    public Transform WeaponHolder
    {
        get
        {
            return weaponHolder;
        }
    }

    [SerializeField]
    Inventory inventory;
    public Inventory Inventory
    {
        get
        {
            return inventory;
        }
    }
}

[Serializable]
public struct ColliderCostPair
{
    public Collider collider;
    public float cost;
}
