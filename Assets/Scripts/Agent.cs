using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using Pathfinding;

public class Agent : MonoBehaviour
{
    [BoxGroup("Projectors")]
    [SerializeField]
    [Tooltip("Projector used to highlight if unit is in selection list")]
    GameObject selectionProjector;

    [BoxGroup("Projectors")]
    [SerializeField]
    [Tooltip("Projector used to highlight if unit is main in selection list")]
    GameObject mainSelectionProjector;

    [BoxGroup("Settings")]
    [SerializeField]
    [Tooltip("Agent's look distance")]
    float lookDistance = 60f;

    [SerializeField]
    [Tooltip("Controller that is able to give commands to this unit")]
    Controller controller;

    [SerializeField]
    [Tooltip("AI path handler")]
    RichAI aiPathHandler;

    private Formation currentFormation = null;

    private Goal currentGoal = null;

    // Moving goal additional help variables
    #region
    private Coroutine checkEndPathCoroutine = null;
    private Vector3 previosCheckCoordinate = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
    #endregion

    // Getters and setters
    #region
    public float AgentRadius
    {
        get
        {
            return aiPathHandler.radius;
        }
    }

    public void SetController(Controller controller)
    {
        this.controller = controller;
    }

    public Controller GetController()
    {
        return controller;
    }

    public void SetCurrentFormation(Formation currentFormation)
    {
        this.currentFormation = currentFormation;
    }

    public Formation GetCurrentFormation()
    {
        return currentFormation;
    }

    public void ClearCurrentFormation()
    {
        currentFormation = null;
    }

    public void SetNewGoal(Goal newGoal)
    {
        currentGoal = newGoal;

        if (currentGoal is MoveGoal)
        {
            aiPathHandler.destination = (currentGoal as MoveGoal).Destination;
            aiPathHandler.isStopped = false;

            checkEndPathCoroutine = StartCoroutine(CheckEndPathAsync());
        }
    }

    public Goal GetCurrentGoal()
    {
        return currentGoal;
    }
    #endregion

    // Selection mark methods
    #region
    public void MarkAsSelected()
    {
        selectionProjector.SetActive(true);
    }

    public void MarkAsMainSelected()
    {
        mainSelectionProjector.SetActive(true);
    }

    public void MarkAsUnselected()
    {
        selectionProjector.SetActive(false);
        mainSelectionProjector.SetActive(false);
    }
    #endregion

    private void Update()
    {
        CurrentBehaviour();
    }

    private void CurrentBehaviour()
    {
        if (currentGoal != null)
        {
            if (currentGoal is MoveGoal)
            {
                MoveToDestination((MoveGoal)currentGoal);
            }
            else if (currentGoal is AttackGoal)
            {
                AttackAgent((AttackGoal)currentGoal);
            }
            else
            {
                currentGoal = null;
            }
        }
        else
        {
            SearchForEnemies();
        }
    }

    private void AttackAgent(AttackGoal attackGoal)
    {
        Debug.Log("Try to attack someone");
    }

    private void MoveToDestination(MoveGoal moveGoal)
    {
        aiPathHandler.destination = moveGoal.Destination;

        if (aiPathHandler.reachedDestination)
        {
            aiPathHandler.isStopped = true;
            currentGoal = null;
            StopCoroutine(checkEndPathCoroutine);
            Debug.Log("Reached move goal!");
        }
    }

    private void SearchForEnemies()
    {
        // Debug.Log("Searching for enemies");
    }

    IEnumerator CheckEndPathAsync()
    {
        previosCheckCoordinate = transform.position;
        yield return new WaitForSeconds(LevelManager.Instance.AgentsSecondsTillCheckEndPath);

        while ((previosCheckCoordinate - transform.position).magnitude > Mathf.Epsilon)
        {
            previosCheckCoordinate = transform.position;
            yield return new WaitForSeconds(LevelManager.Instance.AgentsSecondsTillCheckEndPath);
        }

        if (currentGoal is MoveGoal)
        {
            aiPathHandler.isStopped = true;
            currentGoal = null;
            Debug.Log("Reached move goal!"); ;
        }
    }
}
