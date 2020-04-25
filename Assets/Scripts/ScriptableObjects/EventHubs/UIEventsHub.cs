using System;
using UnityEngine;

[CreateAssetMenu(menuName = "CustomScriptables/UIEventsHub")]
public class UIEventsHub : ScriptableObject
{
    public event Action<SpawnGroup> SpawnButtonPressed;

    public event Action<string> ChangeBattlePointsIncomeSpeed;

    public event Action<int, int> ChangeBattlePointsTotal;

    public event Action<int, int> ChangeCommandPointsTotal;

    public event Action<Team, string> ChangeTeamScore;

    public void TriggerSpawnButtonPressed(SpawnGroup spawnGroup)
    {
        SpawnButtonPressed?.Invoke(spawnGroup);
    }

    public void TriggerChangeBattlePointsIncomeSpeed(string incomeSpeed)
    {
        ChangeBattlePointsIncomeSpeed?.Invoke(incomeSpeed);
    }

    public void TriggerChangeBattlePointsTotal(int totalBattlePoints, int maxBattlePoints)
    {
        ChangeBattlePointsTotal?.Invoke(totalBattlePoints, maxBattlePoints);
    }

    public void TriggerChangeCommandPointsTotal(int totalCommandPoints, int maxCommandPoints)
    {
        ChangeCommandPointsTotal?.Invoke(totalCommandPoints, maxCommandPoints);
    }

    public void TriggerChangeTeamScore(Team team, string newScore)
    {
        ChangeTeamScore?.Invoke(team, newScore);
    }
}
