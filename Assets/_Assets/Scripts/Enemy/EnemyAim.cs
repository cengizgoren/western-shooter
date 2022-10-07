using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAim : MonoBehaviour
{
    public Transform Target;
    public Transform AimPoint;
    public float RotationSpeed = 360f;

    void Update()
    {
        if (Target)
        {
            AimPoint.position = Target.position + new Vector3(0,1f,0f);
        }
        Vector3 playersDirectionToMouse = AimPoint.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(playersDirectionToMouse.x, 0f, playersDirectionToMouse.z), Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * RotationSpeed);
    }
}
