using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerBattlePointsManager : BattlePointsManager
{
    [SerializeField]
    [Required]
    [BoxGroup("!!!UI events hub!!!")]
    protected UIEventsHub uIEventsHub;

    public override float CurrentBattlePointsAmount
    {
        get => currentBattlePointsAmount;

        set
        {
            currentBattlePointsAmount = Mathf.Clamp(value, 0, pointsInfo.BattlePointsLimit);

            uIEventsHub.TriggerChangeBattlePointsTotal((int)currentBattlePointsAmount, (int)pointsInfo.BattlePointsLimit);
        }
    }

    public override float CurrentCommandPointsAmount
    {
        get => currentCommandPointsAmount;

        set
        {
            currentCommandPointsAmount = Mathf.Clamp(value, 0, pointsInfo.CommandPointsLimit);

            uIEventsHub.TriggerChangeCommandPointsTotal((int)currentCommandPointsAmount, (int)pointsInfo.CommandPointsLimit);
        }
    }

    protected override void Awake()
    {
        base.Awake();

        if (GetComponent<Controller>() != LevelManager.Instance.LevelUI.PlayerController)
        {
            return;
        }

        LevelManager.Instance.LevelUI.BattlePointPanel.BattlePointsIncomeSpeed = $"+{pointsInfo.BattlePointsIncreaseSpeed}";

        LevelManager.Instance.LevelUI.BattlePointPanel.BattlePointsTotal = $"{(int)currentBattlePointsAmount}/{(int)pointsInfo.BattlePointsLimit}";

        LevelManager.Instance.LevelUI.BattlePointPanel.CommandPointsTotal = $"{(int)currentCommandPointsAmount}/{(int)pointsInfo.CommandPointsLimit}";
    }

    protected override void IncreasePointsAmount()
    {
        currentBattlePointsAmount
            = Mathf.Clamp(currentBattlePointsAmount + Time.deltaTime * pointsInfo.BattlePointsIncreaseSpeed,
            0, pointsInfo.BattlePointsLimit);

        uIEventsHub.TriggerChangeBattlePointsTotal((int)currentBattlePointsAmount, (int)pointsInfo.BattlePointsLimit);
    }
}
