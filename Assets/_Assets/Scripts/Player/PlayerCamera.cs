using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform CameraRoot;
    [SerializeField][Range(0.0f, 1.0f)] private float MouseWeight = 0.6f;
    [SerializeField][Range(0.0f, 100.0f)] private float MaxCameraOffset = 25f;

    private PlayerInput input;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        CameraRoot.position = transform.position + Vector3.ClampMagnitude(MouseWeight * input.DirectionToMouse, MaxCameraOffset);
    }
}
