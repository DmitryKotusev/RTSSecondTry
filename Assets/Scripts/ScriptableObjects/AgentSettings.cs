using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CustomScriptables/AgentSettings")]
public class AgentSettings : ScriptableObject
{
    [SerializeField]
    private float lookDistance;

    [SerializeField]
    [Range(0, 100)]
    private float desirableWeaponSpreadPersent = 5;

    [SerializeField]
    [Range(0, 100)]
    private float maxWeaponSpreadPersent = 60;

    [SerializeField]
    private float spawnDistance = 2f;

    [SerializeField]
    private float moveDistance = 2f;

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

    public float SpawnDistance
    {
        get
        {
            return spawnDistance;
        }
    }

    public float MoveDistance
    {
        get
        {
            return moveDistance;
        }
    }
}
