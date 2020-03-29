using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamScoreCell : MonoBehaviour
{
    [SerializeField]
    private RawImage flagImage;

    [SerializeField]
    private TMP_Text score;

    [SerializeField]
    private Team team;

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

    public Color FontColor
    {
        get
        {
            return score.color;
        }

        set
        {
            score.color = value;
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

    public Team Team
    {
        get
        {
            return team;
        }

        set
        {
            team = value;

            Flag = team.FlagTexture;

            FontColor = team.TeamColor;
        }
    }
}
