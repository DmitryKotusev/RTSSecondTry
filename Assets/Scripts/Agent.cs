using UnityEngine;
using Sirenix.OdinInspector;

public class Agent : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Projector used to highlight if unit is in selection list")]
    GameObject selectionProjector;

    [SerializeField]
    [Tooltip("Projector used to highlight if unit is main in selection list")]
    GameObject mainSelectionProjector;

    [SerializeField]
    [Tooltip("Controller that is able to give commands to this unit")]
    Controller controller;

    // Getters and setters
    #region
    public void SetController(Controller controller)
    {
        this.controller = controller;
    }

    public Controller GetController()
    {
        return controller;
    }
    #endregion

    public void MarkAsSelected()
    {
        selectionProjector.SetActive(true);
    }

    public void MarkAsMainSelected()
    {
        mainSelectionProjector.SetActive(true);
    }

    public void MarkAsUnselected()
    {
        selectionProjector.SetActive(false);
        mainSelectionProjector.SetActive(false);
    }
}
