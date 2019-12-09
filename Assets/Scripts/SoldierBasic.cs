using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

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
    GameObject leftHand;

    [BoxGroup("Bones")]
    [SerializeField]
    GameObject rightHand;

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
    Inventory inventory;
    public Inventory Inventory
    {
        get
        {
            return inventory;
        }
    }
}
