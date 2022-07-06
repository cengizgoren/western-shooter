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

    public LayerMask groundMask;

    // player
    private float _speed;
    private float _turnSpeed = 10.0f;

    private Animator _animator;
    private CharacterController _controller;
    private DummyInput _input;
    private GameObject _mainCamera;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _input = GetComponent<DummyInput>();
    }

    void Update()
    {
        Move();
    }

    private void LateUpdate()
    {
        CameraRotation();
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

        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
        Vector3 relativeVelocity = transform.InverseTransformDirection(_controller.velocity);
        _controller.Move(_speed * Time.deltaTime * inputDirection);

        if (_animator)
        {

            _animator.SetFloat("SpeedZ", relativeVelocity.z);
            _animator.SetFloat("SpeedX", relativeVelocity.x);
            _animator.SetFloat("SpeedHorizontal", currentHorizontalSpeed);
        }
    }

    void CameraRotation()
    {
        Ray ray = Camera.main.ScreenPointToRay(_input.look);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            LookAt(point);
        }
    }

    void LookAt(Vector3 lookPoint)
    {
        Vector3 direction = (lookPoint - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0.0f, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _turnSpeed);
    }
}