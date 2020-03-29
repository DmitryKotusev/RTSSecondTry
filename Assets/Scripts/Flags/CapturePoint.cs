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

    [SerializeField]
    [ReadOnly]
    private Team currentCaptureCandidate = null;

    [SerializeField]
    [ReadOnly]
    private float currentCaptureProgress;

    public CapturePointData CapturePointData
    {
        get
        {
            return capturePointData;
        }
    }

    public Team CurrentPointHolder => currentPointHolder;

    private void Awake()
    {
        InitCurrentPointHolder();
    }

    private void Update()
    {
        ProcessCapture();
    }

    private void ProcessCapture()
    {
        IEnumerable<Team> leaderTeams = DetectTeamsInCaptureRadius();

        if (leaderTeams.Count() > 0)
        {
            LeadersPresentCase(leaderTeams);
        }
        else
        {
            LeadersAbsentCase(leaderTeams);
        }
    }

    private void LeadersPresentCase(IEnumerable<Team> leaderTeams)
    {
        if (currentPointHolder == null)
        {
            if (currentCaptureCandidate == null)
            {
                if (leaderTeams.Count() == 1)
                {
                    currentCaptureCandidate = leaderTeams.ElementAt(0);
                    currentCaptureProgress = 0;
                    flagHolder.SetCurrentFlag(currentCaptureCandidate, currentCaptureProgress / capturePointData.CaptureCapacity);
                }
            }
            else
            {
                if (leaderTeams.Contains(currentCaptureCandidate))
                {
                    if (leaderTeams.Count() == 1)
                    {
                        if (Mathf.Abs(currentCaptureProgress - capturePointData.CaptureCapacity) >= Mathf.Epsilon)
                        {
                            currentCaptureProgress = Mathf.Clamp(
                            currentCaptureProgress + capturePointData.CaptureSpeed * Time.deltaTime,
                            0,
                            capturePointData.CaptureCapacity
                            );

                            flagHolder.SetFlagProgress(currentCaptureProgress / capturePointData.CaptureCapacity);

                            if (Mathf.Abs(currentCaptureProgress - capturePointData.CaptureCapacity) < Mathf.Epsilon)
                            {
                                currentPointHolder = currentCaptureCandidate;
                                currentCaptureCandidate = null;
                            }
                        }
                    }
                }
                else
                {
                    if (Mathf.Abs(currentCaptureProgress) >= Mathf.Epsilon)
                    {
                        currentCaptureProgress = Mathf.Clamp(
                        currentCaptureProgress - capturePointData.DischargeSpeed * Time.deltaTime,
                        0,
                        capturePointData.CaptureCapacity
                        );

                        flagHolder.SetFlagProgress(currentCaptureProgress / capturePointData.CaptureCapacity);

                        if (Mathf.Abs(currentCaptureProgress) < Mathf.Epsilon)
                        {
                            currentCaptureCandidate = null;
                            flagHolder.SetCurrentFlag(null, currentCaptureProgress / capturePointData.CaptureCapacity);
                        }
                    }
                }
            }
        }
        else
        {
            if (!leaderTeams.Contains(currentPointHolder))
            {
                if (Mathf.Abs(currentCaptureProgress) >= Mathf.Epsilon)
                {
                    currentCaptureProgress = Mathf.Clamp(
                    currentCaptureProgress - capturePointData.DischargeSpeed * Time.deltaTime,
                    0,
                    capturePointData.CaptureCapacity
                    );

                    flagHolder.SetFlagProgress(currentCaptureProgress / capturePointData.CaptureCapacity);

                    if (Mathf.Abs(currentCaptureProgress) < Mathf.Epsilon)
                    {
                        currentPointHolder = null;
                        flagHolder.SetCurrentFlag(null, currentCaptureProgress / capturePointData.CaptureCapacity);
                    }
                }
            }
        }
    }

    private void LeadersAbsentCase(IEnumerable<Team> leaderTeams)
    {
        if (currentPointHolder == null)
        {
            if (Mathf.Abs(currentCaptureProgress) >= Mathf.Epsilon)
            {
                currentCaptureProgress = Mathf.Clamp(
                currentCaptureProgress - capturePointData.DischargeSpeed * Time.deltaTime,
                0,
                capturePointData.CaptureCapacity
                );

                flagHolder.SetFlagProgress(currentCaptureProgress / capturePointData.CaptureCapacity);

                if (Mathf.Abs(currentCaptureProgress) < Mathf.Epsilon)
                {
                    currentCaptureCandidate = null;
                    flagHolder.SetCurrentFlag(null, currentCaptureProgress / capturePointData.CaptureCapacity);
                }
            }
        }
        else
        {
            if (Mathf.Abs(currentCaptureProgress - capturePointData.CaptureCapacity) >= Mathf.Epsilon)
            {
                currentCaptureProgress = Mathf.Clamp(
                currentCaptureProgress + capturePointData.CaptureSpeed * Time.deltaTime,
                0,
                capturePointData.CaptureCapacity
                );

                flagHolder.SetFlagProgress(currentCaptureProgress / capturePointData.CaptureCapacity);
            }
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
            return new List<Team>();
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
