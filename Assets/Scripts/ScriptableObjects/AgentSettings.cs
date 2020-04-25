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

    [SerializeField]
    private float agentSpawnWeight = 2f;

    public float LookDistance => lookDistance;

    public float DesirableWeaponSpreadPersent => desirableWeaponSpreadPersent;

    public float MaxWeaponSpreadPersent => maxWeaponSpreadPersent;

    public float SpawnDistance => spawnDistance;

    public float MoveDistance => moveDistance;

    public float AgentSpawnWeight => agentSpawnWeight;
}
