using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPlayer : MonoBehaviour
{
    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float MoveSpeed = 2.0f;

    [Tooltip("Sprint speed of the character in m/s")]
    public float SprintSpeed = 5.335f;

    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;

    [Tooltip("Offset player movement direction by this angle to make it relative to the camera position")]
    public float CameraYRotationDeg = 45.0f;

    public float TurnSpeed = 10.0f;

    [Space(10)]
    [Tooltip("The height the player can jump")]
    public float JumpHeight = 1.2f;

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -15.0f;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.50f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;

    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;

    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;


    // player
    private float _speed;
    private float _terminalVelocity = 53.0f;
    private float _verticalVelocity;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private Animator _animator;
    private CharacterController _controller;
    private DummyInput _input;



    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _input = GetComponent<DummyInput>();
    }

    void Update()
    {
        PlayerRotation();
        JumpAndGravity();
        GroundedCheck();
        Move();
    }

    private void LateUpdate() { }

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
    }

    private void Move()
    {
        float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        float speedOffset = 0.1f;
        float inputMagnitude = 1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        Vector3 isometricDirection = Quaternion.AngleAxis(CameraYRotationDeg, Vector3.up) * new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
        Vector3 relativeVelocity = transform.InverseTransformDirection(_controller.velocity);

        _controller.Move(isometricDirection * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        if (_animator)
        {
            _animator.SetFloat("SpeedZ", relativeVelocity.z);
            _animator.SetFloat("SpeedX", relativeVelocity.x);
            _animator.SetFloat("SpeedHorizontal", currentHorizontalSpeed);
        }
    }

    void PlayerRotation()
    {
        Ray ray = Camera.main.ScreenPointToRay(_input.look);
        //TODO: Align aiming plane in a better way
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, 1.4f, 0));
        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            Vector3 halfPoint = transform.position + (point - transform.position) / 5;
            CinemachineCameraTarget.transform.position = halfPoint;
            //LookAt(point);
            Vector3 dirToMouse = point - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(dirToMouse.x, 0f, dirToMouse.z), Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 720f);
        }
    }

    void LookAt(Vector3 lookPoint)
    {
        Vector3 direction = (lookPoint - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0.0f, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * TurnSpeed);
    }

    private void JumpAndGravity()
    {
        if (Grounded)
        {
            _fallTimeoutDelta = FallTimeout;

            // Stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            if (_input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                // The square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

            }

            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            _jumpTimeoutDelta = JumpTimeout;

            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }

            _input.jump = false;
        }

        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying == false)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(_input.look);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(point, 0.5f);

            Vector3 halfPoint = transform.position + (point - transform.position) / 5;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(halfPoint, 0.5f);
        }
    }
}