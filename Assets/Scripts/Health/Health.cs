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
    [Min(1)]
    float maxhealthPoints = 100;
    public float MaxhealthPoints
    {
        get
        {
            return maxhealthPoints;
        }
    }

    [SerializeField]
    float healthPoints = 100;
    public float HealthPoints
    {
        get
        {
            return healthPoints;
        }
    }

    public void ChangeHealthPoints(float changeAmount)
    {
        healthPoints = Mathf.Clamp(healthPoints + changeAmount, 0, maxhealthPoints);
    }

    public void Die()
    {
        // Destroy agent
        // Show death FX
    }
}
