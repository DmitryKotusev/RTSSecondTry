using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerController : Controller
{
    [SerializeField]
    [Required]
    Camera playersCamera;

    [SerializeField]
    [Required]
    SelectionManager selectionManager;

    [SerializeField]
    [Required]
    CommandsManager commandsManager;
}
