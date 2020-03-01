using UnityEngine;

public class LevelManager : MonoBehaviour
{
    static public LevelManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        TeamsVisibleUnitsStore.ClearTeamsVisibleUnits();
    }
}
