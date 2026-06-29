using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 2f;
    public float rotationSpeed = 2f;
    public float stepOffset = 0.4f;

    private Animator animator;
    private CharacterController controller;
    private float verticalVelocity = 0f;  // only addition
    private float gravity = -20f;          // only addition

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        controller.stepOffset = stepOffset;
    }

    void Update()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        // UNTOUCHED — your working movement logic
        Vector3 move = transform.forward * vertical + transform.right * horizontal;
        move = Vector3.ClampMagnitude(move, 1f);

        if (move.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // ONLY GRAVITY CHANGED
        if (controller.isGrounded)
            verticalVelocity = -2f;
        else
            verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = move * walkSpeed;
        velocity.y = verticalVelocity;     // replaces old -9.81f * Time.deltaTime
        controller.Move(velocity * Time.deltaTime);

        // UNTOUCHED
        animator.SetFloat("VelX", horizontal);
        animator.SetFloat("VelZ", vertical);
    }
}