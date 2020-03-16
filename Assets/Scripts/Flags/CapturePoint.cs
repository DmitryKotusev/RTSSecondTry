using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

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
        List<Team> currentLeaderTeams;
        bool isLeaderTeamSingle;


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
            Vector3.zero,
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
            else
            {
                passedAgents.Add(agent);
            }

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
            }
            else
            {
                detectedTeamsCounts[teamIndex] = new KeyValuePair<Team, int>(agentsTeam, detectedTeamsCounts[teamIndex].Value + 1);
            }

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

#if UNITY_EDITOR
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.white;
    //    Gizmos.DrawWireSphere(transform.position, capturePointData.CaptureRadius);
    //}
#endif
}
