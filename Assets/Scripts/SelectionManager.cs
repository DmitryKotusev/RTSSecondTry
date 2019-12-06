using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.Linq;

public class SelectionManager : MonoBehaviour
{
    [BoxGroup("Settings")]
    [SerializeField]
    [Tooltip("Selection box accuracy")]
    float selectionBoxAccuracy = 0.05f;

    [BoxGroup("Settings")]
    [SerializeField]
    [Tooltip("Does  frustrum collider distance equal to camera far clip distance")]
    bool equalToCameraFarDistance = false;

    [BoxGroup("Settings")]
    [SerializeField]
    [Tooltip("Selection box collider distance")]
    [HideIf("equalToCameraFarDistance")]
    float selectionBoxColliderDistance = 200;

    [SerializeField]
    Camera playersCamera;

    [SerializeField]
    Frustrum frustrumMeshBuilder;

    [SerializeField]
    MeshCollider meshCollider;

    [Tooltip("Units available for selection")]
    [SerializeField]
    List<Agent> availableAgents = new List<Agent>();

    RectDrawer rectDrawer = new RectDrawer();
    Vector3 startMousePosition;
    Vector3 finishMousePosition;
    Vector3 normalizedStartMousePosition;
    Vector3 normalizedFinishMousePosition;

    Formation currentFormation = null;
    List<Formation> availableFormations = new List<Formation>();

    private bool leftMouseButtonUpTriggered = false;
    private bool leftShiftButtonTriggered = false;

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
        RegisterKeyBoardTriggers();
        CheckSelectionWithRect(); // -> Goes to FixedUpdate probably, needs investigation
        CheckSelectionWithSingleClick();
    }

    private void OnGUI()
    {
        rectDrawer.DrawRect(selectionBoxAccuracy, playersCamera);
    }

    private void RegisterKeyBoardTriggers()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            leftShiftButtonTriggered = true;
        }
    }

    private void RegisterMousePositions()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startMousePosition = Input.mousePosition;
            normalizedStartMousePosition = playersCamera.ScreenToViewportPoint(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0))
        {
            finishMousePosition = Input.mousePosition;
            normalizedFinishMousePosition = playersCamera.ScreenToViewportPoint(Input.mousePosition);
            leftMouseButtonUpTriggered = true;
        }
    }

    private void CheckSelectionWithRect()
    {
        if (Vector3.Distance(startMousePosition, finishMousePosition) <= selectionBoxAccuracy)
        {
            // TODO: Cancel triggers
            leftMouseButtonUpTriggered = false;
            leftShiftButtonTriggered = false;
            return;
        }

        if (!leftMouseButtonUpTriggered)
        {
            // TODO: Cancel triggers
            leftMouseButtonUpTriggered = false;
            leftShiftButtonTriggered = false;
            return;
        }

        // TODO: Cancel triggers
        leftMouseButtonUpTriggered = false;
        leftShiftButtonTriggered = false;

        List<GameObject> remObjects = new List<GameObject>();

        if (!leftShiftButtonTriggered)
        {
            ClearCurrentSelection();
        }

        var topLeft = Vector3.Min(normalizedStartMousePosition, normalizedFinishMousePosition);
        var bottomRight = Vector3.Max(normalizedStartMousePosition, normalizedFinishMousePosition);
        Rect selectRect = Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);

        var nearClipCorners = new Vector3[4];
        playersCamera.CalculateFrustumCorners(selectRect, playersCamera.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, nearClipCorners);
        List<Vector3> nearClipCornersWorld = new List<Vector3>(nearClipCorners.Select((localPoint) =>
        {
            return playersCamera.transform.TransformPoint(localPoint);
        }));

        var farClipCorners = new Vector3[4];
        playersCamera.CalculateFrustumCorners(selectRect, playersCamera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, farClipCorners);
        List<Vector3> farClipCornersWorld = new List<Vector3>(farClipCorners.Select((localPoint) =>
        {
            return playersCamera.transform.TransformPoint(localPoint);
        }));

        frustrumMeshBuilder.SetNearPlanePoints(nearClipCornersWorld);
        frustrumMeshBuilder.SetFarPlanePoints(farClipCornersWorld);
        frustrumMeshBuilder.GenerateNewMesh();

        meshCollider.sharedMesh = frustrumMeshBuilder.FrustumMesh;
        // Logic
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
