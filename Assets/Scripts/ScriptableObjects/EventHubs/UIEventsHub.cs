using System;
using UnityEngine;

[CreateAssetMenu(menuName = "CustomScriptables/UIEventsHub")]
public class UIEventsHub : ScriptableObject
{
    public event Action<SpawnGroup> SpawnButtonPressed;

    public event Action<string> ChangeBattlePointsIncomeSpeed;

    public event Action<string> ChangeBattlePointsTotal;

    public event Action<string> ChangeCommandPointsTotal;

    public event Action<Team, string> ChangeTeamScore;

    public void TriggerSpawnButtonPressed(SpawnGroup spawnGroup)
    {
        SpawnButtonPressed?.Invoke(spawnGroup);
    }

    public void TriggerChangeBattlePointsIncomeSpeed(string incomeSpeed)
    {
        ChangeBattlePointsIncomeSpeed?.Invoke(incomeSpeed);
    }

    public void TriggerChangeBattlePointsTotal(string totalBattlePoints)
    {
        ChangeBattlePointsTotal?.Invoke(totalBattlePoints);
    }

    public void TriggerChangeCommandPointsTotal(string totalCommandPoints)
    {
        ChangeCommandPointsTotal?.Invoke(totalCommandPoints);
    }

    public void TriggerChangeTeamScore(Team team, string newScore)
    {
        ChangeTeamScore?.Invoke(team, newScore);
    }
}
