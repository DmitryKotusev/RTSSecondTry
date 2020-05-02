using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class SpawnButton : MonoBehaviour
{
    [SerializeField]
    [Required]
    [BoxGroup("!!!UI events hub!!!")]
    private UIEventsHub uIEventsHub;

    [SerializeField]
    private Button button;

    [SerializeField]
    private RawImage icon;

    [SerializeField]
    private TMP_Text priceText;

    [SerializeField]
    private GameObject grayCover;

    [SerializeField]
    private GameObject redCover;

    private SpawnGroup spawnGroup;

    private bool isBlockedByBattlePoints = false;

    private bool isBlockedByCommandPoints = false;
    public Texture Icon
    {
        get
        {
            return icon.texture;
        }

        set
        {
            icon.texture = value;
        }
    }

    public string Price
    {
        get
        {
            return priceText.text;
        }

        set
        {
            priceText.text = value;
        }
    }

    public SpawnGroup SpawnGroup
    {
        get => spawnGroup;

        set
        {
            spawnGroup = value;

            icon.texture = spawnGroup.SpawnIcon;

            priceText.text = spawnGroup.PointsCost.ToString();
        }
    }

    public float GetGroupSpawnWeight()
    {
        return spawnGroup.CommandPointsCost;
    }

    public float GetGroupSpawnCost()
    {
        return spawnGroup != null ? spawnGroup.PointsCost : 0;
    }

    public void OnButtonPress()
    {
        uIEventsHub.TriggerSpawnButtonPressed(SpawnGroup);
    }

    public void ChangeBattlePointBlockStatus(bool isBlocked)
    {
        isBlockedByBattlePoints = isBlocked;

        // Logic

        if (isBlockedByBattlePoints)
        {
            if (!isBlockedByCommandPoints)
            {
                MarkButtonGray();
                button.enabled = false;
            }
        }
        else
        {
            if (!isBlockedByCommandPoints)
            {
                MarkButtonClear();
                button.enabled = true;
            }
        }
    }

    public void ChangeCommandPointBlockStatus(bool isBlocked)
    {
        isBlockedByCommandPoints = isBlocked;

        // Logic

        if (isBlockedByCommandPoints)
        {
            MarkButtonRed();
            button.enabled = false;
        }
        else
        {
            if (isBlockedByBattlePoints)
            {
                MarkButtonGray();
                button.enabled = false;
            }
            else
            {
                MarkButtonClear();
                button.enabled = true;
            }
        }
    }

    private void MarkButtonClear()
    {
        redCover.SetActive(false);
        grayCover.SetActive(false);
    }

    private void MarkButtonGray()
    {
        redCover.SetActive(false);
        grayCover.SetActive(true);
    }

    private void MarkButtonRed()
    {
        redCover.SetActive(true);
        grayCover.SetActive(false);
    }
}