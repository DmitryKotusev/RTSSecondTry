using UnityEngine;
using Sirenix.OdinInspector;

public class Flag : MonoBehaviour
{
    [SerializeField]
    [Required]
    private Cloth cloth;

    public Cloth Cloth
    {
        get
        {
            return cloth;
        }
    }
}
