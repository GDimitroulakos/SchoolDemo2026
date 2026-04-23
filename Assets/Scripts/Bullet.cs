using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Bullet : MonoBehaviour {
    [SerializeField] private float lifeTime = 4f;

    private Rigidbody rb;
    private Collider bulletCollider;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        bulletCollider = GetComponent<Collider>();
    }

    private void Start() {
        Destroy(gameObject, lifeTime);
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

    private void OnCollisionEnter(Collision collision) {
        Destroy(gameObject);
    }
}