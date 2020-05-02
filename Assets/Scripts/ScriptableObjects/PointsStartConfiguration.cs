using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "CustomScriptables/PointsStartConfiguration")]
public class PointsStartConfiguration : ScriptableObject
{
    [SerializeField]
    private float battlePointsStartAmount = 200f;

    public float BattlePointsStartAmount => battlePointsStartAmount;
}
