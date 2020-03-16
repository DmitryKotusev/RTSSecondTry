using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(SpriteRenderer))]
public class Border : MonoBehaviour
{
    [SerializeField]
    [Required]
    LineRenderer lineRenderer;
    
    [SerializeField]
    private int oneUnitRadiusPointsCoint = 40;

    [SerializeField]
    private float terraintOffset = 0.1f;

    [SerializeField]
    [Required]
    private CapturePoint capturePoint;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        DrawGizmozBorder();
    }

    private void DrawGizmozBorder()
    {
        if (capturePoint == null || capturePoint.CapturePointData == null)
        {
            return;
        }

        Gizmos.color = Color.white;

        float radius = capturePoint.CapturePointData.CaptureRadius;

        float height = capturePoint.CapturePointData.DetectionCapsuleHeight;

        float stepAngle = 360f / oneUnitRadiusPointsCoint / radius;

        float currentAngle = 0f;

        Vector3 oldPosition = transform.position + transform.forward * radius;

        oldPosition = CheckHeight(height, oldPosition);

        oldPosition += transform.up * terraintOffset;

        for (int i = 0; i < oneUnitRadiusPointsCoint * radius; i++)
        {
            currentAngle += stepAngle;

            Vector3 newPosition = transform.position
                + Quaternion.AngleAxis(currentAngle, transform.up) * transform.forward * radius;

            newPosition = CheckHeight(height, newPosition);

            newPosition += transform.up * terraintOffset;

            Gizmos.DrawLine(oldPosition, newPosition);

            oldPosition = newPosition;
        }
    }

    private Vector3 CheckHeight(float height, Vector3 newPosition)
    {
        Vector3 resultPosition = newPosition;

        RaycastHit raycastHit;

        if (Physics.Raycast(newPosition, -transform.up, out raycastHit, height))
        {
            resultPosition = raycastHit.point;
        }
        else if (Physics.Raycast(newPosition + transform.up * height,
            -transform.up, out raycastHit, height))
        {
            resultPosition = raycastHit.point;
        }

        return resultPosition;
    }
#endif
}
