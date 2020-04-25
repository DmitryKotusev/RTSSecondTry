using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    [Required]
    private ControllersHub controllersHub;

    [SerializeField]
    [Required]
    private LevelUI levelUI;

    [SerializeField]
    private MatchController matchController = new MatchController();

    private AgentsVisibilitySynchronizer visibilitySynchronizer = new AgentsVisibilitySynchronizer();

    private List<Unit> currentFrameUnits = new List<Unit>();

    static private LevelManager instance;

    static public LevelManager Instance
    {
        get
        {
            if (instance == null)
            {
                // Instantiate prefab
                throw new Exception("Instance call when instance is null");
            }

            return instance;
        }
        private set
        {
            instance = value;
        }
    }

    public TeamsVisibleUnitsStore UnitsStore { get; } = new TeamsVisibleUnitsStore();

    public LevelUI LevelUI => levelUI;

    public List<Unit> GetAllUnits()
    {
        return currentFrameUnits;
    }

    private void Awake()
    {
        if (instance != this)
        {
            if (instance == null)
            {
                instance = this;

                matchController.Awake();

                return;
            }
        }

        Destroy(gameObject);
    }

    private void Update()
    {
        FindAllCurrentFrameUnits();

        visibilitySynchronizer.UpdateUnitsInFieldOfViewOfEachTeam(UnitsStore, controllersHub);

        matchController.Update();
    }

    private void FindAllCurrentFrameUnits()
    {
        controllersHub.GetAllUnits(currentFrameUnits);
    }
}
