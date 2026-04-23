using UnityEngine;
using UnityEngine.InputSystem;

public class TankTurretAim : MonoBehaviour {
    [Header("References")]
    [SerializeField] private Camera aimCamera;
    [SerializeField] private Transform fallbackForwardReference;

    [Header("Turret Rotation")]
    [SerializeField] private float turnSpeed = 180f;
    [SerializeField] private float yawOffset = 0f;

    private void Update() {
        if (aimCamera == null)
            return;

        Vector2 mouseScreenPosition = GetMouseScreenPosition();

        // Convert mouse X position to camera viewport X (0..1),
        // then to a symmetric range (-1..1).
        Vector3 viewportPoint = aimCamera.ScreenToViewportPoint(mouseScreenPosition);
        float normalizedX = Mathf.Clamp(viewportPoint.x, 0f, 1f) * 2f - 1f;

        // Horizontal aiming is limited to the camera's visible horizontal FOV.
        float horizontalHalfFov = GetHorizontalFov(aimCamera) * 0.5f;
        float targetYawFromCamera = normalizedX * horizontalHalfFov;

        Vector3 flatReferenceForward = Vector3.ProjectOnPlane(aimCamera.transform.forward, Vector3.up);

        if (flatReferenceForward.sqrMagnitude < 0.0001f) {
            if (fallbackForwardReference != null)
                flatReferenceForward = Vector3.ProjectOnPlane(fallbackForwardReference.forward, Vector3.up);
            else
                flatReferenceForward = Vector3.forward;
        }

        flatReferenceForward.Normalize();

        // Rotate only on the horizontal plane.
        Vector3 targetDirection =
            Quaternion.AngleAxis(targetYawFromCamera, Vector3.up) * flatReferenceForward;

        float targetWorldYaw =
            Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg + yawOffset;

        Quaternion targetRotation = Quaternion.Euler(0f, targetWorldYaw, 0f);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );
    }

    private Vector2 GetMouseScreenPosition() {
        if (Mouse.current == null)
            return new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

        return Mouse.current.position.ReadValue();
    }

    private static float GetHorizontalFov(Camera cam) {
        float verticalFovRad = cam.fieldOfView * Mathf.Deg2Rad;
        float horizontalFovRad = 2f * Mathf.Atan(Mathf.Tan(verticalFovRad * 0.5f) * cam.aspect);
        return horizontalFovRad * Mathf.Rad2Deg;
    }
}