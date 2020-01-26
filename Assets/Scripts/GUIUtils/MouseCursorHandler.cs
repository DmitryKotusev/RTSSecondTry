using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MouseCursorHandler : MonoBehaviour
{
    [BoxGroup("Cursor textures")]
    [SerializeField]
    [Required]
    [Tooltip("Base cursor")]
    Texture2D baseCursor;

    [BoxGroup("Cursor textures")]
    [SerializeField]
    [Required]
    [Tooltip("Attack cursor")]
    Texture2D attackCursor;

    [BoxGroup("Cursor textures")]
    [SerializeField]
    [Required]
    [Tooltip("Move cursor")]
    Texture2D moveCursor;

    [BoxGroup("Service variables")]
    [SerializeField]
    [Required]
    [Tooltip("Selection manager")]
    SelectionManager selectionManager;

    [BoxGroup("Service variables")]
    [SerializeField]
    [Required]
    [Tooltip("Commands manager")]
    CommandsManager commandsManager;

    [BoxGroup("Service variables")]
    [SerializeField]
    [Required]
    [Tooltip("Player controller")]
    PlayerController playerController;

    [BoxGroup("Service variables")]
    [SerializeField]
    [Required]
    [Tooltip("Players camera")]
    Camera playersCamera;

    [BoxGroup("Animators")]
    [SerializeField]
    [Required]
    [Tooltip("Move cursor animator")]
    CursorAnimator moveCursorAnimator = new CursorAnimator();

    private Texture2D currentCursor = null;

    private void Update()
    {
        CursorControl();
    }

    private void CursorControl()
    {
        if (selectionManager.GetCurrentFormation() != null && !selectionManager.IsSelecting)
        {
            if (CheckAttackCursor())
            {
                return;
            }

            if (CheckMoveCursor())
            {
                moveCursorAnimator.Update(Time.deltaTime, 0.5f, 0.5f);
                return;
            }
        }

        if (currentCursor != baseCursor)
        {
            currentCursor = baseCursor;
            Cursor.SetCursor(baseCursor, Vector2.zero, CursorMode.Auto);
        }
    }

    private bool CheckAttackCursor()
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(playersCamera.ScreenPointToRay(Input.mousePosition), out raycastHit, commandsManager.CommandDistance))
        {
            if (raycastHit.rigidbody == null)
            {
                return false;
            }
            if (raycastHit.rigidbody.tag != "Selectable")
            {
                return false;
            }

            Agent agent = raycastHit.rigidbody.GetComponent<Agent>();
            if (agent == null)
            {
                return false;
            }

            if (agent.GetTeam() != playerController.GetTeam())
            {
                if (currentCursor != attackCursor)
                {
                    currentCursor = attackCursor;
                    Cursor.SetCursor(attackCursor, Vector2.zero, CursorMode.Auto);
                }

                return true;
            }
        }

        return false;
    }

    private bool CheckMoveCursor()
    {
        //RaycastHit raycastHit;
        //if (Physics.Raycast(playersCamera.ScreenPointToRay(Input.mousePosition), out raycastHit, commandsManager.CommandDistance))
        //{
        //    if (((int)Mathf.Pow(2, raycastHit.transform.gameObject.layer) & commandsManager.WalkableLayerMask.value) != 0)
        //    {
        //        currentCursor = moveCursor;
        //        //if (currentCursor != moveCursor)
        //        //{
        //        //    currentCursor = moveCursor;
        //        //    Cursor.SetCursor(moveCursor, new Vector2(moveCursor.width / 2, moveCursor.height / 2), CursorMode.Auto);
        //        //}

        //        return true;
        //    }
        //}

        if (currentCursor != moveCursor)
        {
            currentCursor = moveCursor;
        }

        return true;
    }
}
