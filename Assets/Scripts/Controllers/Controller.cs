﻿using System.Collections;
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
    [SerializeField]
    [Required]
    private BattlePointsManager battlePointsManager;

    [SerializeField]
    [Required]
    private Spawner spawner;

    [SerializeField]
    [Required]
    private Team team;

    [SerializeField]
    [Tooltip("Controllers' hub where controller should register when game starts")]
    [Required]
    private ControllersHub controllersHub;

    public virtual void Awake()
    {
        controllersHub.RegisterController(this);
        Debug.Log("Controller " + name + " registered");
    }

    public virtual void OnDestroy()
    {
        controllersHub.UnregisterController(this);
        Debug.Log("Controller " + name + " unregistered");
    }

    public BattlePointsManager BattlePointsManager => battlePointsManager;

    public Spawner Spawner
    {
        get => spawner;

        set
        {
            spawner = value;
        }
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

    public virtual IAgentsHandler GetAgentsHandler()
    {
        return null;
    }
}
