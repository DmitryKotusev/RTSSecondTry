using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Assets/Prefabs/Scriptables/Health/HealthData", menuName = "CustomScriptables/HealthData")]
public class HealthData : ScriptableObject
{
    [SerializeField]
    [Min(1)]
    float maxHealth;

    public float MaxHealth
    {
        get
        {
            return maxHealth;
        }
    }
}
