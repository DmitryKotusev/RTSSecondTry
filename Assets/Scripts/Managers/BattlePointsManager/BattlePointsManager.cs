using UnityEngine;
using Sirenix.OdinInspector;
using System;

public abstract class BattlePointsManager : MonoBehaviour
{
    [SerializeField]
    [Required]
    protected PointsInfo pointsInfo;

    protected float currentBattlePointsAmount = 0;

    protected float currentCommandPointsAmount = 0;

    public virtual float CurrentBattlePointsAmount
    {
        get => currentBattlePointsAmount;

        set
        {
            currentBattlePointsAmount = Mathf.Clamp(value, 0, pointsInfo.BattlePointsLimit);
        }
    }

    public virtual float CurrentCommandPointsAmount
    {
        get => currentCommandPointsAmount;

        set
        {
            currentCommandPointsAmount = Mathf.Clamp(value, 0, pointsInfo.CommandPointsLimit);
        }
    }

    public PointsInfo PointsInfo => pointsInfo;

    protected virtual void Awake()
    {
        CurrentBattlePointsAmount = 0;

        CurrentCommandPointsAmount = 0;
    }

    protected virtual void Update()
    {
        IncreasePointsAmount();
    }

    protected virtual void IncreasePointsAmount()
    {
        currentBattlePointsAmount
            = Mathf.Clamp(currentBattlePointsAmount + Time.deltaTime * pointsInfo.BattlePointsIncreaseSpeed,
            0, pointsInfo.BattlePointsLimit);
    }
}
