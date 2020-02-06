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
    [Required]
    Team team;

    public void Awake()
    {
        LevelManager.Instance.ControllersHub.RegisterController(this);
        Debug.Log("Controller " + name + " registered");
    }

    public void OnDestroy()
    {
        LevelManager.Instance.ControllersHub.UnregisterController(this);
        Debug.Log("Controller " + name + " unregistered");
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
