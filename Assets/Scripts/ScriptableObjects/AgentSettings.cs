using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Assets/Prefabs/Scriptables/AgentSettings/AgentSettings", menuName = "CustomScriptables/AgentSettings")]
public class AgentSettings : ScriptableObject
{
    [SerializeField]
    float lookDistance;

    [SerializeField]
    [Range(0, 100)]
    float desirableWeaponSpreadPersent = 5;

    [SerializeField]
    [Range(0, 100)]
    float maxWeaponSpreadPersent = 60;

    public float LookDistance
    {
        get
        {
            return lookDistance;
        }
    }

    public float DesirableWeaponSpreadPersent
    {
        get
        {
            return desirableWeaponSpreadPersent;
        } 
    }

    public float MaxWeaponSpreadPersent
    {
        get
        {
            return maxWeaponSpreadPersent;
        }
    }
}
