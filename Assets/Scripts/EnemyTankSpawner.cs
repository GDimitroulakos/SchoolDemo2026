using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTankSpawner : MonoBehaviour {
    [Header("References")]
    [SerializeField] private EnemyTankAI enemyPrefab;
    [SerializeField] private Transform playerTank;

    [Header("Spawn Rules")]
    [SerializeField] private int initialEnemyCount = 1;
    [SerializeField] private float spawnRadius = 35f;
    [SerializeField] private float minDistanceFromPlayer = 18f;
    [SerializeField] private float respawnDelay = 2f;
    [SerializeField] private int maxSpawnTries = 20;

    private void Start() {
        for (int i = 0; i < initialEnemyCount; i++) {
            SpawnEnemy();
        }
    }

    public void HandleEnemyDestroyed(EnemyTankAI enemy) {
        Destroy(enemy.gameObject);
        StartCoroutine(RespawnAfterDelay());
    }

    private IEnumerator RespawnAfterDelay() {
        yield return new WaitForSeconds(respawnDelay);
        SpawnEnemy();
    }

    private void SpawnEnemy() {
        Vector3 spawnPosition = FindSpawnPosition();

        EnemyTankAI enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        enemy.Initialize(this);
    }

    private Vector3 FindSpawnPosition() {
        Vector3 center = transform.position;

        for (int i = 0; i < maxSpawnTries; i++) {
            Vector2 random2D = Random.insideUnitCircle * spawnRadius;
            Vector3 candidate = center + new Vector3(random2D.x, 0f, random2D.y);

            if (playerTank != null) {
                float distanceToPlayer = Vector3.Distance(candidate, playerTank.position);
                if (distanceToPlayer < minDistanceFromPlayer)
                    continue;
            }

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 6f, NavMesh.AllAreas))
                return hit.position;
        }

        return center;
    }
}