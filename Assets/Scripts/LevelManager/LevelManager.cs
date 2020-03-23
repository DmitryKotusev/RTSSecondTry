using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    [Required]
    private ControllersHub controllersHub;

    private AgentsVisibilitySynchronizer visibilitySynchronizer = new AgentsVisibilitySynchronizer();

    private List<Unit> currentFrameUnits = new List<Unit>();

    static private LevelManager instance;

    static public LevelManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject newInstance = new GameObject();

                instance = newInstance.AddComponent<LevelManager>();
            }

            return instance;
        }
        private set
        {
            instance = value;
        }
    }

    public TeamsVisibleUnitsStore UnitsStore { get; } = new TeamsVisibleUnitsStore();

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

                return;
            }
        }

        Destroy(gameObject);
    }

    private void Update()
    {
        FindAllCurrentFrameUnits();

        visibilitySynchronizer.UpdateUnitsInFieldOfViewOfEachTeam(UnitsStore, controllersHub);
    }

    private void FindAllCurrentFrameUnits()
    {
        controllersHub.GetAllUnits(currentFrameUnits);
    }
}
