using UnityEngine;

public class TankAudio : MonoBehaviour {
    [Header("Sources")]
    [SerializeField] private AudioSource idleSource;
    [SerializeField] private AudioSource moveSource;

    [Header("Thresholds")]
    [SerializeField] private float moveSpeedThreshold = 0.15f;
    [SerializeField] private float turnSpeedThreshold = 10f;

    private Vector3 previousPosition;
    private float previousYaw;
    private bool wasMoving;

    private void Start() {
        previousPosition = transform.position;
        previousYaw = transform.eulerAngles.y;

        if (idleSource != null && idleSource.clip != null && !idleSource.isPlaying)
            idleSource.Play();

        if (moveSource != null && moveSource.clip != null && !moveSource.isPlaying)
            moveSource.Play();

        ApplyState(false);
    }

    private void Update() {
        float dt = Mathf.Max(Time.deltaTime, 0.0001f);

        Vector3 delta = transform.position - previousPosition;
        delta.y = 0f;
        float planarSpeed = delta.magnitude / dt;

        float currentYaw = transform.eulerAngles.y;
        float yawDelta = Mathf.Abs(Mathf.DeltaAngle(previousYaw, currentYaw));
        float turnSpeed = yawDelta / dt;

        previousPosition = transform.position;
        previousYaw = currentYaw;

        bool isMoving =
            planarSpeed > moveSpeedThreshold ||
            turnSpeed > turnSpeedThreshold;

        if (isMoving != wasMoving) {
            ApplyState(isMoving);
            wasMoving = isMoving;
        }
    }

    private void ApplyState(bool moving) {
        if (idleSource != null) {
            idleSource.volume = moving ? 0f : 1f;
            idleSource.pitch = 1f;
        }

        if (moveSource != null) {
            moveSource.volume = moving ? 1f : 0f;
            moveSource.pitch = 1f;
        }
    }
}