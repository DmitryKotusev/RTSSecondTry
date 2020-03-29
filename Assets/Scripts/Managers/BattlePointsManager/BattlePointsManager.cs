using UnityEngine;
using Sirenix.OdinInspector;
using System;

public abstract class BattlePointsManager : MonoBehaviour
{
    [SerializeField]
    [Required]
    protected PointsInfo pointsInfo;

    protected float currentPointsAmount = 0;

    public float CurrentPointsAmount
    {
        get => currentPointsAmount;

        set
        {
            currentPointsAmount = Mathf.Clamp(value, 0, pointsInfo.BattlePointsIncreaseSpeed);
        }
    }

    public PointsInfo PointsInfo => pointsInfo;

    protected virtual void Awake()
    {
        if (GetComponent<Controller>() != LevelManager.Instance.LevelUI.PlayerController)
        {
            return;
        }
    }

    protected virtual void Update()
    {
        IncreasePointsAmount();
    }

    protected virtual void IncreasePointsAmount()
    {
        currentPointsAmount
            = Mathf.Clamp(currentPointsAmount + Time.deltaTime * pointsInfo.BattlePointsIncreaseSpeed,
            0, pointsInfo.BattlePointsLimit);
    }
}
