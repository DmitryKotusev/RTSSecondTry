using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

public class ScorePanel : MonoBehaviour
{
    [SerializeField]
    [Required]
    [BoxGroup("!!!UI events hub!!!")]
    private UIEventsHub uIEventsHub;

    [SerializeField]
    private GameObject teamCellPrefab;

    private List<TeamScoreCell> teamScoreCells = new List<TeamScoreCell>();

    public void RegisterNewTeamCell(Team team, string score = "0/100")
    {
        GameObject teamCellPrefabInstance = Instantiate(teamCellPrefab, transform);

        TeamScoreCell teamScoreCell = teamCellPrefabInstance.GetComponent<TeamScoreCell>();

        teamScoreCells.Add(teamScoreCell);

        teamScoreCell.Team = team;

        teamScoreCell.Score = score;
    }

    public void OnChangeTeamScore(Team team, string newScore)
    {
        TeamScoreCell teamScoreCell = teamScoreCells.Find(
            (cell) =>
            {
                return cell.Team == team;
            }
            );

        teamScoreCell.Score = newScore;
    }

    private void OnEnable()
    {
        uIEventsHub.ChangeTeamScore += OnChangeTeamScore;
    }

    private void OnDisable()
    {
        uIEventsHub.ChangeTeamScore -= OnChangeTeamScore;
    }
}
