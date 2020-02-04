using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class CommandsManager : MonoBehaviour
{
    [SerializeField]
    [Required]
    Camera playersCamera;

    [BoxGroup("Layer masks")]
    [Tooltip("Layers that are considered walkable by an agent")]
    [SerializeField]
    LayerMask walkableLayerMask;

    [BoxGroup("Layer masks")]
    [Tooltip("Layers that are considered attackable by an agent")]
    [SerializeField]
    LayerMask attackableLayerMask;

    [BoxGroup("Settings")]
    [SerializeField]
    [Tooltip("Max distance user can click on to command agent to do smth")]
    float commandDistance = 100;

    [BoxGroup("Settings")]
    [SerializeField]
    [Tooltip("Distance from ground that waves will be instantiated after click")]
    float clickWavesEffectGroundOffset = 0.05f;

    [BoxGroup("Settings")]
    [SerializeField]
    [Tooltip("Angle (in degrees) between up vector and raycat hit normal to be acceptable to send command to walk to")]
    float moveAngle = 15f;

    [BoxGroup("Service variables")]
    [SerializeField]
    [Required]
    [Tooltip("Player controller")]
    PlayerController playerController;

    public Goal CurrentGoalToCommand { get; private set; }

    public LayerMask WalkableLayerMask
    {
        get
        {
            return walkableLayerMask;
        }
    }

    public LayerMask AttackableLayerMask
    {
        get
        {
            return attackableLayerMask;
        }
    }

    public float CommandDistance
    {
        get
        {
            return commandDistance;
        }
    }

    public void CheckCommands()
    {
        CurrentGoalToCommand = null;
        if (Input.GetMouseButtonUp(1))
        {
            CheckMoveCommand();
            CheckAttackCommand();
        }
    }

    private void CheckMoveCommand()
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(playersCamera.ScreenPointToRay(Input.mousePosition), out raycastHit, commandDistance))
        {
            if (Vector3.Angle(raycastHit.normal, Vector3.up) < moveAngle
                && ((int)Mathf.Pow(2, raycastHit.transform.gameObject.layer) & walkableLayerMask.value) != 0)
            {
                ShowClickWavesEffects(raycastHit);
                CurrentGoalToCommand = new MoveGoal(raycastHit.point);
            }
        }
    }

    private void CheckAttackCommand()
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(playersCamera.ScreenPointToRay(Input.mousePosition), out raycastHit, commandDistance))
        {
            if (raycastHit.rigidbody == null)
            {
                return;
            }
            if (raycastHit.rigidbody.tag != "Selectable")
            {
                return;
            }

            Agent agent = raycastHit.rigidbody.GetComponent<Agent>();
            if (agent == null)
            {
                return;
            }

            if (agent.GetTeam() != playerController.GetTeam())
            {
                ShowAttackClickWavesEffects(agent.transform.position);
                CurrentGoalToCommand = new AttackGoal(agent, agent.transform.position);
            }
        }
    }

    private void ShowClickWavesEffects(RaycastHit raycastHit)
    {
        GameObject clickWavesEffectGameObject = PoolsManager.GetObjectPool(PoolsKeys.clickEffectsPoolKey).GetObject();
        clickWavesEffectGameObject.transform.position = raycastHit.point + Vector3.up * clickWavesEffectGroundOffset;
        clickWavesEffectGameObject.transform.rotation = Quaternion.LookRotation(Vector3.up);
    }

    private void ShowAttackClickWavesEffects(Vector3 position)
    {
        GameObject attackClickWavesEffectGameObject = PoolsManager.GetObjectPool(PoolsKeys.attackClickEffectsPoolKey).GetObject();
        attackClickWavesEffectGameObject.transform.position = position + Vector3.up * clickWavesEffectGroundOffset;
        attackClickWavesEffectGameObject.transform.rotation = Quaternion.LookRotation(Vector3.up);
    }
}
