using UnityEngine;
using UnityEngine.InputSystem;

public class TankGun : MonoBehaviour {
    [Header("References")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private Collider[] tankColliders;

    [Header("Firing")]
    [SerializeField] private float fireCooldown = 0.25f;
    [SerializeField] private float bulletSpeed = 35f;

    private float nextFireTime;

    private void Update() {
        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame) {
            TryFire();
        }
    }

    private void TryFire() {
        if (Time.time < nextFireTime)
            return;

        if (bulletPrefab == null || muzzlePoint == null)
            return;

        nextFireTime = Time.time + fireCooldown;

        GameObject bulletObject = Instantiate(
                                              bulletPrefab,
                                              muzzlePoint.position,
                                              muzzlePoint.rotation
                                             );

        Bullet bullet = bulletObject.GetComponent<Bullet>();
        if (bullet != null) {
            bullet.Initialize(muzzlePoint.forward, bulletSpeed, tankColliders);
            return;
        }

        Rigidbody rb = bulletObject.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.linearVelocity = muzzlePoint.forward * bulletSpeed;
        }
    }
}