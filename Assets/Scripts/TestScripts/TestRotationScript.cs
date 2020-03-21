
using UnityEngine;
using Sirenix.OdinInspector;

public class TestRotationScript : MonoBehaviour
{
    [SerializeField]
    private Transform child;

    [SerializeField]
    private Quaternion desiredChildGlobalRotation;

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
    
    [Button("Check")]
    public void CheckForwardVector()
    {
        Debug.Log(
            $"Is transform forward equal to" +
            $"rotation * Vector.forward: {transform.forward == transform.rotation * Vector3.forward}"
            );
    }

    [Button("Check 2")]
    public void CheckRotations()
    {
        Debug.Log(
            $"Is transform forward equal to" +
            $"rotation * Vector.forward: {child.rotation == transform.rotation * child.localRotation}"
            );
    }

    [Button("Check 3")]
    public void CheckChildRotation()
    {
        transform.rotation = desiredChildGlobalRotation * Quaternion.Inverse(child.localRotation);
        Debug.Log(child.rotation.eulerAngles);
    }
}
