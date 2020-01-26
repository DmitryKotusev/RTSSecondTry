using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using RootMotion.FinalIK;

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
    HandPoser leftHandPoser;
    public HandPoser LeftHandPoser
    {
        get
        {
            return leftHandPoser;
        }
    }

    [BoxGroup("Bones")]
    [SerializeField]
    Transform rightHand;
    public Transform RightHand
    {
        get
        {
            return rightHand;
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
