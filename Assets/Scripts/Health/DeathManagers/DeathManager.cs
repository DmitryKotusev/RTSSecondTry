using UnityEngine;
using Sirenix.OdinInspector;

public class DeathManager : MonoBehaviour
{
    [SerializeField]
    protected Vector3 localOffset = new Vector3(0, 1, 0);

    [SerializeField]
    [Required]
    protected Agent agent;

    [SerializeField]
    protected string deathParticlesKey = "";

    public void Die()
    {
        GameObject particlesObject = PoolsManager.GetObjectPool(deathParticlesKey)?.GetObject();
        if (particlesObject != null)
        {
            particlesObject.transform.position = transform.position + transform.InverseTransformVector(localOffset);
        }

        Destroy(gameObject);
    }
}
