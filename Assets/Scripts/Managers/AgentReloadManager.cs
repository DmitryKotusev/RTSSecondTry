using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class AgentReloadManager : MonoBehaviour
{
    [SerializeField]
    [Required]
    private AnimatorHandler animatorHandler;

    [SerializeField]
    [Required]
    private AgentWeaponManager weaponManager;

    private float animationSpeedMultiplier = 1;

    private float leftPrepareTime;
    private float wholePrepareTime;

    private float leftReloadTime;
    private float wholeReloadTime;

    private float leftFinishTime;
    private float wholeFinishTime;

    private float leftStartTakingClipTime;
    private float wholeStartTakingClipTime;

    private float leftFinishTakingClipTime;
    private float wholeFinishTakingClipTime;

    private float leftStartReturningClipTime;
    private float wholeStartReturningClipTime;

    private float leftFinishReturningClipTime;
    private float wholeFinishReturningClipTime;

    public bool IsReloading => enabled;

    public void StartReloading()
    {
        enabled = true;

        GunInfo activeGunInfo = weaponManager.ActiveGun.GunInfo;

        leftPrepareTime = activeGunInfo.Cooldown * activeGunInfo.CooldownPreparationRatio;
        wholePrepareTime = leftPrepareTime;

        leftReloadTime = activeGunInfo.Cooldown
            * (1 - activeGunInfo.CooldownPreparationRatio - activeGunInfo.CooldownFinishRatio);
        wholeReloadTime = leftReloadTime;

        leftFinishTime = activeGunInfo.Cooldown * activeGunInfo.CooldownFinishRatio;
        wholeFinishTime = leftFinishTime;

        animationSpeedMultiplier = activeGunInfo.ReloadAnimation.length / activeGunInfo.Cooldown;
        animatorHandler.SetReloadAnimationMultiplier(animationSpeedMultiplier);

        animatorHandler.PlayAnimation("RifleReloading", animatorHandler.UpperBodyReloadLayer);
    }

    public void FinishReloading()
    {
        enabled = false;
        animatorHandler.UpdateLayerWeight(animatorHandler.UpperBodyReloadLayer,
                0);

        leftPrepareTime = -1;

        leftReloadTime = -1;

        leftFinishTime = -1;
    }

    private void LateUpdate()
    {
        ReloadActiveGun();

        weaponManager.FullBodyBipedIK.solver.Update();
    }

    private void ReloadActiveGun()
    {
        if (leftPrepareTime > 0)
        {
            leftPrepareTime -= Time.deltaTime;

            animatorHandler.UpdateLayerWeight(animatorHandler.UpperBodyReloadLayer,
                (wholePrepareTime - leftPrepareTime) / wholePrepareTime);

            if (weaponManager.ActiveGun is BigGun)
            {
                Transform leftHandTransform = (weaponManager.ActiveGun as BigGun).LeftHandTransform;

                float weight = leftPrepareTime / wholePrepareTime;

                weaponManager.FullBodyBipedIK.solver.leftHandEffector.position = leftHandTransform.position;
                weaponManager.FullBodyBipedIK.solver.leftHandEffector.rotation = leftHandTransform.rotation;
                weaponManager.FullBodyBipedIK.solver.leftHandEffector.positionWeight = weight;
                weaponManager.FullBodyBipedIK.solver.leftHandEffector.rotationWeight = weight;
                weaponManager.SoldierBasic.LeftHandPoser.localPositionWeight = weight;
                weaponManager.SoldierBasic.LeftHandPoser.localRotationWeight = weight;
                weaponManager.SoldierBasic.LeftHandPoser.poseRoot = leftHandTransform;
            }

            return;
        }

        if (leftReloadTime > 0)
        {
            leftReloadTime -= Time.deltaTime;

            if (!(weaponManager.ActiveGun is BigGun))
            {
                return;
            }

            BigGun bigGun = weaponManager.ActiveGun as BigGun;

            if (bigGun.CatridgeClip == null)
            {
                return;
            }

            Vector3 catridgeHandLocalPosition = bigGun.GunInfo.UsualClipHandInfo.ClipHandPosition;
            Quaternion catridgeHandLocalRotation = bigGun.GunInfo.UsualClipHandInfo.ClipHandRotation;
            Vector3 leftHandLocalScale = bigGun.CatridgeClip.parent.localScale;

            Vector3 catridgeDesiredGlobalPosition
                = bigGun.CatridgeClipHolder.transform.TransformPoint(bigGun.СatridgeDefaultLocalPosition);

            Quaternion catridgeDesiredGlobalRotation
                = bigGun.CatridgeClipHolder.transform.rotation * bigGun.CatridgeDefaultLocalRotation;


            Quaternion leftHandDesiredGlobalRotation
                = catridgeDesiredGlobalRotation * Quaternion.Inverse(catridgeHandLocalRotation);

            Vector3 leftHandDesiredGlobalPosition
                = catridgeDesiredGlobalPosition
                - leftHandDesiredGlobalRotation * Vector3.right * catridgeHandLocalPosition.x / leftHandLocalScale.x
                - leftHandDesiredGlobalRotation * Vector3.up * catridgeHandLocalPosition.y / leftHandLocalScale.y
                - leftHandDesiredGlobalRotation * Vector3.forward * catridgeHandLocalPosition.z / leftHandLocalScale.z;

            if (leftStartTakingClipTime > 0)
            {
                leftStartTakingClipTime -= Time.deltaTime;

                weaponManager.FullBodyBipedIK.solver.leftHandEffector.position
                    = leftHandDesiredGlobalPosition;
                weaponManager.FullBodyBipedIK.solver.leftHandEffector.rotation
                    = leftHandDesiredGlobalRotation;

                weaponManager.FullBodyBipedIK.solver.leftHandEffector.positionWeight
                    = (wholeStartTakingClipTime - leftStartTakingClipTime) / wholeStartTakingClipTime;
                weaponManager.FullBodyBipedIK.solver.leftHandEffector.rotationWeight
                    = (wholeStartTakingClipTime - leftStartTakingClipTime) / wholeStartTakingClipTime;

                return;
            }

            if (leftFinishTakingClipTime > 0)
            {
                leftFinishTakingClipTime -= Time.deltaTime;

                weaponManager.FullBodyBipedIK.solver.leftHandEffector.position
                    = leftHandDesiredGlobalPosition;
                weaponManager.FullBodyBipedIK.solver.leftHandEffector.rotation
                    = leftHandDesiredGlobalRotation;

                weaponManager.FullBodyBipedIK.solver.leftHandEffector.positionWeight
                    = leftFinishTakingClipTime / wholeFinishTakingClipTime;
                weaponManager.FullBodyBipedIK.solver.leftHandEffector.rotationWeight
                    = leftFinishTakingClipTime / wholeFinishTakingClipTime;
            }

            if (leftStartReturningClipTime > 0)
            {
                leftStartReturningClipTime -= Time.deltaTime;

                weaponManager.FullBodyBipedIK.solver.leftHandEffector.position
                    = leftHandDesiredGlobalPosition;
                weaponManager.FullBodyBipedIK.solver.leftHandEffector.rotation
                    = leftHandDesiredGlobalRotation;

                weaponManager.FullBodyBipedIK.solver.leftHandEffector.positionWeight
                    = (wholeStartReturningClipTime - leftStartReturningClipTime) / wholeStartReturningClipTime;
                weaponManager.FullBodyBipedIK.solver.leftHandEffector.rotationWeight
                    = (wholeStartReturningClipTime - leftStartReturningClipTime) / wholeStartReturningClipTime;

                return;
            }

            if (leftFinishReturningClipTime > 0)
            {
                leftFinishReturningClipTime -= Time.deltaTime;

                weaponManager.FullBodyBipedIK.solver.leftHandEffector.position
                    = leftHandDesiredGlobalPosition;
                weaponManager.FullBodyBipedIK.solver.leftHandEffector.rotation
                    = leftHandDesiredGlobalRotation;

                weaponManager.FullBodyBipedIK.solver.leftHandEffector.positionWeight
                    = leftFinishReturningClipTime / wholeFinishReturningClipTime;
                weaponManager.FullBodyBipedIK.solver.leftHandEffector.rotationWeight
                    = leftFinishReturningClipTime / wholeFinishReturningClipTime;
            }

            return;
        }

        if (leftFinishTime > 0)
        {
            leftFinishTime -= Time.deltaTime;

            animatorHandler.UpdateLayerWeight(animatorHandler.UpperBodyReloadLayer,
                leftFinishTime / wholeFinishTime);

            if (weaponManager.ActiveGun is BigGun)
            {
                Transform leftHandTransform = (weaponManager.ActiveGun as BigGun).LeftHandTransform;

                float weight = (wholeFinishTime - leftFinishTime) / wholeFinishTime;

                weaponManager.FullBodyBipedIK.solver.leftHandEffector.position = leftHandTransform.position;
                weaponManager.FullBodyBipedIK.solver.leftHandEffector.rotation = leftHandTransform.rotation;
                weaponManager.FullBodyBipedIK.solver.leftHandEffector.positionWeight = weight;
                weaponManager.FullBodyBipedIK.solver.leftHandEffector.rotationWeight = weight;
                weaponManager.SoldierBasic.LeftHandPoser.localPositionWeight = weight;
                weaponManager.SoldierBasic.LeftHandPoser.localRotationWeight = weight;
                weaponManager.SoldierBasic.LeftHandPoser.poseRoot = leftHandTransform;
            }

            return;
        }

        Gun activeGun = weaponManager.ActiveGun;

        activeGun.CurrentClipRoundsLeft = activeGun.GunInfo.BulletsPerClip;

        FinishReloading();
    }

    private void OnStartTakingClip()
    {
        Gun activeGun = weaponManager.ActiveGun;

        if (!(activeGun is BigGun))
        {
            return;
        }

        BigGun bigGun = activeGun as BigGun;

        if (bigGun.CatridgeClip == null)
        {
            return;
        }

        AnimationEvent currentEvent = bigGun.GunInfo.ReloadAnimation.events[0];

        AnimationEvent nextEvent = bigGun.GunInfo.ReloadAnimation.events[1];

        wholeStartTakingClipTime = (nextEvent.time - currentEvent.time) / animationSpeedMultiplier;

        leftStartTakingClipTime = wholeStartTakingClipTime;
    }

    private void OnTakingClip()
    {
        Gun activeGun = weaponManager.ActiveGun;

        if (!(activeGun is BigGun))
        {
            return;
        }

        BigGun bigGun = activeGun as BigGun;

        if (bigGun.CatridgeClip == null)
        {
            return;
        }

        AnimationEvent currentEvent = bigGun.GunInfo.ReloadAnimation.events[1];

        AnimationEvent nextEvent = bigGun.GunInfo.ReloadAnimation.events[2];

        wholeFinishTakingClipTime = nextEvent.time - currentEvent.time;

        leftFinishTakingClipTime = wholeFinishTakingClipTime;

        bigGun.CatridgeClip.SetParent(weaponManager.SoldierBasic.LeftHandPoser.transform, true);
        bigGun.CatridgeClip.localPosition = bigGun.GunInfo.UsualClipHandInfo.ClipHandPosition;
        bigGun.CatridgeClip.localRotation = bigGun.GunInfo.UsualClipHandInfo.ClipHandRotation;
    }

    private void OnFinishTakingClip() { }

    private void OnStartReturningClip()
    {
        Gun activeGun = weaponManager.ActiveGun;

        if (!(activeGun is BigGun))
        {
            return;
        }

        BigGun bigGun = activeGun as BigGun;

        if (bigGun.CatridgeClip == null)
        {
            return;
        }

        AnimationEvent currentEvent = bigGun.GunInfo.ReloadAnimation.events[3];

        AnimationEvent nextEvent = bigGun.GunInfo.ReloadAnimation.events[4];

        wholeStartReturningClipTime = (nextEvent.time - currentEvent.time) / animationSpeedMultiplier;

        leftStartReturningClipTime = wholeStartReturningClipTime;
    }

    private void OnReturningClip()
    {
        Gun activeGun = weaponManager.ActiveGun;

        if (!(activeGun is BigGun))
        {
            return;
        }

        BigGun bigGun = activeGun as BigGun;

        if (bigGun.CatridgeClip == null)
        {
            return;
        }

        AnimationEvent currentEvent = bigGun.GunInfo.ReloadAnimation.events[4];

        AnimationEvent nextEvent = bigGun.GunInfo.ReloadAnimation.events[5];

        wholeFinishReturningClipTime = (nextEvent.time - currentEvent.time) / animationSpeedMultiplier;

        leftFinishReturningClipTime = wholeFinishReturningClipTime;

        bigGun.CatridgeClip.SetParent(bigGun.CatridgeClipHolder, true);
        bigGun.CatridgeClip.localPosition = bigGun.СatridgeDefaultLocalPosition;
        bigGun.CatridgeClip.localRotation = bigGun.CatridgeDefaultLocalRotation;
    }

    private void OnFinishReturningClip() { }
}
