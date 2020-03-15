using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class FlagHolder : MonoBehaviour
{
    [SerializeField]
    [Required]
    private CapsuleCollider capsuleCollider;

    [SerializeField]
    private Vector3 minFlagLocalPosition;

    [SerializeField]
    private Vector3 maxFlagLocalPosition;

    [SerializeField]
    [Required]
    private Flag whiteFlag;

    private Dictionary<Team, Flag> everCapturedTeams = new Dictionary<Team, Flag>();

    private Flag currentActiveFlag;

    public void SetCurrentFlag(Team team, float progress = 1)
    {
        if (team == null)
        {
            return;
        }

        if (everCapturedTeams.ContainsKey(team))
        {
            Flag newFlagInstance = Instantiate(team.FlagPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<Flag>();

            everCapturedTeams.Add(team, newFlagInstance);
        }

        currentActiveFlag = everCapturedTeams[team];

        SetDefaultFlagTransform();

        SetFlagProgress(progress);
    }

    private void Awake()
    {
        InitCurrentFlag();
    }

    private void InitCurrentFlag()
    {
        if (currentActiveFlag == null)
        {
            currentActiveFlag = whiteFlag;
        }

        SetFlagProgress(0);
    }

    private void SetDefaultFlagTransform()
    {
        currentActiveFlag.transform.localPosition = maxFlagLocalPosition;
        currentActiveFlag.transform.localRotation = Quaternion.identity;
    }

    private void SetFlagProgress(float progress)
    {
        float flagProgress = Mathf.Clamp01(progress);

        Vector3 flagPosition = Vector3.Lerp(minFlagLocalPosition, maxFlagLocalPosition, progress);

        currentActiveFlag.transform.localPosition = flagPosition;
    }
}
