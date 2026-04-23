using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class TankMovement : MonoBehaviour {
    [Header("Movement")]
    [SerializeField] private float forwardSpeed = 7f;
    [SerializeField] private float reverseSpeed = 4f;
    [SerializeField] private float turnSpeed = 90f;

    [Header("Smoothing")]
    [SerializeField] private float acceleration = 8f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float turnAcceleration = 10f;

    private Rigidbody rb;

    private float moveInput;
    private float turnInput;

    private float currentMoveSpeed;
    private float currentTurnSpeed;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void Update() {
        ReadKeyboardInput();
    }

    private void FixedUpdate() {
        ApplyMovement();
        ApplyRotation();
    }

    private void ReadKeyboardInput() {
        moveInput = 0f;
        turnInput = 0f;

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
            return;

        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
            moveInput += 1f;

        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
            moveInput -= 1f;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            turnInput -= 1f;

        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            turnInput += 1f;
    }

    private void ApplyMovement() {
        float targetSpeed = 0f;

        if (moveInput > 0f)
            targetSpeed = moveInput * forwardSpeed;
        else if (moveInput < 0f)
            targetSpeed = moveInput * reverseSpeed;

        float rate = Mathf.Abs(targetSpeed) > Mathf.Abs(currentMoveSpeed) ? acceleration : deceleration;
        currentMoveSpeed = Mathf.MoveTowards(currentMoveSpeed, targetSpeed, rate * Time.fixedDeltaTime);

        Vector3 movement = transform.forward * currentMoveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }

    /*private void ApplyRotation() {
        float targetTurn = turnInput * turnSpeed;
        currentTurnSpeed = Mathf.MoveTowards(currentTurnSpeed, targetTurn, turnAcceleration * turnSpeed * Time.fixedDeltaTime);

        Quaternion deltaRotation = Quaternion.Euler(0f, currentTurnSpeed * Time.fixedDeltaTime, 0f);
        rb.MoveRotation(rb.rotation * deltaRotation);
    }*/

    private void ApplyRotation() {
        float turnFactor = Mathf.Abs(currentMoveSpeed) > 0.1f ? 1f : 0.45f;
        float targetTurn = turnInput * turnSpeed * turnFactor;

        currentTurnSpeed = Mathf.MoveTowards(
                                             currentTurnSpeed,
                                             targetTurn,
                                             turnAcceleration * turnSpeed * Time.fixedDeltaTime
                                            );

        Quaternion deltaRotation = Quaternion.Euler(0f, currentTurnSpeed * Time.fixedDeltaTime, 0f);
        rb.MoveRotation(rb.rotation * deltaRotation);
    }
}