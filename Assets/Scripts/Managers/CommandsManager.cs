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
    float commandDistance = 50;

    [BoxGroup("Settings")]
    [SerializeField]
    [Tooltip("Distance from ground that waves will be instantiated after click")]
    float clickWavesEffectGroundOffset = 0.05f;

    public Goal CurrentGoalToCommand { get; private set; }

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
        if (Physics.Raycast(playersCamera.ScreenPointToRay(Input.mousePosition), out raycastHit, commandDistance, walkableLayerMask))
        {
            ShowClickWavesEffects(raycastHit);
            CurrentGoalToCommand = new MoveByCommandGoal(raycastHit.point);
        }
    }

    private void CheckAttackCommand()
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(playersCamera.ScreenPointToRay(Input.mousePosition), out raycastHit, commandDistance, attackableLayerMask))
        {
        }
    }

    private void ShowClickWavesEffects(RaycastHit raycastHit)
    {
        GameObject clickWavesEffectGameObject = PoolsManager.GetObjectPool(Poolskeys.clickEffectsPoolKey).GetObject();
        clickWavesEffectGameObject.transform.position = raycastHit.point + Vector3.up * clickWavesEffectGroundOffset;
        clickWavesEffectGameObject.transform.rotation = Quaternion.LookRotation(Vector3.up);
    }
}
