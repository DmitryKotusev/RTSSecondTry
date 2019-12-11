using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SoldierBasic : Unit
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
