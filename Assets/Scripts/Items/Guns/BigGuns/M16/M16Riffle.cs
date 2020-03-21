using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

public class M16Riffle : BigGun
{
    [Button("Check fire")]
    public override void Fire()
    {
        if (!IsReloadRequired() && timeTillNextShot <= 0)
        {
            ShootBullet();

            IncreaseCurrentSpreadForShot();

            timeTillNextShot = 1 / gunInfo.RateOfFire * 60f;
        }
    }

    private void ShootBullet()
    {
        GameObject bulletGameObject = PoolsManager.GetObjectPool(PoolsKeys.m16BulletsPoolKey).GetObject();
        bulletGameObject.transform.position = roundEmitter.position;
        bulletGameObject.transform.rotation = roundEmitter.rotation;

        Vector3 accurateShotVector = roundEmitter.forward * gunInfo.ProjectileSpeed;
        float randomRotationDegree = Random.Range(0, 360);
        float randomSpreadMagnitude = Random.Range(0, currentSpread);
        Vector3 spreadVector = Quaternion.AngleAxis(randomRotationDegree, accurateShotVector) * roundEmitter.right * randomSpreadMagnitude;
        Vector3 resultVector = spreadVector + accurateShotVector;
        bulletGameObject.transform.rotation = Quaternion.LookRotation(resultVector);

        // Reset tail
        TrailRenderer trailRenderer = bulletGameObject.GetComponent<TrailRenderer>();
        trailRenderer.Clear();

        // Set currentBulletDamage
        Bullet bullet = bulletGameObject.GetComponent<Bullet>();
        bullet.CurrentDamage = gunInfo.BasicDamagePerShot;

        // Set bullet speed
        bullet.BulletSpeed = gunInfo.ProjectileSpeed;

        //Play sound
        //Show effects from the barrel
        GameObject effectGameObject = PoolsManager.GetObjectPool(PoolsKeys.m16ShotEffectsPoolKey).GetObject();
        effectGameObject.transform.position = roundEmitter.position;
        effectGameObject.transform.rotation = roundEmitter.rotation;

        currentClipRoundsLeft--;
    }

    private void IncreaseCurrentSpreadForShot()
    {
        // currentSpread = Mathf.Lerp(currentSpread, gunInfo.MaxSpread, gunInfo.SpreadIncreseSpeed);
        currentSpread = Mathf.Clamp(currentSpread + gunInfo.SpreadIncreseSpeedForShot, gunInfo.Spread, gunInfo.MaxSpread);
    }

    private void Update()
    {
        UpdateTimeTillNextShot();
        UpdateCurrentSpeadIncreaseForMove();
        UpdateCurrentSpeadDeacrease();
    }

    private void UpdateTimeTillNextShot()
    {
        if (timeTillNextShot > 0)
        {
            timeTillNextShot = Mathf.Clamp(timeTillNextShot - Time.deltaTime, 0f, timeTillNextShot);
        }
    }

    private void UpdateCurrentSpeadIncreaseForMove()
    {
        currentSpread = Mathf.Clamp(
            currentSpread + gunInfo.SpreadIncreseSpeedForMove * Time.deltaTime * Mathf.Pow((roundEmitter.position - lastFrameEmitterPosition).magnitude, 2),
            gunInfo.Spread,
            gunInfo.MaxSpread);
        lastFrameEmitterPosition = roundEmitter.position;
    }

    private void UpdateCurrentSpeadDeacrease()
    {
        currentSpread = Mathf.Clamp(currentSpread - gunInfo.SpreadDeacreseSpeed * Time.deltaTime, gunInfo.Spread, gunInfo.MaxSpread);
    }

    private void OnDrawGizmosSelected()
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(roundEmitter.position, roundEmitter.forward, out raycastHit))
        {
            if (gunInfo.FireDistance >= raycastHit.distance)
            {
                Gizmos.color = Color.green;
                Handles.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
                Handles.color = Color.red;
            }

            Vector3 pathVector = raycastHit.point - roundEmitter.position;
            Gizmos.DrawLine(roundEmitter.position, raycastHit.point);
            Handles.DrawWireArc(
                raycastHit.point,
                pathVector,
                roundEmitter.right,
                360,
                pathVector.magnitude / gunInfo.ProjectileSpeed * currentSpread);
        }
    }
}
