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
    GameObject helmet;

    [SerializeField]
    Team team;

    public Team Team
    {
        get
        {
            return team;
        }
    }
}
