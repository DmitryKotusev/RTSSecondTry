using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "CustomScriptables/PointsInfo")]
public class PointsInfo : ScriptableObject
{
    [SerializeField]
    private float battlePointsLimit = 5000;

    [SerializeField]
    private float commandPointsLimit = 100;

    [SerializeField]
    private float battlePointsIncreaseSpeed = 10f;

    public float BattlePointsLimit => battlePointsLimit;

    public float CommandPointsLimit => commandPointsLimit;

    public float BattlePointsIncreaseSpeed => battlePointsIncreaseSpeed;
}
