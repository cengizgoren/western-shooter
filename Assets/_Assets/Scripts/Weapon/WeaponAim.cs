using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAim : MonoBehaviour
{
    [Range(-10.0f, 10.0f)]
    public float MouseAndAimPointDistance = 2f;
    public Transform WeaponMuzzle;

    private Vector3 rotationMask = new Vector3(1f, 0f, 0f);
    private Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, 0f, 0f));

    private void Update()
    {
        AimMuzzle();
    }

    private void AimMuzzle()
    {
        Ray ray = Camera.main.ScreenPointToRay(Controls.InputActions.Player.Look.ReadValue<Vector2>());
        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 mousePoint = ray.GetPoint(rayDistance);
            Vector3 playersDirectionToMouse = mousePoint - transform.position;
            Vector3 aimPoint = mousePoint + new Vector3(playersDirectionToMouse.x, 0f, playersDirectionToMouse.z).normalized * MouseAndAimPointDistance;
            Vector3 muzzleToMousePoint = aimPoint - WeaponMuzzle.transform.position;
            Vector3 lookAtRotation = Quaternion.LookRotation(muzzleToMousePoint).eulerAngles;
            WeaponMuzzle.transform.localRotation = Quaternion.Euler(Vector3.Scale(lookAtRotation, rotationMask));
        }
    }
}
