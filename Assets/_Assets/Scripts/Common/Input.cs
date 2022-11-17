using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Input : MonoBehaviour
{
    [SerializeField] protected Transform Mouse;
    [SerializeField] protected bool AllowAiming;

    public bool IsAimingAllowed { get => AllowAiming; }

    public Vector3 MouseWorldPosition { get => Mouse.position; }

    public Vector3 DirectionToMouse { get; protected set; }

}
