using System.Collections.Generic;
using UnityEngine;

public class TeamsVisibleUnitsStore
{
    private Dictionary<Team, HashSet<Unit>> teamsVisibleUnits = new Dictionary<Team, HashSet<Unit>>();

    public void ClearTeamsVisibleUnits()
    {
        foreach (var teamVisibleUnits in teamsVisibleUnits)
        {
            teamsVisibleUnits[teamVisibleUnits.Key].Clear();
        }
    }

    public void RegisterVisibleUnit(Team team, Unit unit)
    {
        if (teamsVisibleUnits.ContainsKey(team))
        {
            teamsVisibleUnits[team].Add(unit);
        }
        else
        {
            teamsVisibleUnits.Add(team, new HashSet<Unit>() { unit });
        }
    }

    public void RegisterVisibleUnitsRange(Team team, IEnumerable<Unit> units)
    {
        if (!teamsVisibleUnits.ContainsKey(team))
        {
            teamsVisibleUnits.Add(team, new HashSet<Unit>());
        }

        foreach (var unit in units)
        {
            teamsVisibleUnits[team].Add(unit);
        }
    }

    public HashSet<Unit> GetTeamVisibleUnits(Team team)
    {
        if (teamsVisibleUnits.ContainsKey(team))
        {
            return teamsVisibleUnits[team];
        }

        return new HashSet<Unit>();
    }

    public List<Unit> GetTeamUnitsInFieldOfViewOrderedByDistance(float distance, Team team, Transform transform)
    {
        List<Unit> units = new List<Unit>(GetTeamVisibleUnits(team));

        units.Sort((enemyUnit1, enemyUnit2) =>
        {
            return (enemyUnit1.transform.position - transform.position).magnitude
            .CompareTo((enemyUnit2.transform.position - transform.position).magnitude);
        });

        return units;
    }

    public Unit GetClothestEnemyUnitAtDistance(float distance, Team team, Transform transform)
    {
        List<Unit> enemyUnits = GetTeamUnitsInFieldOfViewOrderedByDistance(distance, team, transform);

        if (enemyUnits.Count != 0)
        {
            return enemyUnits[0];
        }

        return null;
    }

    public Unit GetClothestReachableEnemyUnitAtDistance(float distance, Team team, Transform transform, EyeSightManager eyeSightManager)
    {
        List<Unit> enemies = GetTeamUnitsInFieldOfViewOrderedByDistance(
            distance,
            team,
            transform);

        Unit closestEnemy = enemies.Find((enemy) =>
        {
            return eyeSightManager.IsEnemyReachableAtDistance(enemy, distance);
        });

        return closestEnemy;
    }

    public bool IsUnitInTeamVisibleUnits(Team team, Unit unit)
    {
        return GetTeamVisibleUnits(team).Contains(unit);
    }
}
