using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.Linq;

public class SelectionManager : MonoBehaviour, IAgentsHandler
{
    [BoxGroup("Settings")]
    [SerializeField]
    [Tooltip("Selection box accuracy")]
    private float selectionBoxAccuracy = 0.05f;

    [BoxGroup("Settings")]
    [SerializeField]
    [Tooltip("Does  frustrum collider distance equal to camera far clip distance")]
    private bool equalToCameraFarDistance = false;

    [BoxGroup("Settings")]
    [SerializeField]
    [Tooltip("Selection distance")]
    [HideIf("equalToCameraFarDistance")]
    private float selectionDistance = 50;

    [SerializeField]
    [Required]
    private Camera playersCamera;

    //[Tooltip("Units available for selection")]
    //[SerializeField]
    //List<Agent> availableAgents = new List<Agent>();

    private RectDrawer rectDrawer;
    private Vector3 startMousePosition;
    private Vector3 finishMousePosition;
    private Vector3 normalizedStartMousePosition;
    private Vector3 normalizedFinishMousePosition;

    private Formation currentFormation = null;
    private List<Formation> availableFormations = new List<Formation>();

    private void Start()
    {
        rectDrawer = new RectDrawer(this);
    }

    // Selection box variables
    #region
    private bool isSelecting = false;

    private HashSet<Agent> latestDetectedSelectionBoxObjects = new HashSet<Agent>();
    // private bool wasOnTriggerAbleToBeCalled = false;
    #endregion

    // Getters and setters
    #region
    public Formation GetCurrentFormation()
    {
        if (currentFormation != null && currentFormation.IsEmpty())
        {
            currentFormation = null;
        }
        return currentFormation;
    }
    #endregion

    // IAgentsHandler implementation
    #region
    public List<Agent> GetAllAvailableAgents()
    {
        List<Agent> resultList = new List<Agent>();

        foreach (Formation formation in availableFormations)
        {
            resultList.AddRange(formation.GetAllFormationAgents());
        }

        return resultList;
    }

    public void RegisterAgent(Agent agent)
    {
        if (agent.GetCurrentFormation() == null)
        {
            Formation agentsNewFormation = new Formation(agent);
            availableFormations.Add(agentsNewFormation);
            Debug.Log("Available formations count:" + availableFormations.Count);
        }
    }

    public void UnregisterAgent(Agent agent)
    {
        Formation oldAgentsFormation = agent.GetCurrentFormation();
        oldAgentsFormation?.RemoveAgentFromFormation(agent);
        TryRemoveFormation(oldAgentsFormation);
    }
    #endregion

    public bool IsSelectionCycleFinished
    {
        get
        {
            return !isSelecting;
        }
    }

    public bool IsSelecting
    {
        get
        {
            return isSelecting || !IsSelectionCycleFinished;
        }
    }

    void Update()
    {
        rectDrawer.MouseClickControl();
        RegisterMousePositions();
        CheckSelectionWithSingleClick();
        CheckSelectionWithRect();
    }

    private void CheckSelectionWithRect()
    {
        if (Vector3.Distance(startMousePosition, finishMousePosition) <= selectionBoxAccuracy)
        {
            return;
        }

        if (!Input.GetMouseButtonUp(0))
        {
            return;
        }

        Rect selectRect = GetSelectionRectangle();

        List<Vector3> nearClipCorners = GetNearClipCorners(selectRect);

        List<Vector3> farClipCorners = GetFarClipCorners(selectRect);

        Plane[] selectionRectFrustumPlanes = CalculateSelectionRectFrustumPlanes(nearClipCorners, farClipCorners);

        DetectSelectionBoxObjects(selectionRectFrustumPlanes);

        AdjustFormationWithBoxSelection();
    }

    private void AdjustFormationWithBoxSelection()
    {
        if (!Input.GetKey(KeyCode.LeftShift))
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
    }

    private void DetectSelectionBoxObjects(Plane[] selectionRectFrustumPlanes)
    {
        latestDetectedSelectionBoxObjects.Clear();

        List<Agent> availableAgents = GetAllAvailableAgents();

        foreach(Agent agent in availableAgents)
        {
            Unit unit = agent.SoldierBasic;

            List<ColliderCostPair> colliderCostPairs = unit.GetHitCollidersCosts();

            foreach(ColliderCostPair colliderCostPair in colliderCostPairs)
            {
                Bounds colliderBounds = colliderCostPair.collider.bounds;

                if (GeometryUtility.TestPlanesAABB(selectionRectFrustumPlanes, colliderBounds))
                {
                    latestDetectedSelectionBoxObjects.Add(agent);
                    break;
                }
            }
        }
    }

