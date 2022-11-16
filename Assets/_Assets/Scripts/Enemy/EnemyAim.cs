using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAim : MonoBehaviour
{
    [SerializeField] private Transform DefaultTarget;
    [SerializeField] private Transform Target;
    [SerializeField] private Transform AimPoint;
    [SerializeField] private float RotationSpeed = 360f;

    private void Update()
    {
        if (Target)
        {
            AimPoint.position = Target.position + new Vector3(0, 1f, 0f);
        }
        Vector3 playersDirectionToMouse = AimPoint.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(playersDirectionToMouse.x, 0f, playersDirectionToMouse.z), Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * RotationSpeed);
    }

    public void SetTargetTransform(Transform target)
    {
        Target = target;
    }

    public void ResetTargetTransform()
    {
        Target = DefaultTarget;
    }
}
