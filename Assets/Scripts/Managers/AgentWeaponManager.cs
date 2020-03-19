using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using RootMotion.FinalIK;
using System;

public class AgentWeaponManager : MonoBehaviour
{
    [SerializeField]
    SoldierBasic soldierBasic;

    [SerializeField]
    Gun activeGun;
    public Gun ActiveGun
    {
        get
        {
            return activeGun;
        }
        set
        {
            activeGun = value;
        }
    }

    [SerializeField]
    Helmet activeHelmet;

    [SerializeField]
    FullBodyBipedIK fullBodyBipedIK;

    [SerializeField]
    [Required]
    AgentAimManager agentAimManager;
    public AgentAimManager AgentAimManager
    {
        get
        {
            return agentAimManager;
        }
    }

    [SerializeField]
    [Required]
    AgentReloadManager agentReloadManager;
    public AgentReloadManager AgentReloadManager
    {
        get
        {
            return agentReloadManager;
        }
    }

    [SerializeField]
    AnimatorHandler animatorHandler;

    [SerializeField]
    AimIK aimIK;

    private void Start()
    {
        fullBodyBipedIK.enabled = false;
        AdjustInventory();
    }

    private void LateUpdate()
    {
        agentAimManager.UpdateAimManager();

        AdjustHandsOnWeapons();

        fullBodyBipedIK.solver.Update();
    }

    private void AdjustHandsOnWeapons()
    {
        if (activeGun != null)
        {
            if (activeGun is BigGun)
            {
                Transform leftHandTransform = (activeGun as BigGun).LeftHandTransform;

                fullBodyBipedIK.solver.leftHandEffector.position = leftHandTransform.position;
                fullBodyBipedIK.solver.leftHandEffector.rotation = leftHandTransform.rotation;
                fullBodyBipedIK.solver.leftHandEffector.positionWeight = 1;
                fullBodyBipedIK.solver.leftHandEffector.rotationWeight = 1;
                soldierBasic.LeftHandPoser.poseRoot = leftHandTransform;
            }
        }
        else
        {
            fullBodyBipedIK.solver.leftHandEffector.positionWeight = 0;
            fullBodyBipedIK.solver.leftHandEffector.rotationWeight = 0;
            soldierBasic.LeftHandPoser.poseRoot = null;
        }
    }

    private void AdjustInventory()
    {
        if (activeGun != null)
        {
            soldierBasic.Inventory.AddItem(activeGun);
            ActivateGun();
            animatorHandler.UpdateLayerWeight(animatorHandler.BothHandsRiffleWeaponLayerIndex, 1);
        }
        else
        {
            animatorHandler.UpdateLayerWeight(animatorHandler.BothHandsRiffleWeaponLayerIndex, 0);
        }

        if (activeHelmet != null)
        {
            soldierBasic.Inventory.AddItem(activeHelmet);
            ActivateHelmet();
        }
    }

    private void ActivateGun()
    {
        soldierBasic.Inventory.ActivateItem(activeGun);
        activeGun.transform.SetParent(soldierBasic.WeaponHolder, true);
        activeGun.transform.localPosition = Vector3.zero;
        activeGun.transform.localRotation = Quaternion.Euler(0, 0, 0);
        aimIK.solver.transform = activeGun.RoundEmitter;
    }

    private void ActivateHelmet()
    {
        soldierBasic.Inventory.ActivateItem(activeHelmet);
        activeHelmet.transform.SetParent(soldierBasic.HelmetHolder, true);
        activeHelmet.transform.localPosition = Vector3.zero;
        activeHelmet.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
}
