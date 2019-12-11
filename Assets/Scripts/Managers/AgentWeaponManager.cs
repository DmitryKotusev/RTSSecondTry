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

    [SerializeField]
    Helmet activeHelmet;

    [SerializeField]
    BipedIK bipedIK;

    [SerializeField]
    AgentAimManager agentAimManager;

    [SerializeField]
    AnimatorHandler animatorHandler;

    private void Start()
    {
        bipedIK.enabled = false;
        AdjustInventory();
    }

    private void LateUpdate()
    {
        agentAimManager.UpdateAimManager();

        AdjustHandsOnWeapons();

        bipedIK.UpdateBipedIK();
    }

    private void AdjustHandsOnWeapons()
    {
        if (activeGun != null)
        {
            if (activeGun is BigGun)
            {
                Transform leftHandTransform = (activeGun as BigGun).LeftHandTransform;

                bipedIK.solvers.leftHand.IKPosition = leftHandTransform.position;
                bipedIK.solvers.leftHand.IKRotation = leftHandTransform.rotation;
                bipedIK.solvers.leftHand.IKPositionWeight = 1;
                bipedIK.solvers.leftHand.IKRotationWeight = 1;
            }
        }
        else
        {
            bipedIK.solvers.leftHand.IKPositionWeight = 0;
            bipedIK.solvers.leftHand.IKRotationWeight = 0;
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
    }

    private void ActivateHelmet()
    {
        soldierBasic.Inventory.ActivateItem(activeHelmet);
        activeHelmet.transform.SetParent(soldierBasic.HelmetHolder, true);
        activeHelmet.transform.localPosition = Vector3.zero;
        activeHelmet.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
}
