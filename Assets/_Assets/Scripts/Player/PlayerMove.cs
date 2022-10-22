using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    public float CameraYRotationDeg = 45f;
    public float MaxVelocity;
    public float AccelerationMultiplier;
    public float DeaccelerationMultiplier;

    [Header("Measurements")]
    public bool ShowDebugVectors = false;
    public Vector3 Velocity;
    public Vector3 TargetVelocity;
    public float Acceleration;

    private CharacterController controller;
    private Animator animator;

    public void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Move();
        Animate();
    }

    private void Move()
    {
        Vector2 move = Controls.InputActions.Player.Move.ReadValue<Vector2>().normalized;

        TargetVelocity = Quaternion.AngleAxis(CameraYRotationDeg, Vector3.up) * new Vector3(move.x, 0f, move.y) * MaxVelocity;
        Velocity = controller.velocity;
        Vector3 velocityDelta = TargetVelocity - Velocity;

        if (TargetVelocity.magnitude != 0f)
        {
            Acceleration = velocityDelta.magnitude * AccelerationMultiplier;
            Velocity = Vector3.MoveTowards(Velocity, TargetVelocity, Time.deltaTime * Acceleration);
        }
        else
        {
            Acceleration = velocityDelta.magnitude * DeaccelerationMultiplier;
            Velocity = Vector3.MoveTowards(Velocity, TargetVelocity, Time.deltaTime * Acceleration);
        }

        Velocity.y = 0f;
        controller.Move(Velocity * Time.deltaTime);

        // Debug
        if (ShowDebugVectors)
        {
            // TODO: check this stuff inside draw functions
            if (controller.velocity.magnitude > 0.1f)
            {
                DebugTools.Draw.DebugArrow(transform.position, Velocity, Color.blue, 1f);
                if (velocityDelta.magnitude > 0.1f)
                {
                    DebugTools.Draw.DebugArrow(transform.position + controller.velocity, velocityDelta * Acceleration, Color.cyan, 1f);
                }
            }

            if (TargetVelocity.magnitude > 0.1f)
            {
                DebugTools.Draw.DebugArrow(transform.position, TargetVelocity, Color.red, 1f);
                DebugTools.Draw.DrawWireArc(transform.position, -transform.forward, 360f, TargetVelocity.magnitude, Color.white);
            }
        }
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

    //private void OldMove()
    //{
    //    float targetSpeed = Controls.InputActions.Player.Sprint.IsPressed() ? SprintSpeed : MoveSpeed;
    //    float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
    //    float speedOffset = 0.1f;
    //    float inputMagnitude = 1f;

    //    if (currentHorizontalSpeed < targetSpeed - speedOffset ||
    //        currentHorizontalSpeed > targetSpeed + speedOffset)
    //    {
    //        _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
    //        _speed = Mathf.Round(_speed * 1000f) / 1000f;
    //    }
    //    else
    //    {
    //        _speed = targetSpeed;
    //    }

    //    Vector2 move = Controls.InputActions.Player.Move.ReadValue<Vector2>();
    //    Vector3 isometricDirection = Quaternion.AngleAxis(CameraYRotationDeg, Vector3.up) * new Vector3(move.x, 0.0f, move.y).normalized;
    //    Vector3 relativeVelocity = transform.InverseTransformDirection(_controller.velocity);

    //    // Vertical velocity but thats gravitys job?
    //    _controller.Move(isometricDirection * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

    //    if (_animator)
    //    {
    //        _animator.SetFloat("SpeedZ", relativeVelocity.z);
    //        _animator.SetFloat("SpeedX", relativeVelocity.x);
    //        _animator.SetFloat("SpeedHorizontal", currentHorizontalSpeed);
    //    }
    //}
}
