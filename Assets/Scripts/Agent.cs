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

    [SerializeField]
    [Tooltip("Controller that is able to give commands to this unit")]
    Controller controller;

    [SerializeField]
    [Tooltip("AI path handler")]
    RichAI aiPathHandler;

    private Formation currentFormation = null;

    private Goal currentGoal = null;

    // Getters and setters
    #region
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
                MoveToDestiantion((MoveGoal)currentGoal);
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

    private void MoveToDestiantion(MoveGoal moveGoal)
    {
        Debug.Log("MovingToestination");
    }

    private void SearchForEnemies()
    {
        Debug.Log("Searching for enemies");
    }
}
