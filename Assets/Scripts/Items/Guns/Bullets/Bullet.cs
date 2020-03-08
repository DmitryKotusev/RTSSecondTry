using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    public float BulletSpeed
    {
        get; set;
    } = 300f;

    protected bool hasReachedTarget = false;
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

    protected virtual void CalculateFrameMovement()
    {
        Vector3 currentPosition = transform.position;

        Vector3 nextFramePosition = currentPosition + BulletSpeed * Time.deltaTime * transform.forward;

        Ray nextFramePathRay = new Ray(currentPosition, nextFramePosition - currentPosition);

        RaycastHit raycastHitinfo;

        if (Physics.Raycast(nextFramePathRay, out raycastHitinfo, (nextFramePosition - currentPosition).magnitude))
        {
            hasReachedTarget = true;
            transform.position = raycastHitinfo.point;
        }
        else
        {
            transform.position = nextFramePosition;
        }
    }

    protected abstract bool CheckIfReachedTarget();
}
