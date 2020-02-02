using UnityEngine;
using Sirenix.OdinInspector;

public class M16Riffle : BigGun
{
    [Button("Check fire")]
    public override void Fire()
    {
        if (timeTillNextShot <= 0)
        {
            ShootBullet();

            timeTillNextShot = 1 / gunInfo.RateOfFire * 60f;
        }
    }

    private void ShootBullet()
    {
        GameObject bulletGameObject = PoolsManager.GetObjectPool(Poolskeys.m16BulletsPoolKey).GetObject();
        bulletGameObject.transform.position = roundEmitter.position;
        bulletGameObject.transform.rotation = roundEmitter.rotation;

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
        GameObject effectGameObject = PoolsManager.GetObjectPool(Poolskeys.m16ShotEffectsPoolKey).GetObject();
        effectGameObject.transform.position = roundEmitter.position;
        effectGameObject.transform.rotation = roundEmitter.rotation;
        ShotEffect shotEffect = effectGameObject.GetComponent<ShotEffect>();
        shotEffect.ShowEffect();
    }

    private void Update()
    {
        UpdateTimeTillNextShot();
    }

    private void UpdateTimeTillNextShot()
    {
        if (timeTillNextShot > 0)
        {
            timeTillNextShot = Mathf.Clamp(timeTillNextShot - Time.deltaTime, 0f, timeTillNextShot);
        }
    }
}
