using UnityEngine;
using Sirenix.OdinInspector;

public class M16Bullet : Bullet
{
    [SerializeField]
    [Required]
    Team greenTeam;

    [SerializeField]
    [Required]
    Team tanTeam;

    protected override void CalculateFrameMovement()
    {
        Vector3 currentPosition = transform.position;

        Vector3 nextFramePosition = currentPosition + BulletSpeed * Time.deltaTime * transform.forward;

        Ray nextFramePathRay = new Ray(currentPosition, nextFramePosition - currentPosition);

        RaycastHit raycastHitinfo;

        if (Physics.Raycast(nextFramePathRay, out raycastHitinfo, (nextFramePosition - currentPosition).magnitude))
        {
            HandleHealthChange(raycastHitinfo);

            HandleAgentHit(raycastHitinfo);

            hasReachedTarget = true;
            transform.position = raycastHitinfo.point;
        }
        else
        {
            transform.position = nextFramePosition;
        }
    }

    private void HandleAgentHit(RaycastHit raycastHitinfo)
    {
        Agent agent = raycastHitinfo.transform.GetComponent<Agent>();

        if (agent == null)
        {
            return;
        }

        ShowHitAgentEffect(raycastHitinfo, agent);
    }

    private void ShowHitAgentEffect(RaycastHit raycastHitinfo, Agent agent)
    {
        GameObject shotEffectGameObject = null;
        if (agent.GetTeam() == greenTeam)
        {
            shotEffectGameObject = PoolsManager.GetObjectPool(PoolsKeys.m16RifleGreenSoldierHitEffectPoolKey).GetObject();
        }
        else if (agent.GetTeam() == tanTeam)
        {
            shotEffectGameObject = PoolsManager.GetObjectPool(PoolsKeys.m16RifleTanSoldierHitEffectPoolKey).GetObject();
        }

        if (shotEffectGameObject == null)
        {
            return;
        }

        shotEffectGameObject.transform.position = raycastHitinfo.point;
        shotEffectGameObject.transform.rotation = Quaternion.LookRotation(raycastHitinfo.normal);
        shotEffectGameObject.transform.SetParent(raycastHitinfo.collider.transform, true);
    }

    private void HandleHealthChange(RaycastHit raycastHitinfo)
    {
        Health healthSystem = raycastHitinfo.transform.GetComponent<Health>();
        healthSystem?.ChangeHealthPoints(-CurrentDamage, raycastHitinfo.collider);
    }

    protected override bool CheckIfReachedTarget()
    {
        if (hasReachedTarget)
        {
            // Return bullet to pool
            PoolsManager.GetObjectPool(PoolsKeys.m16BulletsPoolKey).ReturnObject(gameObject);
        }

        return hasReachedTarget;
    }
}
