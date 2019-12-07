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
    Frustum frustumMeshBuilder;

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


    // Selection box variables
    #region
    private bool leftMousePhysicsUp = false;
    private bool leftShiftPhysics = false;

    private HashSet<GameObject> latestDetectedSelectionBoxObjects = new HashSet<GameObject>();
    // private bool wasOnTriggerAbleToBeCalled = false;
    #endregion

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
        CheckSelectionWithSingleClick();
    }

    private void FixedUpdate()
    {
        CheckSelectionWithRect();
    }

    private void CheckSelectionWithRect()
    {
        if (leftMousePhysicsUp)
        {
            if (Vector3.Distance(startMousePosition, finishMousePosition) <= selectionBoxAccuracy)
            {
                leftMousePhysicsUp = false;
                leftShiftPhysics = false;
                return;
            }

            if (meshCollider.enabled)
            {
                SelectionWithRect();
                leftMousePhysicsUp = false;
                leftShiftPhysics = false;
                meshCollider.enabled = false;
                // Clear collider hashset
                latestDetectedSelectionBoxObjects.Clear();
            }
            else
            {
                BuildColliderMesh();
                meshCollider.enabled = true;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        latestDetectedSelectionBoxObjects.Add(other.attachedRigidbody.gameObject);
    }

    private void OnGUI()
    {
        rectDrawer.DrawRect(selectionBoxAccuracy, playersCamera);
    }

    private void RegisterKeyBoardTriggers()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            leftShiftPhysics = true;
        }
        else
        {
            leftShiftPhysics = false;
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
            leftMousePhysicsUp = true;
            RegisterKeyBoardTriggers();
        }
    }

    private void SelectionWithRect()
    {
        if (!leftShiftPhysics)
        {
            ClearCurrentSelection();
            FormNewFormationFromBoxSelection();
        }
        else
        {
            if (currentFormation != null)
            {
                AddNewAgentsToCurrentFormation();
            }
            else
            {
                FormNewFormationFromBoxSelection();
            }
        }
        Debug.Log("Available formations count:" + availableFormations.Count);
    }

    private void AddNewAgentsToCurrentFormation()
    {
        List<Agent> agentsList = new List<Agent>();

        foreach (var selectionBoxObject in latestDetectedSelectionBoxObjects)
        {
            if (selectionBoxObject.tag == "Selectable")
            {
                Agent agent = selectionBoxObject.GetComponent<Agent>();
                if (GetComponent<Controller>() == agent.GetController())
                {
                    agentsList.Add(agent);
                }
            }
        }

        if (agentsList.Count == 0)
        {
            return;
        }

        foreach (var agent in agentsList)
        {
            if (!currentFormation.DoesBelongToFormation(agent))
            {
                Formation oldAgentsFormation = agent.GetCurrentFormation();
                oldAgentsFormation?.RemoveAgentFromFormation(agent);
                if (oldAgentsFormation != null && oldAgentsFormation.IsEmpty())
                {
                    availableFormations.Remove(oldAgentsFormation);
                }
                currentFormation.AddAgentToFormation(agent);
            }
        }
        MarkAgentsAsSelected(currentFormation);
    }

    private void FormNewFormationFromBoxSelection()
    {
        // Logic
        List<Agent> agentsList = new List<Agent>();

        foreach (var selectionBoxObject in latestDetectedSelectionBoxObjects)
        {
            if (selectionBoxObject.tag == "Selectable")
            {
                Agent agent = selectionBoxObject.GetComponent<Agent>();
                if (GetComponent<Controller>() == agent.GetController())
                {
                    agentsList.Add(agent);
                }
            }
        }

        if (agentsList.Count == 0)
        {
            return;
        }

        Formation maxAgentsFormation = GetMaxAgentsFormation(agentsList);

        if (maxAgentsFormation == null)
        {
            maxAgentsFormation = new Formation();
            availableFormations.Add(maxAgentsFormation);
        }

        if (DoesAgentsListContainWholeFormation(agentsList, maxAgentsFormation))
        {
            foreach (var agent in agentsList)
            {
                if (!maxAgentsFormation.DoesBelongToFormation(agent))
                {
                    Formation oldAgentsFormation = agent.GetCurrentFormation();
                    oldAgentsFormation?.RemoveAgentFromFormation(agent);
                    if (oldAgentsFormation != null && oldAgentsFormation.IsEmpty())
                    {
                        availableFormations.Remove(oldAgentsFormation);
                    }

                    maxAgentsFormation.AddAgentToFormation(agent);
                }
            }
            currentFormation = maxAgentsFormation;
            MarkAgentsAsSelected(currentFormation);
            return;
        }

        Formation newCurrentFormation = new Formation();
        foreach (var agent in agentsList)
        {
            Formation oldAgentsFormation = agent.GetCurrentFormation();
            oldAgentsFormation?.RemoveAgentFromFormation(agent);
            if (oldAgentsFormation != null && oldAgentsFormation.IsEmpty())
            {
                availableFormations.Remove(oldAgentsFormation);
            }
            newCurrentFormation.AddAgentToFormation(agent);
        }
        currentFormation = newCurrentFormation;
        availableFormations.Add(newCurrentFormation);
        MarkAgentsAsSelected(currentFormation);
    }

    private void BuildColliderMesh()
    {
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
        if (equalToCameraFarDistance)
        {
            playersCamera.CalculateFrustumCorners(selectRect, playersCamera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, farClipCorners);
        }
        else
        {
            playersCamera.CalculateFrustumCorners(selectRect, Mathf.Clamp(selectionBoxColliderDistance, playersCamera.nearClipPlane, playersCamera.farClipPlane),
                Camera.MonoOrStereoscopicEye.Mono, farClipCorners);
        }

        List<Vector3> farClipCornersWorld = new List<Vector3>(farClipCorners.Select((localPoint) =>
        {
            return playersCamera.transform.TransformPoint(localPoint);
        }));

        frustumMeshBuilder.SetNearPlanePoints(nearClipCornersWorld);
        frustumMeshBuilder.SetFarPlanePoints(farClipCornersWorld);
        frustumMeshBuilder.GenerateNewMesh();

        meshCollider.sharedMesh = frustumMeshBuilder.FrustumMesh;
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
                if (GetComponent<Controller>() != agent.GetController())
                {
                    ClearCurrentSelection();
                }
                else
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        ModifyCurrentFormationSingleClick(agent);
                    }
                    else
                    {
                        ClearCurrentSelection();
                        ModifyCurrentFormationSingleClick(agent);
                    }
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
        Debug.Log("Available formations count:" + availableFormations.Count);
    }

    private void ModifyCurrentFormationSingleClick(Agent newAgent)
    {
        // Find agents old formation
        Formation agentsOldFormation = newAgent.GetCurrentFormation();
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

    private Formation GetMaxAgentsFormation(List<Agent> agentsList)
    {
        Dictionary<Formation, int> formationsAgentsCounts = new Dictionary<Formation, int>();

        foreach (var agent in agentsList)
        {
            if (agent.GetCurrentFormation() != null)
            {
                if (formationsAgentsCounts.ContainsKey(agent.GetCurrentFormation()))
                {
                    formationsAgentsCounts[agent.GetCurrentFormation()]++;
                }
                else
                {
                    formationsAgentsCounts.Add(agent.GetCurrentFormation(), 1);
                }
            }
        }

        Formation maxAgentsFormation = null;

        int maxCount = -1;
        foreach (var formation in formationsAgentsCounts.Keys)
        {
            if (formationsAgentsCounts[formation] > maxCount)
            {
                maxCount = formationsAgentsCounts[formation];
                maxAgentsFormation = formation;
            }
        }

        return maxAgentsFormation;
    }

    private bool DoesAgentsListContainWholeFormation(List<Agent> agentsList, Formation maxAgentsFormation)
    {
        return maxAgentsFormation.GetAllFormationAgents().All((agent) =>
        {
            return agentsList.Contains(agent);
        });
    }

    private void MarkAgentsAsSelected(Formation currentFormation)
    {
        currentFormation.GetFormationLeader()?.MarkAsMainSelected();

        foreach(var agent in currentFormation.GetFormationAgentsWithoutLeader())
        {
            agent.MarkAsSelected();
        }
    }
}
