using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EyeSightManager : MonoBehaviour
{
    [SerializeField]
    Transform lookAroundPoint;

    [SerializeField]
    [Tooltip("Agents mask used to create raycast sphere")]
    LayerMask agentsMask;
    /// <summary>
    /// Returns the visible part with the highest prioraty if there is one
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public ColliderCostPair GetUnitsVisibleBodyPart(Unit unit)
    {
        if (unit == null)
        {
            return null;
        }

        List<ColliderCostPair> unitsColliderCostPairs = unit.GetHitCollidersCosts();

        List<ColliderCostPair> colliderCostPairsInSight = unitsColliderCostPairs.FindAll((unitsColliderCostPair) =>
        {
            RaycastHit raycastHit;
            if (Physics.Raycast(lookAroundPoint.position,
                unitsColliderCostPair.collider.transform.position - lookAroundPoint.position,
                out raycastHit,
                (unitsColliderCostPair.collider.transform.position - lookAroundPoint.position).magnitude))
            {
                // Debug.DrawRay(lookAroundPoint.position, unitsColliderCostPair.collider.transform.position - lookAroundPoint.position, Color.white, 1f);
                if (raycastHit.collider.transform == unitsColliderCostPair.collider.transform)
                {
                    return true;
                }
            }

            return false;
        });

        if (colliderCostPairsInSight.Count == 0)
        {
            return null;
        }

        float maxPriority = colliderCostPairsInSight.Max(colliderCostPair => colliderCostPair.GetPriority());
        var maxPriorityColliderCostPair = colliderCostPairsInSight.First(colliderCostPair => colliderCostPair.GetPriority() == maxPriority);

        return maxPriorityColliderCostPair;
    }

    public bool IsEnemyReachableAtDistance(Unit unit, float distance)
    {
        return (transform.position - unit.transform.position).sqrMagnitude < Mathf.Pow(distance, 2);
    }

    public List<Unit> GetEnemyUnitsInFieldOfView(float lookDistance, Team friendlyTeam)
    {
        List<Unit> allUnits = LevelManager.Instance.GetAllUnits();

        List<Unit> visibleEnemyUnits = new List<Unit>();

        foreach (Unit enemyUnit in allUnits)
        {
            if (enemyUnit.Agent.GetTeam() == friendlyTeam)
            {
                continue;
            }

            foreach (ColliderCostPair colliderCostPair in enemyUnit.GetHitCollidersCosts())
            {
                RaycastHit raycastHit;
                if (Physics.Raycast(lookAroundPoint.position,
                    colliderCostPair.collider.transform.position - lookAroundPoint.position,
                    out raycastHit,
                    lookDistance))
                {
                    if (raycastHit.collider.transform == colliderCostPair.collider.transform)
                    {
                        visibleEnemyUnits.Add(enemyUnit);

                        break;
                    }
                }
            }
        }

        return visibleEnemyUnits;
    }

    public List<Unit> GetEnemyUnitsInFieldOfViewOrderedByDistance(float distance, Team friendlyTeam)
    {
        List<Unit> enemyUnitsList = GetEnemyUnitsInFieldOfView(distance, friendlyTeam);

        enemyUnitsList.Sort((enemyUnit1, enemyUnit2) =>
        {
            return (enemyUnit1.transform.position - transform.position).magnitude
            .CompareTo((enemyUnit2.transform.position - transform.position).magnitude);
        });

        return enemyUnitsList;
    }

    public Unit GetClothestEnemyUnitInFieldOfView(float distance, Team friendlyTeam)
    {
        List<Unit> enemyUnits = GetEnemyUnitsInFieldOfViewOrderedByDistance(distance, friendlyTeam);

        if (enemyUnits.Count != 0)
        {
            return enemyUnits[0];
        }

        return null;
    }
}
