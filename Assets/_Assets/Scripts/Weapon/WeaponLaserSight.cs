using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLaserSight : MonoBehaviour
{
    public LineRenderer LaserSightEffectPrefab;
    public Transform Origin;

    private LineRenderer LaserSightEffect;
    private LineRenderer GroundLaserSightEffect;

    private void Start()
    {
        LaserSightEffect = Instantiate(LaserSightEffectPrefab, transform);
        GroundLaserSightEffect = Instantiate(LaserSightEffectPrefab, transform);

    }

    private void Update()
    {
        Ray ray = new Ray(Origin.transform.position, Origin.transform.forward);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, 0f, 0f));
        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 groundPoint = ray.GetPoint(rayDistance);
            LaserSightEffect.SetPosition(0, Origin.position);
            LaserSightEffect.SetPosition(1, groundPoint);

            GroundLaserSightEffect.SetPosition(0, new Vector3(Origin.position.x, 0f, Origin.position.z));
            GroundLaserSightEffect.SetPosition(1, groundPoint);
        }
    }
}
