using UnityEngine;
using Sirenix.OdinInspector;

public class SelectionMarker : MonoBehaviour
{
    [BoxGroup("Projectors")]
    [SerializeField]
    [Tooltip("Projector used to highlight if unit is in selection list")]
    [Required]
    private GameObject selectionProjector;

    [BoxGroup("Projectors")]
    [SerializeField]
    [Tooltip("Projector used to highlight if unit is main in selection list")]
    [Required]
    private GameObject mainSelectionProjector;

    public void MarkAsSelected()
    {
        mainSelectionProjector.SetActive(false);
        selectionProjector.SetActive(true);
    }

    public void MarkAsMainSelected()
    {
        selectionProjector.SetActive(false);
        mainSelectionProjector.SetActive(true);
    }

    public void MarkAsUnselected()
    {
        selectionProjector.SetActive(false);
        mainSelectionProjector.SetActive(false);
    }
}
