using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class BattlePointPanel : MonoBehaviour
{
    [SerializeField]
    [Required]
    [BoxGroup("!!!UI events hub!!!")]
    private UIEventsHub uIEventsHub;

    [SerializeField]
    private TMP_Text battlePointsIncomeSpeedText;

    [SerializeField]
    private TMP_Text battlePointsTotalText;

    [SerializeField]
    private TMP_Text commandPointsTotalText;

    public string BattlePointsIncomeSpeed
    {
        get => battlePointsIncomeSpeedText.text;

        set
        {
            battlePointsIncomeSpeedText.text = value;
        }
    }

    public string BattlePointsTotal
    {
        get => battlePointsTotalText.text;

        set
        {
            battlePointsTotalText.text = value;
        }
    }

    public string CommandPointsTotal
    {
        get => commandPointsTotalText.text;

        set
        {
            commandPointsTotalText.text = value;
        }
    }

    public void OnChangeBattlePointsIncomeSpeed(string incomeSpeed)
    {
        BattlePointsIncomeSpeed = incomeSpeed;
    }

    public void OnChangeBattlePointsTotal(int totalBattlePoints, int maxBattlePoints)
    {
        BattlePointsTotal = $"{totalBattlePoints}/{maxBattlePoints}";
    }

    public void OnChangeCommandPointsTotal(int totalCommandPoints, int maxCommandPoints)
    {
        CommandPointsTotal = $"{totalCommandPoints}/{maxCommandPoints}";
    }

    private void OnEnable()
    {
        uIEventsHub.ChangeBattlePointsIncomeSpeed += OnChangeBattlePointsIncomeSpeed;
        uIEventsHub.ChangeBattlePointsTotal += OnChangeBattlePointsTotal;
        uIEventsHub.ChangeCommandPointsTotal += OnChangeCommandPointsTotal;
    }

    private void OnDisable()
    {
        uIEventsHub.ChangeBattlePointsIncomeSpeed -= OnChangeBattlePointsIncomeSpeed;
        uIEventsHub.ChangeBattlePointsTotal -= OnChangeBattlePointsTotal;
        uIEventsHub.ChangeCommandPointsTotal -= OnChangeCommandPointsTotal;
    }
}
