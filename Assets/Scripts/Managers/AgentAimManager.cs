using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using RootMotion.FinalIK;
using System;

public class AgentAimManager : MonoBehaviour
{
    [BoxGroup("Settings")]
    [SerializeField]
    [Tooltip("Speed of going to aim state (units per second)")]
    float aimSpeed = 1f;

    [BoxGroup("Settings")]
    [SerializeField]
    [Tooltip("Aim layer weight value from which the weigth of aim IK will linearly increase")]
    [Range(0, 1)]
    float aimWeightBound = 0.5f;

    [BoxGroup("Settings")]
    [SerializeField]
    [Tooltip("Character root will be rotate around the Y axis to keep root forward within this angle from the aiming direction.")]
    /// <summary>
    ///Character root will be rotate around the Y axis to keep root forward within this angle from the aiming direction.
    /// </summary>
    [Range(0f, 180f)] private float maxRootAngle = 35f;

    [BoxGroup("Settings")]
    [SerializeField]
    [Tooltip("If enabled, aligns the root forward to target direction after 'Max Root Angle' has been exceeded.")]
    /// <summary>
    /// If enabled, aligns the root forward to target direction after 'Max Root Angle' has been exceeded.
    /// </summary>
    private bool turnToTarget;

    [BoxGroup("Settings")]
    [SerializeField]
    [Tooltip("The time of turning towards the target direction if 'Max Root Angle has been exceeded and 'Turn To Target' is enabled.")]
    /// <summary>
    /// The time of turning towards the target direction if 'Max Root Angle has been exceeded and 'Turn To Target' is enabled.
    /// </summary>
    private float turnToTargetTime = 0.2f;

    [BoxGroup("Settings")]
    [SerializeField]
    [Tooltip("Enables smooth turning towards the target according to the parameters under this header.")]
    /// <summary>
    /// Enables smooth turning towards the target according to the parameters under this header.
    /// </summary>
    private bool smoothTurnTowardsTarget = true;

    [BoxGroup("Settings")]
    [SerializeField]
    [Tooltip("The position of the pivot that the aim target is rotated around relative to the root of the character.")]
    /// <summary>
    /// The position of the pivot that the aim target is rotated around relative to the root of the character.
    /// </summary>
    private Vector3 pivotOffsetFromRoot = Vector3.up;

    [BoxGroup("Settings")]
    [SerializeField]
    [Tooltip("Minimum distance of aiming from the first bone. Keeps the solver from failing if the target is too close.")]
    /// <summary>
    /// Minimum distance of aiming from the first bone. Keeps the solver from failing if the target is too close.
    /// </summary>
    private float minDistance = 1f;

    [BoxGroup("Settings")]
    [SerializeField]
    [Tooltip("Speed of slerping towards the target.")]
    /// <summary>
    /// Speed of slerping towards the target.
    /// </summary>
    private float slerpSpeed = 3f;

    [SerializeField]
    AimIK aimIK;

    [SerializeField]
    AnimatorHandler animatorHandler;

    [SerializeField]
    [ReadOnly]
    [Tooltip("Is aiming?")]
    private bool isAiming;
    public bool IsAiming
    {
        get
        {
            return isAiming;
        }
    }

    [SerializeField]
    [Tooltip("Current target to aim")]
    private Transform currentTargetToAim = null;

    private Coroutine aimingCoroutine = null;

    // Final IK example variables
    private bool turningToTarget;
    private float turnToTargetMyltilpier = 1f;
    private float turnToTargetMyltilpierVelocity;
    private Vector3 currentAimDirection;

    private void Start()
    {
        aimIK.enabled = false;
        currentAimDirection = aimIK.solver.IKPosition - Pivot;
    }

    public void UpdateAimManager()
    {
        AimTarget();
        aimIK.solver.Update();
    }

    public void SetTarget(Transform newTarget)
    {
        currentTargetToAim = newTarget;
    }

    public Transform GetTarget()
    {
        return currentTargetToAim;
    }

    public void ClearTarget()
    {
        currentTargetToAim = null;
    }

    [Button("Check start aiming")]
    public void StartAiming(Action onStartAimingFinish = null)
    {
        isAiming = true;

        if (aimingCoroutine != null)
        {
            StopCoroutine(aimingCoroutine);
        }

        aimingCoroutine = StartCoroutine(StartAimingAsync(onStartAimingFinish));
    }

    [Button("Check stop aiming")]
    public void StopAiming(Action onStopAimingFinish = null)
    {
        if (aimingCoroutine != null)
        {
            StopCoroutine(aimingCoroutine);
        }

        aimingCoroutine = StartCoroutine(StopAimingAsync(onStopAimingFinish));
    }

