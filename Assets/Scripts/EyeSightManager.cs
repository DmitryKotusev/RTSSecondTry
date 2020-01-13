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
            if (Physics.Raycast(lookAroundPoint.position, unitsColliderCostPair.collider.transform.position - lookAroundPoint.position, out raycastHit,
                Vector3.Distance(lookAroundPoint.position, unitsColliderCostPair.collider.transform.position)))
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

    public List<Unit> GetEnemyUnitsInFieldOfView(float lookDistance, Team friendlyTeam)
    {
        Collider[] agentsColliders = Physics.OverlapSphere(transform.position, lookDistance, agentsMask);

        HashSet<Unit> enemyUnits = new HashSet<Unit>();

        foreach (Collider agentCollider in agentsColliders)
        {
            Agent agentInfo = agentCollider.attachedRigidbody.GetComponent<Agent>();

            if (agentInfo != null && agentInfo.GetController().GetTeam() != friendlyTeam)
            {
                if (GetUnitsVisibleBodyPart(agentInfo.SoldierBasic) != null)
                {
                    enemyUnits.Add(agentInfo.SoldierBasic);
                }
            }
        }

        return new List<Unit>(enemyUnits);
    }

    public List<Unit> GetEnemyUnitsInFieldOfViewOrderedByDistance(float lookDistance, Team friendlyTeam)
    {
        List<Unit> enemyUnitsList = GetEnemyUnitsInFieldOfView(lookDistance, friendlyTeam);

        enemyUnitsList.Sort((enemyUnit1, enemyUnit2) =>
        {
            return (enemyUnit1.transform.position - transform.position).magnitude
            .CompareTo((enemyUnit2.transform.position - transform.position).magnitude);
        });

        return enemyUnitsList;
    }

    public Unit GetClothestEnemyUnitInFieldOfView(float lookDistance, Team friendlyTeam)
    {
        List<Unit> enemyUnits = GetEnemyUnitsInFieldOfViewOrderedByDistance(lookDistance, friendlyTeam);

        if (enemyUnits.Count != 0)
        {
            return enemyUnits[0];
        }

        return null;
    }
}
