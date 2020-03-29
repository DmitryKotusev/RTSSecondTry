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

    private SpawnGroup spawnGroup;

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

    public void OnButtonPress()
    {
        uIEventsHub.TriggerSpawnButtonPressed(SpawnGroup);
    }
}
