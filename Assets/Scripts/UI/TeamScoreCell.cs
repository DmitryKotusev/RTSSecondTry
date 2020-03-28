using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamScoreCell : MonoBehaviour
{
    [SerializeField]
    private RawImage flagImage;

    [SerializeField]
    private TMP_Text score;

    public Texture Flag
    {
        get
        {
            return flagImage.texture;
        }

        set
        {
            flagImage.texture = value;
        }
    }

    public string Score
    {
        get
        {
            return score.text;
        }

        set
        {
            score.text = value;
        }
    }
}