    private Plane[] CalculateSelectionRectFrustumPlanes(List<Vector3> nearClipCorners, List<Vector3> farClipCorners)
    {
        Vector3 A0 = nearClipCorners[0];

        Vector3 A1 = nearClipCorners[1];

        Vector3 A2 = nearClipCorners[2];

        Vector3 A3 = nearClipCorners[3];

        Vector3 A4 = farClipCorners[0];

        Vector3 A5 = farClipCorners[1];

        Vector3 A6 = farClipCorners[2];

        Vector3 A7 = farClipCorners[3];

        Plane leftPlane = new Plane(A0, A5, A1);

        Plane rightPlane = new Plane(A2, A6, A7);

        Plane downPlane = new Plane(A0, A3, A7);

        Plane upPlane = new Plane(A1, A5, A6);

        Plane nearPlane = new Plane(A0, A1, A2);

        Plane farPlane = new Plane(A4, A7, A6);

        return new Plane[]
        {
            leftPlane.flipped,
            rightPlane.flipped,
            downPlane.flipped,
            upPlane.flipped,
            nearPlane.flipped,
            farPlane.flipped
        };
    }

    private List<Vector3> GetFarClipCorners(Rect selectRect)
    {
        var farClipCorners = new Vector3[4];
        if (equalToCameraFarDistance)
        {
            playersCamera.CalculateFrustumCorners(selectRect, playersCamera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, farClipCorners);
        }
        else
        {
            playersCamera.CalculateFrustumCorners(selectRect, Mathf.Clamp(selectionDistance, playersCamera.nearClipPlane, playersCamera.farClipPlane),
                Camera.MonoOrStereoscopicEye.Mono, farClipCorners);
        }

        List<Vector3> farClipCornersWorld = new List<Vector3>(farClipCorners.Select((localPoint) =>
        {
            return playersCamera.transform.TransformPoint(localPoint);
        }));

        return farClipCornersWorld;
    }

    private List<Vector3> GetNearClipCorners(Rect selectRect)
    {
        var nearClipCorners = new Vector3[4];
        playersCamera.CalculateFrustumCorners(selectRect, playersCamera.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, nearClipCorners);
        List<Vector3> nearClipCornersWorld = new List<Vector3>(nearClipCorners.Select((localPoint) =>
        {
            return playersCamera.transform.TransformPoint(localPoint);
        }));

        return nearClipCornersWorld;
    }

    private Rect GetSelectionRectangle()
    {
        var topLeft = Vector3.Min(normalizedStartMousePosition, normalizedFinishMousePosition);
        var bottomRight = Vector3.Max(normalizedStartMousePosition, normalizedFinishMousePosition);
        Rect selectRect = Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
        return selectRect;
    }

    private void OnGUI()
    {
        rectDrawer.DrawRect(selectionBoxAccuracy, playersCamera);
    }

    private void RegisterMousePositions()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isSelecting = true;
            startMousePosition = Input.mousePosition;
            normalizedStartMousePosition = playersCamera.ScreenToViewportPoint(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0))
        {
            isSelecting = false;
            finishMousePosition = Input.mousePosition;
            normalizedFinishMousePosition = playersCamera.ScreenToViewportPoint(Input.mousePosition);
        }
    }

    private void AddNewAgentsToCurrentFormation()
    {
        List<Agent> agentsList = new List<Agent>();

        foreach (var agent in latestDetectedSelectionBoxObjects)
        {
            if (agent != null)
            {
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

        foreach (var agent in latestDetectedSelectionBoxObjects)
        {
            if (agent != null)
            {
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
                    TryRemoveFormation(oldAgentsFormation);
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
            TryRemoveFormation(oldAgentsFormation);
            newCurrentFormation.AddAgentToFormation(agent);
        }
        currentFormation = newCurrentFormation;
        availableFormations.Add(newCurrentFormation);
        MarkAgentsAsSelected(currentFormation);
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
        if (Physics.Raycast(playersCamera.ScreenPointToRay(Input.mousePosition), out raycastHit, selectionDistance))
        {
            Agent agent = raycastHit.transform.GetComponent<Agent>();
            if (agent != null)
            {
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
            TryRemoveFormation(agentsOldFormation);
            currentFormation.AddAgentToFormation(newAgent);
            newAgent.MarkAsSelected();
        }
    }

    private void TryRemoveFormation(Formation formation)
    {
        if (formation != null && formation.IsEmpty())
        {
            availableFormations.Remove(formation);
        }
    }

    private void ClearCurrentSelection()
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
