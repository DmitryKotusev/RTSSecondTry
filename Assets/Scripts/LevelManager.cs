using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private ControllersHub controllersHub = new ControllersHub();

    static public LevelManager Instance { get; private set; }

    public ControllersHub ControllersHub
    {
        get
        {
            return controllersHub;
        }
    }

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
}
