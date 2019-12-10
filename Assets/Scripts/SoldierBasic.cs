using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using RootMotion.FinalIK;

public class SoldierBasic : MonoBehaviour
{
    [BoxGroup("Bones")]
    [SerializeField]
    GameObject rootBone;

    [BoxGroup("Bones")]
    [SerializeField]
    GameObject headBone;

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
