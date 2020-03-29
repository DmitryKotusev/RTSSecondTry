using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using System.Linq;

[Serializable]
public class MatchController
{
    [SerializeField]
    [Required]
    private MatchControllerSettings matchControllerSettings;

    [SerializeField]
    private List<Team> participatingTeams = new List<Team>();

    [SerializeField]
    private float victoryScore = 150f;

    [SerializeField]
    [Required]
    private UIEventsHub uIEventsHub;

    [SerializeField]
    private List<CapturePoint> allCapturePoints = new List<CapturePoint>();

    private Dictionary<Team, float> teamScores = new Dictionary<Team, float>();

    public void Awake()
    {
        foreach (Team team in participatingTeams)
        {
            teamScores.Add(team, 0);

            LevelManager.Instance.LevelUI.ScorePanel.RegisterNewTeamCell(team, $"{0}/{victoryScore}");
        }
    }

    public void Update()
    {
        CheckTeamsCaptureProgress();

        CheckEndMatch();
    }

    public void AddPoints(Team team, float points)
    {
        if (!participatingTeams.Contains(team))
        {
            participatingTeams.Add(team);
        }

        if (!teamScores.ContainsKey(team))
        {
            teamScores.Add(team, points);
        }
        else
        {
            teamScores[team] += points;
        }
    }

    private void CheckTeamsCaptureProgress()
    {
        Dictionary<Team, int> teamsCapturePoints = new Dictionary<Team, int>();

        int maxPoints = 0;

        foreach (CapturePoint capturePoint in allCapturePoints)
        {
            if (capturePoint.CurrentPointHolder == null)
            {
                continue;
            }

            if (!teamsCapturePoints.ContainsKey(capturePoint.CurrentPointHolder))
            {
                teamsCapturePoints.Add(capturePoint.CurrentPointHolder, 1);
            }
            else
            {
                teamsCapturePoints[capturePoint.CurrentPointHolder]++;
            }

            if (teamsCapturePoints[capturePoint.CurrentPointHolder] > maxPoints)
            {
                maxPoints = teamsCapturePoints[capturePoint.CurrentPointHolder];
            }
        }

        if (maxPoints == 0)
        {
            return;
        }

        List<Team> maxPointsTeams = new List<Team>();

        List<int> capturePointsDeltas = new List<int>() { maxPoints };

        foreach (KeyValuePair<Team, int> teamCapturePoints in teamsCapturePoints)
        {
            if (teamCapturePoints.Value == maxPoints)
            {
                maxPointsTeams.Add(teamCapturePoints.Key);
            }
            else
            {
                capturePointsDeltas.Add(maxPoints - teamCapturePoints.Value);
            }
        }

        if (maxPointsTeams.Count != 1)
        {
            return;
        }

        int capturePointDelta = capturePointsDeltas.Min();

        // Signal change UI score display
        teamScores[maxPointsTeams[0]]
            = Mathf.Clamp(
            teamScores[maxPointsTeams[0]]
            + Time.deltaTime * matchControllerSettings.PointsIncomePerPointSpeed * capturePointDelta,
            0,
            victoryScore
            );

        uIEventsHub.TriggerChangeTeamScore(maxPointsTeams[0], $"{(int)teamScores[maxPointsTeams[0]]}/{victoryScore}");
    }

    private void CheckEndMatch()
    {

    }
}
