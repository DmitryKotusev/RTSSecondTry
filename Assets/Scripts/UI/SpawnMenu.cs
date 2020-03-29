using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SpawnMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject spawnButtonPrefab;

    public void RegisterSpawnButton(SpawnGroup spawnGroup)
    {
        GameObject spawnButtonPrefabInstance = Instantiate(spawnButtonPrefab, transform);

        SpawnButton spawnButton = spawnButtonPrefabInstance.GetComponent<SpawnButton>();

        spawnButton.SpawnGroup = spawnGroup;
    }
}
