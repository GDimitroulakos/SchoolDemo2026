using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Bullet : MonoBehaviour {
    [Header("Lifetime")]
    [SerializeField] private float lifeTime = 5f;

    [Header("Explosion")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionCleanupTime = 8f;
    [SerializeField] private bool explodeOnCollision = true;

    private Rigidbody rb;
    private Collider bulletCollider;
    private bool hasExploded;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        bulletCollider = GetComponent<Collider>();
    }

    private void Start() {
        StartCoroutine(LifeTimer());
    }

    public void Initialize(Vector3 direction, float speed, Collider[] collidersToIgnore) {
        direction.y = 0f;
        direction.Normalize();

        rb.linearVelocity = direction * speed;

        if (collidersToIgnore == null)
            return;

        foreach (Collider tankCollider in collidersToIgnore) {
            if (tankCollider != null) {
                Physics.IgnoreCollision(bulletCollider, tankCollider, true);
            }
        }
    }

    private IEnumerator LifeTimer() {
        yield return new WaitForSeconds(lifeTime);
        Explode();
    }

    private void OnCollisionEnter(Collision collision) {
        if (explodeOnCollision)
            Explode();
        else
            Destroy(gameObject);
    }

    private void Explode() {
        if (hasExploded)
            return;

        hasExploded = true;

        if (explosionPrefab != null) {
            GameObject fx = Instantiate(
                explosionPrefab,
                transform.position,
                Quaternion.identity
            );

            Destroy(fx, explosionCleanupTime);
        }

        Destroy(gameObject);
    }
}