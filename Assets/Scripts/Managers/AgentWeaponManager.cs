using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class AgentWeaponManager : MonoBehaviour
{
    [SerializeField]
    SoldierBasic soldierBasic;

    [SerializeField]
    Gun activeGun;

    [SerializeField]
    Helmet activeHelmet;

    private void Start()
    {
        AdjustInventory();
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
    }

    private void ActivateHelmet()
    {
        soldierBasic.Inventory.ActivateItem(activeHelmet);
        activeHelmet.transform.SetParent(soldierBasic.HelmetHolder, true);
        activeHelmet.transform.localPosition = Vector3.zero;
        activeHelmet.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
}
