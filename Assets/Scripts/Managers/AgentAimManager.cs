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

    [BoxGroup("Settings")]
    [SerializeField]
    [Tooltip("Slerp edge angle in degrees.")]
    /// <summary>
    /// Speed of slerping towards the target.
    /// </summary>
    private float edgeAngle = 5f;

    [BoxGroup("Settings")]
    [SerializeField]
    [Tooltip("Required and current direction vectors angle that is considered to be acceptable to shoot target.")]
    /// <summary>
    /// Speed of slerping towards the target.
    /// </summary>
    private float shootAngle = 1f;

    [SerializeField]
    [Required]
    AimIK aimIK;

    [SerializeField]
    [Required]
    AnimatorHandler animatorHandler;

    [SerializeField]
    [Required]
    AgentWeaponManager weaponManager;

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
    [Tooltip("Current target to aim, usually agent's part of the body")]
    private Transform currentTargetToAim = null;

    [SerializeField]
    [Tooltip("Current agent to aim")]
    private Agent agentToAim = null;

    private Coroutine aimingCoroutine = null;

    private bool turningToTarget;
    private Vector3 currentAimDirection;
    private Vector3 targetDirection;

    private float desiredRotation;
    private float currentRotation;
    private float turnToTargetVelocity;

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

    public void SetAgentToAim(Agent agentToAim)
    {
        this.agentToAim = agentToAim;
    }

    public Transform GetTarget()
    {
        return currentTargetToAim;
    }

    public Agent GetAgentToAim()
    {
        return agentToAim;
    }

    public void ClearTarget()
    {
        currentTargetToAim = null;
    }

    public void ClearAgentToAim()
    {
        agentToAim = null;
    }

    [Button("Check start aiming")]
    public void StartAiming(Action onStartAimingFinish = null)
    {

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

    public bool IsTargetReachable(Transform target)
    {
        if (currentTargetToAim == null)
        {
            return false;
        }

        //RaycastHit raycastHit;
        //if (Physics.Raycast(aimIK.solver.transform.position, aimIK.solver.transform.forward, out raycastHit,
        //    Mathf.Infinity))
        //{
        //    if (raycastHit.collider.transform == target)
        //    {
        //        return true;
        //    }
        //}

        if (Vector3.Angle(currentAimDirection, targetDirection) < shootAngle)
        {
            return true;
        }

        return false;
    }

    IEnumerator StartAimingAsync(Action onStartAimingFinish)
    {
        isAiming = true;

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

        if (agentToAim != null)
        {
            float weaponProjectileSpeed = weaponManager.ActiveGun.ProjectileSpeed;

            Vector3 agentsVelocity = agentToAim.GetVelocity();

            float travelTime = GetTravelTime(weaponProjectileSpeed, agentsVelocity, agentToAim.transform.position);

            requiredPosition += agentsVelocity * travelTime;
        }

        targetDirection = requiredPosition - Pivot;

        if (smoothTurnTowardsTarget)
        {
            if (Vector3.Angle(currentAimDirection, targetDirection) > edgeAngle)
            {
                currentAimDirection = Vector3.Slerp(currentAimDirection, targetDirection, Time.deltaTime * slerpSpeed);
            }
            else
            {
                currentAimDirection = requiredPosition - Pivot;
            }
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

    // Character root will be rotated around the Y axis to keep root forward within this angle from the aiming direction.
    private void RootRotation()
    {
        if (!turningToTarget)
        {
            Vector3 faceDirLocal = transform.InverseTransformDirection(aimIK.solver.IKPosition - Pivot);
            float angle = Mathf.Atan2(faceDirLocal.x, faceDirLocal.z) * Mathf.Rad2Deg;

            if (angle > maxRootAngle)
            {
                desiredRotation = angle;
                currentRotation = 0f;
                turningToTarget = true;
                // if (!turningToTarget && turnToTarget) StartCoroutine(TurnToTarget());
            }
            if (angle < -maxRootAngle)
            {
                desiredRotation = angle;
                currentRotation = 0f;
                turningToTarget = true;
                // if ( && turnToTarget) StartCoroutine(TurnToTarget());
            }

            return;
        }

        TurnToTarget();
    }

    // Aligns the root forward to target direction after "Max Root Angle" has been exceeded.
    private void TurnToTarget()
    {
        if (Mathf.Abs(desiredRotation - currentRotation) > 0.01f)
        {
            float oldRotation = currentRotation;
            currentRotation = Mathf.SmoothDamp(currentRotation, desiredRotation, ref turnToTargetVelocity, turnToTargetTime);
            transform.rotation = Quaternion.AngleAxis(currentRotation - oldRotation, transform.up) * transform.rotation;

            return;
        }

        turningToTarget = false;
    }

    /// <summary>
    /// Quite complex method, finds travel time as a solution of quadratic equation
    /// </summary>
    /// <param name="weaponProjectileSpeed"></param>
    /// <param name="agentsVelocity"></param>
    /// <returns></returns>
    private float GetTravelTime(float weaponProjectileSpeed, Vector3 agentsVelocity, Vector3 aimTargetPosition)
    {
        float a = Mathf.Pow(agentsVelocity.x, 2)
            + Mathf.Pow(agentsVelocity.y, 2)
            + Mathf.Pow(agentsVelocity.z, 2)
            - Mathf.Pow(weaponProjectileSpeed, 2);

        float b = 2 * agentsVelocity.x * (aimTargetPosition.x - transform.position.x)
            + 2 * agentsVelocity.y * (aimTargetPosition.y - transform.position.y)
            + 2 * agentsVelocity.z * (aimTargetPosition.z - transform.position.z);

        float c = Mathf.Pow(aimTargetPosition.x - transform.position.x, 2)
            + Mathf.Pow(aimTargetPosition.y - transform.position.y, 2)
            + Mathf.Pow(aimTargetPosition.z - transform.position.z, 2);

        float d = Mathf.Pow(b, 2) - 4 * a * c;

        float t1 = (-b + Mathf.Sqrt(d)) / 2 / a;

        float t2 = (-b - Mathf.Sqrt(d)) / 2 / a;

        return t1 > 0 ? t1 : t2;
    }
}
