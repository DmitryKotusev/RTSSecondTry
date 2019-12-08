using UnityEngine;
using Sirenix.OdinInspector;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    [Required]
    Transform mainCameraTransform;

    [BoxGroup("Camera movement settings")]
    [SerializeField]
    float planeSpeed = 20f;
    [BoxGroup("Camera movement settings")]
    [SerializeField]
    float verticalSpeed = 10f;
    [BoxGroup("Camera movement settings")]
    [SerializeField]
    [Tooltip("Should camera move when cursur is next to screen borders")]
    bool takeBordersIntoAccount = false;
    [BoxGroup("Camera movement settings")]
    [SerializeField]
    [Tooltip("Distance between cursor and border in pixels, when camera starts moving")]
    [ShowIf("takeBordersIntoAccount")]
    float panBorderThickness = 10f;

    [BoxGroup("Rotation limit settings")]
    [SerializeField]
    float minVerticalRotation = 0f;
    [BoxGroup("Rotation limit settings")]
    [SerializeField]
    float maxVerticalRotation = 65f;
    [BoxGroup("Vertical position limit settings")]
    [SerializeField]
    float minVerticalPosition = 2f;
    [BoxGroup("Vertical position limit settings")]
    [SerializeField]
    float maxVerticalPosition = 15f;

    private bool verticalRotationEnabled = true;

    private float mouseX = 0;
    private float mouseY = 0;

    void LateUpdate()
    {
        PlanePositionUpdate();
        RotationUpdate();
        VerticalPositionUpdate();
    }

    private void PlanePositionUpdate()
    {
        Vector3 pos = transform.position;

        Vector3 moveVector = Vector3.zero;

        bool forwardMove = false;
        bool backwardMove = false;
        bool leftMove = false;
        bool rightMove = false;

        if (takeBordersIntoAccount)
        {
            forwardMove = Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height - panBorderThickness;
            backwardMove = Input.GetKey(KeyCode.S) || Input.mousePosition.y <= panBorderThickness;
            leftMove = Input.GetKey(KeyCode.A) || Input.mousePosition.x <= panBorderThickness;
            rightMove = Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - panBorderThickness;
        }
        else
        {
            forwardMove = Input.GetKey(KeyCode.W);
            backwardMove = Input.GetKey(KeyCode.S);
            leftMove = Input.GetKey(KeyCode.A);
            rightMove = Input.GetKey(KeyCode.D);
        }

        if (forwardMove)
        {
            moveVector += transform.forward.normalized;
        }

        if (backwardMove)
        {
            moveVector -= transform.forward.normalized;
        }

        if (leftMove)
        {
            moveVector -= transform.right.normalized;
        }

        if (rightMove)
        {
            moveVector += transform.right.normalized;
        }

        moveVector.y = 0;
        moveVector = moveVector.normalized * planeSpeed * Time.deltaTime;
        transform.position += moveVector;
    }

    private void RotationUpdate()
    {
        var easeFactor = 10f;
        bool midMouseButtonPressDown = Input.GetMouseButtonDown(2);
        if (midMouseButtonPressDown)
        {
            mouseX = Input.mousePosition.x;
            mouseY = Input.mousePosition.y;
            return;
        }
        bool midMouseButtonPressed = Input.GetMouseButton(2);
        if (midMouseButtonPressed)
        {
            float deltaXMouseMove = Input.mousePosition.x - mouseX;
            float deltaYMouseMove = mouseY - Input.mousePosition.y;
            mouseX = Input.mousePosition.x;
            mouseY = Input.mousePosition.y;
            // Horizontal
            if (Mathf.Abs(deltaXMouseMove) > 0)
            {
                var cameraRotationY = deltaXMouseMove * easeFactor * Time.deltaTime;
                transform.Rotate(0, cameraRotationY, 0);
            }

            // Vertical
            if (Mathf.Abs(deltaYMouseMove) > 0)
            {
                var cameraRotationX = deltaYMouseMove * easeFactor * Time.deltaTime;
                var desiredRotation = Mathf.Clamp(cameraRotationX + mainCameraTransform.rotation.eulerAngles.x, minVerticalRotation, maxVerticalRotation);
                mainCameraTransform.rotation = Quaternion.Euler(desiredRotation, mainCameraTransform.rotation.eulerAngles.y, mainCameraTransform.rotation.eulerAngles.z);
            }
        }
    }

    private void VerticalPositionUpdate()
    {
        bool mouseWheelUp = Input.GetAxis("Mouse ScrollWheel") > 0;
        bool mouseWheelDown = Input.GetAxis("Mouse ScrollWheel") < 0;
        // Move down
        if (mouseWheelUp)
        {
            var desiredCameraPositionY = Mathf.Clamp(transform.position.y - verticalSpeed * Time.deltaTime, minVerticalPosition, maxVerticalPosition);
            transform.position = new Vector3(transform.position.x, desiredCameraPositionY, transform.position.z);
        }
        // Move up
        if (mouseWheelDown)
        {
            var desiredCameraPositionY = Mathf.Clamp(transform.position.y + verticalSpeed * Time.deltaTime, minVerticalPosition, maxVerticalPosition);
            transform.position = new Vector3(transform.position.x, desiredCameraPositionY, transform.position.z);
        }
    }
}
