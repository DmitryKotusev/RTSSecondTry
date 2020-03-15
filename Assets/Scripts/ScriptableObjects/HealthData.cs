using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CustomScriptables/HealthData")]
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
