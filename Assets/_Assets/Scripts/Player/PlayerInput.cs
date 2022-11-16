using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerInput : MonoBehaviour
{
    private readonly Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, 0f, 0f));
    private Vector3 mouseScreenPosition;

    public Vector3 MouseWorldPosition { get; private set; }
    public Vector3 DirectionToMouse { get; private set; }

    private void Update()
    {
        if (Controls.InputActions.Player.enabled)
        {
            mouseScreenPosition = Controls.InputActions.Player.Look.ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);

            if (groundPlane.Raycast(ray, out float rayDistance))
            {
                MouseWorldPosition = ray.GetPoint(rayDistance);
                DirectionToMouse = MouseWorldPosition - transform.position;
            }
        }
    }
}
