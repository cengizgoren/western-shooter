using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class WeaponController : MonoBehaviour
{
    public UnityAction OnTriggerPressed;
    public UnityAction OnTriggerReleased;
    public UnityAction OnShootingAllowed;
    public UnityAction OnShootingForbidden;
}
