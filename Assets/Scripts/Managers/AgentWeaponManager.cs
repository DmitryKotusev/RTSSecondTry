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
    AimIK aimIK;

    [SerializeField]
    FullBodyBipedIK fullBodyBipedIK;

    private void Start()
    {
        aimIK.enabled = false;
        fullBodyBipedIK.enabled = false;
        AdjustInventory();
    }

    private void LateUpdate()
    {
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
                // fullBodyBipedIK.solver.leftHandEffector.rotation = leftHandTransform.rotation;
                // fullBodyBipedIK.solver.leftHandEffector.positionWeight = 1;
                // fullBodyBipedIK.solver.leftHandEffector.rotationWeight = 1;
            }
        }
        else
        {
            fullBodyBipedIK.solver.leftHandEffector.positionWeight = 0;
            fullBodyBipedIK.solver.leftHandEffector.rotationWeight = 0;
        }
    }

    private void AdjustInventory()
    {
        if (activeGun != null)
        {
            soldierBasic.Inventory.AddItem(activeGun);
            ActivateGun();
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
