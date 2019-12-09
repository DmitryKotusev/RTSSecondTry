using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using System;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Inventory capacity")]
    protected int capacity = 1;
    public int Capacity
    {
        get
        {
            return capacity;
        }
    }

    [SerializeField]
    [Tooltip("Transforms to show big Guns on character")]
    List<Transform> bigGunsPlaceHoldersTransforms = new List<Transform>();

    [SerializeField]
    [Tooltip("Inventory items")]
    protected List<Item> items = new List<Item>();

    protected HashSet<Item> activeItems = new HashSet<Item>();

    // TODO: Check 
    // Функция активации предмета из инвентаря

    private void Awake()
    {
        CheckFreeBigGunHolders();
    }

    public void CheckFreeBigGunHolders()
    {
        List<Transform> freeBigGunHolders = FindAllFreeBigGunHolders();

        foreach (var freeHolder in freeBigGunHolders)
        {
            Item bigGunWithoutHolder = FindBigGunWithoutHolder();
            if (bigGunWithoutHolder == null)
            {
                break;
            }

            SetHolderForBigGun(bigGunWithoutHolder, freeHolder);
        }
    }

    private Transform FindFreeBigGunHolder()
    {
        return bigGunsPlaceHoldersTransforms.Find((placeHolder) =>
        {
            return placeHolder.childCount == 0;
        });
    }

    private List<Transform> FindAllFreeBigGunHolders()
    {
        return bigGunsPlaceHoldersTransforms.FindAll((placeHolder) =>
        {
            return placeHolder.childCount == 0;
        });
    }

    private void FindFreeBigGunHolderForBigGun(Item item)
    {
        Transform freeBigGunHolder = FindFreeBigGunHolder();

        if (freeBigGunHolder == null)
        {
            return;
        }

        if (item is BigGun)
        {
            SetHolderForBigGun(item, freeBigGunHolder);
        }
    }

    private void SetHolderForBigGun(Item item, Transform freeBigGunHolder)
    {
        item.transform.SetParent(freeBigGunHolder, true);
        item.gameObject.SetActive(true);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    private Transform FindItemsBigGunHolder(Item item)
    {
        return bigGunsPlaceHoldersTransforms.Find((placeHolder) =>
        {
            return item.transform.parent == placeHolder;
        });
    }

    private Item FindBigGunWithoutHolder()
    {
        return items.Find((item) =>
        {
            if (item is BigGun)
            {
                return !activeItems.Contains(item) && FindItemsBigGunHolder(item) == null;
            }

            return false;
        });
    }

    private void FindNewItemForBigGunHolder(Item item)
    {
        if (item is BigGun)
        {
            Transform removedItemPlaceHolder = FindItemsBigGunHolder(item);

            if (removedItemPlaceHolder != null)
            {
                Item bigGunWithoutHolder = FindBigGunWithoutHolder();
                SetHolderForBigGun(bigGunWithoutHolder, removedItemPlaceHolder);
            }
        }
    }

    public bool RemoveItem(Item item)
    {
        item.transform.SetParent(null, true);
        item.gameObject.SetActive(true);
        FindNewItemForBigGunHolder(item);
        if (activeItems.Contains(item))
        {
            DeactivateItem(item);
        }

        return items.Remove(item);
    }

    public List<Item> GetItems()
    {
        return new List<Item>(items);
    }

    public bool IsItemPresent(Item item)
    {
        return items.Contains(item);
    }

    public bool AddItem(Item item)
    {
        if (IsItemPresent(item))
        {
            return false;
        }
        items.Add(item);

        item.gameObject.SetActive(false);
        item.transform.SetParent(transform, true);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.Euler(0, 0, 0);

        FindFreeBigGunHolderForBigGun(item);

        return true;
    }

    public bool ActivateItem(Item item)
    {
        if (IsItemPresent(item) && activeItems.Add(item))
        {
            item.transform.SetParent(null, true);
            item.gameObject.SetActive(true);
            FindNewItemForBigGunHolder(item);

            return true;
        }

        return false;
    }

    public bool DeactivateItem(Item item)
    {
        if (!activeItems.Remove(item))
        {
            return false;
        }

        FindFreeBigGunHolderForBigGun(item);
        return true;
    }
}
