using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CustomScriptables/CapturePointData")]
public class CapturePointData : ScriptableObject
{
    [SerializeField]
    private float captureRadius = 15;

    [SerializeField]
    private float captureSpeed = 15;

    [SerializeField]
    private float dischargeSpeed = 45;

    [SerializeField]
    private float captureCapacity = 1000;

    [SerializeField]
    private float detectionCapsuleHeight = 10;

    public float CaptureRadius => captureRadius;

    public float CaptureSpeed => captureSpeed;

    public float DischargeSpeed => dischargeSpeed;

    public float CaptureCapacity => captureCapacity;

    public float DetectionCapsuleHeight => detectionCapsuleHeight;
}
