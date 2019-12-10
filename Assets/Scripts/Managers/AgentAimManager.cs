using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using RootMotion.FinalIK;

public class AgentAimManager : MonoBehaviour
{
    [BoxGroup("Settings")]
    [SerializeField]
    [Tooltip("Speed of going to aim state (units per second)")]
    float aimSpeed = 1f;

    [SerializeField]
    AimIK aimIK;

    [SerializeField]
    FullBodyBipedIK fullBodyBipedIK;
}
