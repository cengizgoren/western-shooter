using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [Range(-10.0f, 10.0f)]
    public float MouseAndAimPointDistance = 0f;
    public Vector3 MousePoint;
    public Vector3 AimPoint;
    public Transform CameraRoot;
    public Transform AimArrow;
    public LineRenderer LaserSight;
    public float RotationSpeed = 720f;

    private void Update()
    {
        Rotation();
    }

    private void Rotation()
    {
        Ray ray = Camera.main.ScreenPointToRay(Controls.InputActions.Player.Look.ReadValue<Vector2>());
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, 0f, 0f));
        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            MousePoint = ray.GetPoint(rayDistance);
            Vector3 cameraRootPoint = transform.position + (MousePoint - transform.position) / 5;
            CameraRoot.position = cameraRootPoint;
            Vector3 playersDirectionToMouse = MousePoint - transform.position;
            AimPoint = MousePoint + new Vector3(playersDirectionToMouse.x, 0f, playersDirectionToMouse.z).normalized * MouseAndAimPointDistance;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(playersDirectionToMouse.x, 0f, playersDirectionToMouse.z), Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * RotationSpeed);

            // Position aim arrow
            AimArrow.position = new Vector3(MousePoint.x, AimArrow.position.y, MousePoint.z);
        }
    }
}
