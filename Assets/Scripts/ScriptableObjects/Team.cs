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

    [SerializeField]
    [Required]
    GameObject aiControllerPrefab;

    private List<Controller> availableControllers = new List<Controller>();

    private Controller SpawnAIController()
    {
        GameObject aiController = Instantiate(aiControllerPrefab, Vector3.zero, Quaternion.identity);
        Controller controller = aiController.GetComponent<Controller>();
        return controller;
    }

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

    public void RegisterController(Controller controller)
    {
        availableControllers.Add(controller);
    }

    public void UnregisterController(Controller controller)
    {
        availableControllers.Remove(controller);
    }

    public void FindControllerForAgent(Agent agent)
    {
        //if ()
        //{

        //}
    }
}
