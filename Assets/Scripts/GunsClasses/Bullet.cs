using UnityEngine;
using Sirenix.OdinInspector;

public class Bullet : MonoBehaviour
{
    [BoxGroup("Bullet Info")]
    [Tooltip("Speed in toy meters per second")]
    [SerializeField]
    protected int bulletSpeed;

    private void Update()
    {
        CalculateFrameMovement();
    }

    private void CalculateFrameMovement()
    {
        Vector3 currentPosition = transform.position;

        Vector3 nextFramePosition = currentPosition + bulletSpeed * Time.deltaTime * transform.forward;

        Ray nextFramePathRay = new Ray(currentPosition, nextFramePosition - currentPosition);

        RaycastHit raycastHitinfo;

        if (Physics.Raycast(nextFramePathRay, out raycastHitinfo, (nextFramePosition - currentPosition).magnitude))
        {
            if (raycastHitinfo.transform.tag == "Agent")
            {
                // Hit Logic
            }

            Debug.Log(raycastHitinfo.transform.tag);

            // Play hit sound

            // Return bullet to pool
            PoolsManager.GetObjectPool(Poolskeys.m16BulletsPoolKey).ReturnObject(gameObject);
        }
        else
        {
            transform.position = nextFramePosition;
        }
    }
}
