using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using System;

public class CapturePoint : MonoBehaviour
{
    [SerializeField]
    private LayerMask agentsMask;

    [SerializeField]
    [Required]
    private FlagHolder flagHolder;

    [SerializeField]
    [Required]
    private CapturePointData capturePointData;

    [SerializeField]
    private Team startPointHolder;

    [SerializeField]
    [ReadOnly]
    private Team currentPointHolder = null;

    private Team currentCaptureCandidate = null;

    private float currentCaptureProgress;

    public CapturePointData CapturePointData
    {
        get
        {
            return capturePointData;
        }
    }

    private void Awake()
    {
        InitCurrentPointHolder();
    }

    private void Update()
    {
        // Detect teams in circle
        IEnumerable<Team> leaderTeams = DetectTeamsInCaptureRadius();

        if (leaderTeams != null)
        {
            LeadersPresentCase(leaderTeams);

            return;
        }

        LeadersAbsentCase();
    }

    private void LeadersPresentCase(IEnumerable<Team> leaderTeams)
    {
        int leaderTeamsCount = leaderTeams.Count();

        if (leaderTeams.Contains(currentPointHolder))
        {
            if (leaderTeamsCount > 1)
            {
                return;
            }

            currentCaptureCandidate = leaderTeams.ElementAt(0);

            IncreaseCurrentCaptureProgress();

            return;
        }

        if (leaderTeamsCount > 0)
        {
            if (currentPointHolder != null)
            {
                DecreaseCurrentCaptureProgress();

                return;
            }

            if (leaderTeamsCount > 1)
            {
                return;
            }

            Team leaderTeam = leaderTeams.ElementAt(0);

            if (currentCaptureCandidate != null && currentCaptureCandidate != leaderTeam)
            {
                DecreaseCurrentCaptureProgress();

                CheckCurrentCaptureCandidate();

                return;
            }

            currentCaptureCandidate = leaderTeam;

            IncreaseCurrentCaptureProgress();
        }
    }

    private void CheckCurrentCaptureCandidate()
    {
        if (Mathf.Abs(currentCaptureProgress) < Mathf.Epsilon)
        {
            currentCaptureCandidate = null;
        }
    }

    private void LeadersAbsentCase()
    {
        DecreaseCurrentCaptureProgress();

        CheckCurrentCaptureCandidate();
    }

    private void IncreaseCurrentCaptureProgress()
    {
        currentCaptureProgress = Mathf.Clamp(
            currentCaptureProgress + capturePointData.CaptureSpeed * Time.deltaTime,
            0,
            capturePointData.CaptureCapacity
            );

        flagHolder.SetCurrentFlag(
            currentCaptureCandidate,
            currentCaptureProgress / capturePointData.CaptureCapacity
            );

        if (currentCaptureCandidate == currentPointHolder)
        {
            return;
        }

        if (Mathf.Abs(currentCaptureProgress - capturePointData.CaptureCapacity) < Mathf.Epsilon)
        {
            currentPointHolder = currentCaptureCandidate;
        }
    }

    private void DecreaseCurrentCaptureProgress()
    {
        currentCaptureProgress = Mathf.Clamp(
            currentCaptureProgress - capturePointData.DischargeSpeed * Time.deltaTime,
            0,
            capturePointData.CaptureCapacity
            );

        flagHolder.SetCurrentFlag(
            currentCaptureCandidate,
            currentCaptureProgress / capturePointData.CaptureCapacity
            );

        if (Mathf.Abs(currentCaptureProgress) < Mathf.Epsilon)
        {
            currentPointHolder = null;
        }
    }

    private void InitCurrentPointHolder()
    {
        currentPointHolder = startPointHolder;

        flagHolder.SetCurrentFlag(currentPointHolder);

        InitCurrentCaptureProgress();
    }

    private void InitCurrentCaptureProgress()
    {
        if (currentPointHolder != null)
        {
            currentCaptureProgress = capturePointData.CaptureCapacity;

            return;
        }

        currentCaptureProgress = 0;
    }

    private IEnumerable<Team> DetectTeamsInCaptureRadius()
    {
        RaycastHit[] hits = Physics.CapsuleCastAll(
            transform.position + capturePointData.DetectionCapsuleHeight * transform.up,
            transform.position - capturePointData.DetectionCapsuleHeight * transform.up,
            capturePointData.CaptureRadius,
            transform.forward,
            0,
            agentsMask);

        List<KeyValuePair<Team, int>> detectedTeamsCounts = new List<KeyValuePair<Team, int>>();

        HashSet<Agent> passedAgents = new HashSet<Agent>();

        int leaderTeamUnitsCount = 0;

        foreach (RaycastHit hit in hits)
        {
            Agent agent = hit.collider?.attachedRigidbody?.GetComponent<Agent>();

            if (agent == null)
            {
                continue;
            }

            if (passedAgents.Contains(agent))
            {
                continue;
            }

            passedAgents.Add(agent);

            Team agentsTeam = agent.GetTeam();
            int teamIndex = -1;

            for (int i = 0; i < detectedTeamsCounts.Count; i++)
            {
                if (detectedTeamsCounts[i].Key == agentsTeam)
                {
                    teamIndex = i;
                    break;
                }
            }

            if (teamIndex == -1)
            {
                detectedTeamsCounts.Add(new KeyValuePair<Team, int>(agentsTeam, 1));

                if (1 > leaderTeamUnitsCount)
                {
                    leaderTeamUnitsCount = 1;
                }

                continue;
            }

            detectedTeamsCounts[teamIndex]
                = new KeyValuePair<Team, int>(agentsTeam, detectedTeamsCounts[teamIndex].Value + 1);

            if (detectedTeamsCounts[teamIndex].Value > leaderTeamUnitsCount)
            {
                leaderTeamUnitsCount = detectedTeamsCounts[teamIndex].Value;
            }
        }

        if (detectedTeamsCounts.Count == 0)
        {
            return null;
        }

        IEnumerable<Team> leaderTeams = detectedTeamsCounts.FindAll((detectedTeamsCount) =>
        {
            return detectedTeamsCount.Value == leaderTeamUnitsCount;
        }).Select((leaderTeamsCount) =>
        {
            return leaderTeamsCount.Key;
        });

        return leaderTeams;
    }
}
