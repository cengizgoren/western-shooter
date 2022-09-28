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
    public Vector3 MousePoint;
    [Space(10)]
    public AnimationCurve DistanceCurve;
    public float BaseOveraimDistance = 2f;
    public float MaxCurveDistance = 90f;
    public float Magnitude;
    public float OveraimFactor;

    private Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, 0f, 0f));

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
            CameraRoot.position = transform.position + (MousePoint - transform.position) / 5;
            
            // Rotation
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(playersDirectionToMouse.x, 0f, playersDirectionToMouse.z), Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * RotationSpeed);

            // Aim Point
            Magnitude = playersDirectionToMouse.magnitude;
            OveraimFactor = DistanceCurve.Evaluate(Magnitude / MaxCurveDistance);
            AimPoint.position = MousePoint + OveraimFactor * BaseOveraimDistance * new Vector3(playersDirectionToMouse.x, 0f, playersDirectionToMouse.z).normalized;

            // Direction Arrow
            DirectionArrow.position = new Vector3(MousePoint.x, DirectionArrow.position.y, MousePoint.z);
        }
    }
}
