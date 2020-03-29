using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class LevelUI : MonoBehaviour
{
    [SerializeField]
    [Required]
    [BoxGroup("!!!UI events hub!!!")]
    private UIEventsHub uIEventsHub;

    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    [Required]
    private ScorePanel scorePanel;

    [SerializeField]
    [Required]
    private SpawnMenu spawnMenu;

    [SerializeField]
    [Required]
    private BattlePointPanel battlePointPanel;

    public PlayerController PlayerController
    {
        get => playerController;

        set
        {
            playerController = value;
        }
    }

    public ScorePanel ScorePanel => scorePanel;

    public SpawnMenu SpawnMenu => spawnMenu;

    public BattlePointPanel BattlePointPanel => battlePointPanel;

    public void OnSpawnButtonPressed(SpawnGroup spawnGroup)
    {
        playerController?.Spawner.SpawnGroup(spawnGroup);
    }

    private void OnEnable()
    {
        uIEventsHub.SpawnButtonPressed += OnSpawnButtonPressed;
    }

    private void OnDisable()
    {
        uIEventsHub.SpawnButtonPressed -= OnSpawnButtonPressed;
    }
}
