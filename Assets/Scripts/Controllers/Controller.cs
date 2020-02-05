using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// Basic class for controller entity.
/// It can either be human player or
/// AI.
/// </summary>
abstract public class Controller : MonoBehaviour
{
    static HashSet<short> controllerIdsPool = new HashSet<short>();

    [SerializeField]
    Team team;

    public void Awake()
    {
    }

    // Getters and setters
    #region
    public void SetTeam(Team team)
    {
        this.team = team;
    }

    public Team GetTeam()
    {
        return team;
    }
    #endregion
}
