
using UnityEngine;
using Sirenix.OdinInspector;

public class TestRotationScript : MonoBehaviour
{
    [Button("Inverse")]
    public void Inverse()
    {
        transform.rotation = Quaternion.Inverse(transform.rotation);
    }

    [Button("Identify")]
    public void Identify()
    {
        transform.rotation = Quaternion.identity;
    }
}
