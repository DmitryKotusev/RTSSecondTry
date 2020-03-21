using UnityEngine;
using System;

[Serializable]
public class ClipHandInfo
{
    [SerializeField]
    private Vector3 clipHandPosition;

    [SerializeField]
    private Quaternion clipHandRotation;

    public Vector3 ClipHandPosition
    {
        get
        {
            return clipHandPosition;
        }
    }

    public Quaternion ClipHandRotation
    {
        get
        {
            return clipHandRotation;
        }
    }
}
