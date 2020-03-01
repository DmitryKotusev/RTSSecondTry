using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class TeamsVisibleUnitsStore
{
    private static Dictionary<Team, HashSet<Unit>> teamsVisibleUnits = new Dictionary<Team, HashSet<Unit>>();

    public static void ClearTeamsVisibleUnits()
    {
        foreach (var teamVisibleUnits in teamsVisibleUnits)
        {
            teamsVisibleUnits[teamVisibleUnits.Key].Clear();
        }
    }

    public static void RegisterVisibleUnit(Team team, Unit unit)
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

    public static void RegisterVisibleUnitsRange(Team team, IEnumerable<Unit> units)
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

    public static HashSet<Unit> GetTeamVisibleUnits(Team team)
    {
        if (teamsVisibleUnits.ContainsKey(team))
        {
            return teamsVisibleUnits[team];
        }

        return new HashSet<Unit>();
    }
}
