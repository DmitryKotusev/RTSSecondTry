using UnityEngine;
using Sirenix.OdinInspector;

abstract public class DeathManager : MonoBehaviour
{
    [SerializeField]
    protected Vector3 localOffset = new Vector3(0, 1, 0);

    [SerializeField]
    [Required]
    protected PooledObject pooledObject;

    abstract public void Die();
}
