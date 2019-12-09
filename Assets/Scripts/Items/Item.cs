using UnityEngine;
using Sirenix.OdinInspector;

abstract public class Item : MonoBehaviour
{
    [BoxGroup("Item info")]
    [SerializeField]
    [Tooltip("Items dimension")]
    protected int dimension = 1;
    public int Dimension
    {
        get
        {
            return dimension;
        }
    }
}
