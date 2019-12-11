using UnityEngine;
using Sirenix.OdinInspector;

public class Bullet : MonoBehaviour
{
    [BoxGroup("Bullet Info")]
    [Tooltip("Speed in toy meters per second")]
    [SerializeField]
    protected int bulletSpeed;

    bool hasReachedTarget = false;
    public float CurrentDamage
    {
        get; set;
    } = 10f;

    private void OnEnable()
    {
        hasReachedTarget = false;
    }

    private void Update()
    {
        if (!CheckIfReachedTarget())
        {
            CalculateFrameMovement();
        }
    }

    private void CalculateFrameMovement()
    {
        Vector3 currentPosition = transform.position;

        Vector3 nextFramePosition = currentPosition + bulletSpeed * Time.deltaTime * transform.forward;

        Ray nextFramePathRay = new Ray(currentPosition, nextFramePosition - currentPosition);

        RaycastHit raycastHitinfo;

        if (Physics.Raycast(nextFramePathRay, out raycastHitinfo, (nextFramePosition - currentPosition).magnitude))
        {
            if (raycastHitinfo.transform.tag == "Selectable")
            {
                Health healthSystem = raycastHitinfo.transform.GetComponent<Health>();
                healthSystem?.ChangeHealthPoints(-CurrentDamage, raycastHitinfo.collider);
            }

            // Play hit sound

            hasReachedTarget = true;
            transform.position = raycastHitinfo.point;
        }
        else
        {
            transform.position = nextFramePosition;
        }
    }

    bool CheckIfReachedTarget()
    {
        if (hasReachedTarget)
        {
            // Return bullet to pool
            PoolsManager.GetObjectPool(Poolskeys.m16BulletsPoolKey).ReturnObject(gameObject);
        }

        return hasReachedTarget;
    }
}
