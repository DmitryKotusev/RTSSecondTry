using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CustomScriptables/SpawnGroup")]
public class SpawnGroup : ScriptableObject
{
    [SerializeField]
    private List<KeyValuePair<GameObject, int>> spawnPairs;

    public List<KeyValuePair<GameObject, int>> SpawnPairs => spawnPairs;
}
