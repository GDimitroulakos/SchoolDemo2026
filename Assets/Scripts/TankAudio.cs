using UnityEngine;

public class TankAudio : MonoBehaviour {
    [Header("Sources")]
    [SerializeField] private AudioSource idleSource;
    [SerializeField] private AudioSource moveSource;

    [Header("Reference Speeds")]
    [SerializeField] private float maxReferenceMoveSpeed = 7f;
    [SerializeField] private float maxReferenceTurnSpeed = 120f;

    [Header("Blend")]
    [SerializeField] private float response = 4f;
    [SerializeField] private float turnWeight = 0.7f;

    [Header("Idle Mix")]
    [SerializeField] private float idleVolumeAtRest = 0.45f;
    [SerializeField] private float idleVolumeAtMotion = 0.20f;
    [SerializeField] private float idlePitchAtRest = 0.95f;
    [SerializeField] private float idlePitchAtMotion = 1.05f;

    [Header("Move Mix")]
    [SerializeField] private float moveVolumeAtRest = 0.00f;
    [SerializeField] private float moveVolumeAtMotion = 0.85f;
    [SerializeField] private float movePitchAtRest = 0.85f;
    [SerializeField] private float movePitchAtMotion = 1.20f;

    private Vector3 previousPosition;
    private float previousYaw;
    private float activityBlend;

    private void Start() {
        previousPosition = transform.position;
        previousYaw = transform.eulerAngles.y;

        if (idleSource != null && idleSource.clip != null && !idleSource.isPlaying)
            idleSource.Play();

        if (moveSource != null && moveSource.clip != null && !moveSource.isPlaying)
            moveSource.Play();
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

        float move01 = Mathf.Clamp01(planarSpeed / maxReferenceMoveSpeed);
        float turn01 = Mathf.Clamp01(turnSpeed / maxReferenceTurnSpeed);

        float targetActivity = Mathf.Max(move01, turn01 * turnWeight);
        activityBlend = Mathf.MoveTowards(activityBlend, targetActivity, response * Time.deltaTime);

        if (idleSource != null) {
            idleSource.volume = Mathf.Lerp(idleVolumeAtRest, idleVolumeAtMotion, activityBlend);
            idleSource.pitch = Mathf.Lerp(idlePitchAtRest, idlePitchAtMotion, activityBlend);
        }

        if (moveSource != null) {
            moveSource.volume = Mathf.Lerp(moveVolumeAtRest, moveVolumeAtMotion, activityBlend);
            moveSource.pitch = Mathf.Lerp(movePitchAtRest, movePitchAtMotion, activityBlend);
        }
    }
}