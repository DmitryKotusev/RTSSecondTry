using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SpawnMenu : MonoBehaviour
{
    [SerializeField]
    [Required]
    [BoxGroup("!!!UI events hub!!!")]
    private UIEventsHub uIEventsHub;

    [SerializeField]
    private GameObject spawnButtonPrefab;

    [SerializeField]
    [ReadOnly]
    private List<SpawnButton> spawnButtons = new List<SpawnButton>();

    public List<SpawnButton> SpawnButtons
    {
        get => spawnButtons;
    }

    public void RegisterSpawnButton(SpawnGroup spawnGroup)
    {
        GameObject spawnButtonPrefabInstance = Instantiate(spawnButtonPrefab, transform);

        SpawnButton spawnButton = spawnButtonPrefabInstance.GetComponent<SpawnButton>();

        spawnButtons.Add(spawnButton);

        spawnButton.SpawnGroup = spawnGroup;
    }
    public void OnChangeBattlePointsTotal(int totalBattlePoints, int maxBattlePoints)
    {
        foreach (SpawnButton spawnButton in spawnButtons)
        {
            int buttonGroupSpawnCost = (int)spawnButton.GetGroupSpawnCost();

            spawnButton.ChangeBattlePointBlockStatus(totalBattlePoints < buttonGroupSpawnCost);
        }
    }

    public void OnChangeCommandPointsTotal(int totalCommandPoints, int maxCommandPoints)
    {
        foreach (SpawnButton spawnButton in spawnButtons)
        {
            int buttonGroupWeight = (int)spawnButton.GetGroupSpawnWeight();

            spawnButton.ChangeCommandPointBlockStatus(totalCommandPoints + buttonGroupWeight > maxCommandPoints);
        }
    }

    private void OnEnable()
    {
        uIEventsHub.ChangeBattlePointsTotal += OnChangeBattlePointsTotal;
        uIEventsHub.ChangeCommandPointsTotal += OnChangeCommandPointsTotal;
    }

    private void OnDisable()
    {
        uIEventsHub.ChangeBattlePointsTotal -= OnChangeBattlePointsTotal;
        uIEventsHub.ChangeCommandPointsTotal -= OnChangeCommandPointsTotal;
    }
}
