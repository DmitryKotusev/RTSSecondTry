using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField]
    Transform bodyPart;

    private void Update()
    {
        Debug.Log(bodyPart);
    }
}
