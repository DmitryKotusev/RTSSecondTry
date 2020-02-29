using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Assets/Prefabs/Scriptables/SelectionManagerSettings/SelectionManagerSettings", menuName = "CustomScriptables/SelectionManagerSettings")]
public class SelectionManagerSettings : ScriptableObject
{
    [BoxGroup("Settings")]
    [SerializeField]
    [Tooltip("Selection box accuracy")]
    private float selectionBoxAccuracy = 0.05f;

    [BoxGroup("Settings")]
    [SerializeField]
    [Tooltip("Does  frustrum collider distance equal to camera far clip distance")]
    private bool equalToCameraFarDistance = false;

    [BoxGroup("Settings")]
    [SerializeField]
    [Tooltip("Selection distance")]
    [HideIf("equalToCameraFarDistance")]
    private float selectionDistance = 50;

    public float SelectionBoxAccuracy
    {
        get
        {
            return selectionBoxAccuracy;
        }
    }

    public bool EqualToCameraFarDistance
    {
        get
        {
            return equalToCameraFarDistance;
        }
    }

    public float SelectionDistance
    {
        get
        {
            return selectionDistance;
        }
    }
}
