using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    public Transform CameraRoot;
    public float RotationSpeed = 720f;
    [Space(10)]

    public Transform AimPoint;
    public Transform DirectionArrow; 
    
    [Header("Camera")]
    [Range(1.0f, 2.0f)] public float OveraimFactor = 1.25f;
    [Range(0.0f, 1.0f)] public float MouseWeight = 0.6f;
    [Range(0.0f, 100.0f)] public float MaxCameraOffset = 25f;

    private Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, 0f, 0f));
    private Vector3 MousePoint;

    private void Update()
    {
        Rotation();
    }

    private void Rotation()
    {
        Ray ray = Camera.main.ScreenPointToRay(Controls.InputActions.Player.Look.ReadValue<Vector2>());
        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            MousePoint = ray.GetPoint(rayDistance);
            Vector3 playersDirectionToMouse = MousePoint - transform.position;

            // Camera
            CameraRoot.position = transform.position + Vector3.ClampMagnitude(MouseWeight * playersDirectionToMouse, MaxCameraOffset);

            // Rotation
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(playersDirectionToMouse.x, 0f, playersDirectionToMouse.z), Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * RotationSpeed);

            // Aim Point
            AimPoint.position = transform.position + OveraimFactor * playersDirectionToMouse;

            // Direction Arrow
            DirectionArrow.position = new Vector3(MousePoint.x, DirectionArrow.position.y, MousePoint.z);
        }
    }
}
