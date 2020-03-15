using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "CustomScriptables/ControllerSettings")]
public class ControllerSettings : ScriptableObject
{
    [BoxGroup("Common settings")]
    [SerializeField]
    [Tooltip("Max amount of agents that can be selected per time")]
    private float maxAgentSelectionCount = 15f;
    public float MaxAgentSelectionCount
    {
        get
        {
            return maxAgentSelectionCount;
        }
    }

    [BoxGroup("Common settings")]
    [SerializeField]
    [Tooltip("Agent radius multiplier when counting formation agents position")]
    private float radiusMultiplier = 4f;
    public float RadiusMultiplier
    {
        get
        {
            return radiusMultiplier;
        }
    }
}