    IEnumerator StartAimingAsync(Action onStartAimingFinish)
    {
        float currentAimWeight = animatorHandler.GetLayersWeight(animatorHandler.FullBodyRifleAimStandLayer);

        while (true)
        {
            currentAimWeight = Mathf.Clamp01(currentAimWeight + aimSpeed * Time.deltaTime);
            float currentAimIKWieght = Mathf.Clamp01((currentAimWeight - aimWeightBound) / (1 - aimWeightBound));

            animatorHandler.UpdateLayerWeight(animatorHandler.FullBodyRifleAimStandLayer, currentAimWeight);
            aimIK.solver.IKPositionWeight = currentAimIKWieght;

            if (Mathf.Abs(1 - currentAimWeight) < Mathf.Epsilon)
            {
                break;
            }

            yield return null;
        }

        onStartAimingFinish?.Invoke();
    }

    IEnumerator StopAimingAsync(Action onStopAimingFinish)
    {
        float currentAimWeight = animatorHandler.GetLayersWeight(animatorHandler.FullBodyRifleAimStandLayer);

        while (true)
        {
            currentAimWeight = Mathf.Clamp01(currentAimWeight - aimSpeed * Time.deltaTime);
            float currentAimIKWieght = Mathf.Clamp01((currentAimWeight - aimWeightBound) / (1 - aimWeightBound));

            animatorHandler.UpdateLayerWeight(animatorHandler.FullBodyRifleAimStandLayer, currentAimWeight);
            aimIK.solver.IKPositionWeight = currentAimIKWieght;

            if (currentAimWeight < Mathf.Epsilon)
            {
                break;
            }

            yield return null;
        }

        isAiming = false;

        onStopAimingFinish?.Invoke();
    }

    private void AimTarget()
    {
        if (currentTargetToAim == null)
        {
            return;
        }



        SmoothAim();

        // Min distance from the pivot
        ApplyMinDistance();

        // Root rotation
        RootRotation();
    }

    private void SmoothAim()
    {
        Vector3 requiredPosition = currentTargetToAim.position;

        if (smoothTurnTowardsTarget)
        {
            Vector3 targetDirection = requiredPosition - Pivot;
            currentAimDirection = Vector3.Slerp(currentAimDirection, targetDirection, Time.deltaTime * slerpSpeed);
        }
        else
        {
            currentAimDirection = requiredPosition - Pivot;
        }

        aimIK.solver.IKPosition = Pivot + currentAimDirection;
    }

    // Pivot of rotating the aiming direction.
    private Vector3 Pivot
    {
        get
        {
            return transform.position + transform.rotation * pivotOffsetFromRoot;
        }
    }

    // Make sure aiming target is not too close (might make the solver instable when the target is closer to the first bone than the last bone is).
    void ApplyMinDistance()
    {
        Vector3 aimFrom = Pivot;
        Vector3 direction = aimIK.solver.IKPosition - aimFrom;
        direction = direction.normalized * Mathf.Max(direction.magnitude, minDistance);

        aimIK.solver.IKPosition = aimFrom + direction;
    }

    // Character root will be rotate around the Y axis to keep root forward within this angle from the aiming direction.
    private void RootRotation()
    {
        float max = Mathf.Lerp(180f, maxRootAngle * turnToTargetMyltilpier, aimIK.solver.IKPositionWeight);

        if (max < 180f)
        {
            Vector3 faceDirLocal = transform.InverseTransformDirection(aimIK.solver.IKPosition - Pivot);
            float angle = Mathf.Atan2(faceDirLocal.x, faceDirLocal.z) * Mathf.Rad2Deg;

            float rotation = 0f;

            if (angle > max)
            {
                rotation = angle - max;
                if (!turningToTarget && turnToTarget) StartCoroutine(TurnToTarget());
            }
            if (angle < -max)
            {
                rotation = angle + max;
                if (!turningToTarget && turnToTarget) StartCoroutine(TurnToTarget());
            }

            transform.rotation = Quaternion.AngleAxis(rotation, transform.up) * transform.rotation;
        }
    }

    // Aligns the root forward to target direction after "Max Root Angle" has been exceeded.
    private IEnumerator TurnToTarget()
    {
        turningToTarget = true;

        while (turnToTargetMyltilpier > 0f)
        {
            turnToTargetMyltilpier = Mathf.SmoothDamp(turnToTargetMyltilpier, 0f, ref turnToTargetMyltilpierVelocity, turnToTargetTime);
            if (turnToTargetMyltilpier < 0.01f)
            {
                turnToTargetMyltilpier = 0f;
            }

            yield return null;
        }

        turnToTargetMyltilpier = 1f;
        turningToTarget = false;
    }
}
