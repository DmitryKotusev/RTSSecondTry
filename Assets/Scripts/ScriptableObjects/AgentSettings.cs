using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Assets/Prefabs/Scriptables/AgentSettings/AgentSettings", menuName = "CustomScriptables/AgentSettings")]
public class AgentSettings : ScriptableObject
{
    [SerializeField]
    float lookDistance;

    public float LookDistance
    {
        get
        {
            return lookDistance;
        }
    }
}
