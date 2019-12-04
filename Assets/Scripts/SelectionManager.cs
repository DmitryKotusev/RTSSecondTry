using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class SelectionManager : MonoBehaviour
{
    [BoxGroup("Settings")]
    [SerializeField]
    [Tooltip("Selection box accuracy")]
    float selectionBoxAccuracy = 0.05f;

    [SerializeField]
    Camera playersCamera;

    [Tooltip("Units available for selection")]
    [SerializeField]
    List<Agent> availableAgents = new List<Agent>();

    RectDrawer rectDrawer = new RectDrawer();
    Vector3 startMousePosition;
    Vector3 finishMousePosition;

    Formation currentFormation = null;
    List<Formation> availableFormations = new List<Formation>();

    // Getters and setters
    #region
    public Formation GetCurrentFormation()
    {
        return currentFormation;
    }
    #endregion

    void Update()
    {
        rectDrawer.MouseClickControl();
        RegisterMousePositions();
        // SelectWithRect(); -> Goes to FixedUpdate probably, needs investigation
        CheckSelectionWithSingleClick();
    }

    private void OnGUI()
    {
        rectDrawer.DrawRect(selectionBoxAccuracy, playersCamera);
    }

    private void RegisterMousePositions()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startMousePosition = playersCamera.ScreenToViewportPoint(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0))
        {
            finishMousePosition = playersCamera.ScreenToViewportPoint(Input.mousePosition);
        }
    }

    private void CheckSelectionWithSingleClick()
    {
        if (!(Vector3.Distance(startMousePosition, finishMousePosition) <= selectionBoxAccuracy))
        {
            return;
        }

        if (!Input.GetMouseButtonUp(0))
        {
            return;
        }

        RaycastHit raycastHit;
        if (Physics.Raycast(playersCamera.ScreenPointToRay(Input.mousePosition), out raycastHit, Mathf.Infinity))
        {
            if (raycastHit.transform.tag == "Selectable")
            {
                Agent agent = raycastHit.transform.GetComponent<Agent>();
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    ModifyCurrentFormation(agent);
                }
                else
                {
                    ClearCurrentSelection();
                    ModifyCurrentFormation(agent);
                }
            }
            else
            {
                ClearCurrentSelection();
            }
        }
        else
        {
            ClearCurrentSelection();
        }
    }

    private void ModifyCurrentFormation(Agent newAgent)
    {
        // Find agents old formation
        Formation agentsOldFormation = null;
        foreach(Formation availableFormation in availableFormations)
        {
            if (availableFormation.DoesBelongToFormation(newAgent))
            {
                agentsOldFormation = availableFormation;
                break;
            }
        }
        if (agentsOldFormation == null)
        {
            Debug.Log("Selected agents had no formations!");
            agentsOldFormation = new Formation(newAgent);
            availableFormations.Add(agentsOldFormation);
        }

        // If current formation == oldformation then remove agent from currentformation
        // else remove agent from old formation and add to current

        // CurrentFormation != null && agentsOldFormation == currentFormation
        if (agentsOldFormation == currentFormation)
        {
            if (currentFormation.IsLeader(newAgent))
            {
                // Do nothing, leader is leader)
            }
            else
            {
                currentFormation.RemoveAgentFromFormation(newAgent);
                Formation agentsNewFormation = new Formation(newAgent);
                availableFormations.Add(agentsNewFormation);
                newAgent.MarkAsUnselected();
            }
        }
        // CurrentFormation == null
        else if (currentFormation == null)
        {
            if (agentsOldFormation.GetAllFormationAgentsCount() == 1)
            {
                // Only assign current formation
                currentFormation = agentsOldFormation;
            }
            else
            {
                agentsOldFormation.RemoveAgentFromFormation(newAgent);
                Formation agentsNewFormation = new Formation(newAgent);
                availableFormations.Add(agentsNewFormation);
                currentFormation = agentsNewFormation;
            }
            newAgent.MarkAsMainSelected();
        }
        // CurrentFormation != null && agentsOldFormation != currentFormation
        else
        {
            agentsOldFormation.RemoveAgentFromFormation(newAgent);
            if (agentsOldFormation.IsEmpty())
            {
                availableFormations.Remove(agentsOldFormation);
            }
            currentFormation.AddAgentToFormation(newAgent);
            newAgent.MarkAsSelected();
        }
    }

    void ClearCurrentSelection()
    {
        if (currentFormation != null)
        {
            foreach (Agent agent in currentFormation)
            {
                agent.MarkAsUnselected();
            }
            currentFormation = null;
        }
    }
}
