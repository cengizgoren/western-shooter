using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float CameraYRotationDeg = 45f;
    [SerializeField] private float MoveVelocity;
    [SerializeField] private float SprintVelocity;
    [SerializeField] private float AccelerationMultiplier;
    [SerializeField] private float DeaccelerationMultiplier;
    [SerializeField] private bool ShowDebugVectors = false;

    private Vector3 velocity;
    private Vector3 targetVelocity;
    private Vector3 velocityDelta;
    private float acceleration;

    private CharacterController controller;
    private Animator animator;

    public void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Move();
        Animate();
        Debug();
    }

    private void Move()
    {
        Vector2 move = Controls.InputActions.Player.Move.ReadValue<Vector2>().normalized;

        targetVelocity = Quaternion.AngleAxis(CameraYRotationDeg, Vector3.up) * new Vector3(move.x, 0f, move.y) * (Controls.InputActions.Player.Sprint.IsPressed() ? SprintVelocity : MoveVelocity);
        velocity = controller.velocity;
        velocityDelta = targetVelocity - velocity;

        if (targetVelocity.magnitude != 0f)
        {
            acceleration = velocityDelta.magnitude * AccelerationMultiplier;
            velocity = Vector3.MoveTowards(velocity, targetVelocity, Time.deltaTime * acceleration);
        }
        else
        {
            acceleration = velocityDelta.magnitude * DeaccelerationMultiplier;
            velocity = Vector3.MoveTowards(velocity, targetVelocity, Time.deltaTime * acceleration);
        }

        velocity.y = 0f;
        controller.Move(velocity * Time.deltaTime);
    }

    private void Animate()
    {
        if (animator)
        {
            float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;
            Vector3 relativeVelocity = transform.InverseTransformDirection(controller.velocity);
            animator.SetFloat("SpeedZ", relativeVelocity.z);
            animator.SetFloat("SpeedX", relativeVelocity.x);
            animator.SetFloat("SpeedHorizontal", currentHorizontalSpeed);
        }
    }

    private void Debug()
    {
        if (ShowDebugVectors)
        {
            // TODO: check this stuff inside draw functions
            if (controller.velocity.magnitude > 0.1f)
            {
                DebugTools.Draw.DebugArrow(transform.position, velocity, Color.blue, 1f);
                if (velocityDelta.magnitude > 0.1f)
                {
                    DebugTools.Draw.DebugArrow(transform.position + controller.velocity, velocityDelta * acceleration, Color.cyan, 1f);
                }
            }

            if (targetVelocity.magnitude > 0.1f)
            {
                DebugTools.Draw.DebugArrow(transform.position, targetVelocity, Color.red, 1f);
                DebugTools.Draw.DrawWireArc(transform.position, -transform.forward, 360f, targetVelocity.magnitude, Color.white);
            }
        }
    }
}
