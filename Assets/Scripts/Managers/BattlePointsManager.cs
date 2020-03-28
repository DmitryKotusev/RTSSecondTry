using UnityEngine;
using Sirenix.OdinInspector;

public class BattlePointsManager : MonoBehaviour
{
    [SerializeField]
    [Required]
    private PointsInfo pointsInfo;

    [SerializeField]
    [Required]
    private AvailableGroupsInfo availableGroupsInfo;

    private float currentPointsAmount = 0;

    public float CurrentPointsAmount
    {
        get => currentPointsAmount;

        set
        {
            currentPointsAmount = Mathf.Clamp(value, 0, pointsInfo.BattlePointsIncreaseSpeed);
        }
    }

    public PointsInfo PointsInfo => pointsInfo;

    public AvailableGroupsInfo AvailableGroupsInfo => availableGroupsInfo;
}
