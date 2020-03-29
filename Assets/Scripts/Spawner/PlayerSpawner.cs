using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerSpawner : Spawner
{
    public override bool SpawnGroup(SpawnGroup spawnGroup)
    {
        if (spawnPoint == null)
        {
            return false;
        }

        if (battlePointsManager.CurrentPointsAmount < spawnGroup.PointsCost)
        {
            return false;
        }

        if (spawnPoint.SpawnGroup(spawnGroup))
        {
            battlePointsManager.CurrentPointsAmount -= spawnGroup.PointsCost;

            return true;
        }

        return false;
    }

    private void Awake()
    {
        if (GetComponent<Controller>() != LevelManager.Instance.LevelUI.PlayerController)
        {
            return;
        }

        foreach (SpawnGroup spawnGroup in availableGroupsInfo.AvailableSpawnGroups)
        {
            LevelManager.Instance.LevelUI.SpawnMenu.RegisterSpawnButton(spawnGroup);
        }
    }
}
