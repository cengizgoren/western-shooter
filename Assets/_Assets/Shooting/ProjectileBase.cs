using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ProjectileBase : MonoBehaviour
{
    public UnityAction OnShoot;
    public Vector3 InitialPosition { get; private set; }
    public Vector3 InitialDirection { get; private set; }

    public void Shoot(WeaponController controller)
    {
        InitialPosition = transform.position;
        InitialDirection = transform.forward;
        OnShoot?.Invoke();
    }
}
