using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Assets/Prefabs/Scriptables/Teams/Team", menuName = "CustomScriptables/Team")]
public class Team : ScriptableObject
{
    [SerializeField]
    string teamName;

    [SerializeField]
    Color teamColor;

    public string TeamName
    {
        get
        {
            return teamName;
        }
    }

    public Color TeamColor
    {
        get
        {
            return teamColor;
        }
    }
}
