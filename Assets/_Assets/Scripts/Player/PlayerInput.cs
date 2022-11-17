using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerInput : Input
{
    private readonly Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, 0f, 0f));
    private Vector3 mouseScreenPosition;

    private void Update()
    {
        if (Controls.InputActions.Player.enabled)
        {
            mouseScreenPosition = Controls.InputActions.Player.Look.ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);

            if (groundPlane.Raycast(ray, out float rayDistance))
            {
                Mouse.position = ray.GetPoint(rayDistance);
                DirectionToMouse = Mouse.position - transform.position;
            }
        }
    }
}
