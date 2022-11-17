using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyInput : Input
{
    [SerializeField] private Transform Target;
    [SerializeField] private Transform DefaultTarget;

    private void Start()
    {
        Target = DefaultTarget;
    }

    private void Update()
    {
        Mouse.position = Target.position;
        DirectionToMouse = Mouse.position - transform.position;
    }

    public void SetTargetTransform(Transform target)
    {
        AllowAiming = true;
        Target = target;
    }

    public void ResetTargetTransform()
    {
        AllowAiming = false;
        Target = DefaultTarget;
    }
}
