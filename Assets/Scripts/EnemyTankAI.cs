using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyTankAI : MonoBehaviour {
    [Header("Tank Movement")]
    [SerializeField] private float forwardSpeed = 3.5f;
    [SerializeField] private float turnSpeed = 100f;
    [SerializeField] private float moveWhenAngleBelow = 35f;
    [SerializeField] private float arriveDistance = 1.2f;

    [Header("Wander")]
    [SerializeField] private float wanderRadius = 12f;
    [SerializeField] private float repathDelay = 1.2f;
    [SerializeField] private int maxRandomPointTries = 10;

    [Header("Effects")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionCleanupTime = 8f;

    private NavMeshAgent agent;
    private EnemyTankSpawner spawner;
    private bool isDestroyed;
    private float nextRepathTime;

    public void Initialize(EnemyTankSpawner ownerSpawner) {
        spawner = ownerSpawner;
    }

    private void Awake() {
        agent = GetComponent<NavMeshAgent>();

        // We use the NavMeshAgent only for path planning,
        // not for direct sliding movement.
        agent.updatePosition = false;
        agent.updateRotation = false;
    }

    private void OnEnable() {
        isDestroyed = false;
        nextRepathTime = 0f;
    }

    private void Update() {
        if (isDestroyed)
            return;

        if (!agent.isOnNavMesh)
            return;

        if (NeedsNewDestination()) {
            SetRandomDestination();
            nextRepathTime = Time.time + repathDelay;
        }

        MoveLikeTank();

        // Keep the agent synced with the actual transform position.
        agent.nextPosition = transform.position;
    }

    private bool NeedsNewDestination() {
        if (Time.time < nextRepathTime)
            return false;

        if (!agent.hasPath)
            return true;

        return agent.remainingDistance <= arriveDistance;
    }

    private void MoveLikeTank() {
        Vector3 targetPoint = agent.steeringTarget;
        Vector3 toTarget = targetPoint - transform.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude < 0.01f)
            return;

        Vector3 desiredDirection = toTarget.normalized;

        float signedAngle = Vector3.SignedAngle(
                                                transform.forward,
                                                desiredDirection,
                                                Vector3.up
                                               );

        float maxTurnThisFrame = turnSpeed * Time.deltaTime;
        float clampedTurn = Mathf.Clamp(signedAngle, -maxTurnThisFrame, maxTurnThisFrame);
        transform.Rotate(0f, clampedTurn, 0f);

        float absAngle = Mathf.Abs(signedAngle);

        if (absAngle < moveWhenAngleBelow) {
            float alignment = 1f - (absAngle / moveWhenAngleBelow);
            float speedFactor = Mathf.Lerp(0.45f, 1f, alignment);
            float moveStep = forwardSpeed * speedFactor * Time.deltaTime;
            transform.position += transform.forward * moveStep;
        }
    }

    private void SetRandomDestination() {
        Vector3 origin = transform.position;

        for (int i = 0; i < maxRandomPointTries; i++) {
            Vector2 random2D = Random.insideUnitCircle * wanderRadius;
            Vector3 candidate = origin + new Vector3(random2D.x, 0f, random2D.y);

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 4f, NavMesh.AllAreas)) {
                agent.SetDestination(hit.position);
                return;
            }
        }
    }

    public void Hit() {
        if (isDestroyed)
            return;

        isDestroyed = true;

        if (explosionPrefab != null) {
            GameObject fx = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(fx, explosionCleanupTime);
        }

        if (spawner != null)
            spawner.HandleEnemyDestroyed(this);
        else
            Destroy(gameObject);
    }
}