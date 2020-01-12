using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MouseCursorHandler : MonoBehaviour
{
    [BoxGroup("Cursor textures")]
    [SerializeField]
    [Tooltip("Attack cursor")]
    Texture2D attackCursor;
}
