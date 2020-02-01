using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// Responsible for managing health and death of agent
/// </summary>
public class Health : MonoBehaviour
{
    [SerializeField]
    HealthData healthData;
    public float MaxhealthPoints
    {
        get
        {
            return healthData.MaxHealth;
        }
    }

    [SerializeField]
    float healthPoints = 100;

    [SerializeField]
    [Required]
    DeathManager deathManager;

    [SerializeField]
    [Required]
    Unit unit;

    public float HealthPoints
    {
        get
        {
            return healthPoints;
        }
    }

    private void Start()
    {
        healthPoints = healthData.MaxHealth;
    }

    public void ChangeHealthPoints(float changeAmount, Collider hitedCollider = null)
    {
        float resultChangeAmount = changeAmount;
        if (hitedCollider != null)
        {
            float multiplier = unit.GetHitColliderCost(hitedCollider);
            resultChangeAmount *= multiplier;
        }
        healthPoints = Mathf.Clamp(healthPoints + resultChangeAmount, 0, MaxhealthPoints);

        if (healthPoints == 0)
        {
            Die();
        }
    }

    public void Die()
    {
        deathManager.Die();
    }
}
