using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "CustomScriptables/Team")]
public class Team : ScriptableObject
{
    [SerializeField]
    private string teamName;

    [SerializeField]
    private Color teamColor;

    [SerializeField]
    [Required]
    private GameObject flagPrefab;

    [SerializeField]
    [Required]
    private Texture2D flagTexture;

    public string TeamName => teamName;

    public Color TeamColor => teamColor;

    public GameObject FlagPrefab => flagPrefab;

    public Texture2D FlagTexture => flagTexture;
}
