using UnityEngine;
using Sirenix.OdinInspector;

public class M16Riffle : Gun
{

    [Button("Check fire")]
    public override void Fire()
    {
        if (timeTillNextShot <= 0)
        {
            ShootBullet();

            timeTillNextShot = 1 / rateOfFire * 60f;
        }
    }

    private void ShootBullet()
    {
        GameObject bulletGameObject = PoolsManager.GetObjectPool(Poolskeys.m16BulletsPoolKey).GetObject();
        bulletGameObject.transform.position = roundEmitter.position;
        bulletGameObject.transform.rotation = roundEmitter.rotation;

        TrailRenderer trailRenderer = bulletGameObject.GetComponent<TrailRenderer>();
        trailRenderer.Clear();

        //Play sound

        //Show effects from the barrel
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
