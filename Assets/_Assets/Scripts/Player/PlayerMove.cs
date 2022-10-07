using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    public float MoveSpeed = 2.0f;
    public float SprintSpeed = 5.335f;
    public float SpeedChangeRate = 10.0f;
    public float CameraYRotationDeg = 45.0f;

    private float _speed;
    private float _verticalVelocity; // ????

    private CharacterController _controller;
    private Animator _animator;

    public void Start()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        float targetSpeed = Controls.InputActions.Player.Sprint.IsPressed() ? SprintSpeed : MoveSpeed;
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

        Vector2 move = Controls.InputActions.Player.Move.ReadValue<Vector2>();
        Vector3 isometricDirection = Quaternion.AngleAxis(CameraYRotationDeg, Vector3.up) * new Vector3(move.x, 0.0f, move.y).normalized;
        Vector3 relativeVelocity = transform.InverseTransformDirection(_controller.velocity);

        // Vertical velocity but thats gravitys job?
        _controller.Move(isometricDirection * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        if (_animator)
        {
            _animator.SetFloat("SpeedZ", relativeVelocity.z);
            _animator.SetFloat("SpeedX", relativeVelocity.x);
            _animator.SetFloat("SpeedHorizontal", currentHorizontalSpeed);
        }
    }
}
