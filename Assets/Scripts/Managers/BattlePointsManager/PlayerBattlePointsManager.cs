using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerBattlePointsManager : BattlePointsManager
{
    [SerializeField]
    [Required]
    [BoxGroup("!!!UI events hub!!!")]
    protected UIEventsHub uIEventsHub;

    protected override void Awake()
    {
        if (GetComponent<Controller>() != LevelManager.Instance.LevelUI.PlayerController)
        {
            return;
        }

        LevelManager.Instance.LevelUI.BattlePointPanel.BattlePointsIncomeSpeed = $"+{pointsInfo.BattlePointsIncreaseSpeed}";

        LevelManager.Instance.LevelUI.BattlePointPanel.BattlePointsTotal = $"{(int)currentPointsAmount}/{(int)pointsInfo.BattlePointsLimit}";
    }

    protected override void IncreasePointsAmount()
    {
        currentPointsAmount
            = Mathf.Clamp(currentPointsAmount + Time.deltaTime * pointsInfo.BattlePointsIncreaseSpeed,
            0, pointsInfo.BattlePointsLimit);

        uIEventsHub.TriggerChangeBattlePointsTotal($"{(int)currentPointsAmount}/{(int)pointsInfo.BattlePointsLimit}");
    }
}
