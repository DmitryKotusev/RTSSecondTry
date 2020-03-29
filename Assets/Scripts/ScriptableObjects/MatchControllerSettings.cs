using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CustomScriptables/MatchControllerSettings")]
public class MatchControllerSettings : ScriptableObject
{
    [SerializeField]
    private float pointsIncomePerPointSpeed = 0.5f;

    public float PointsIncomePerPointSpeed => pointsIncomePerPointSpeed;
}
