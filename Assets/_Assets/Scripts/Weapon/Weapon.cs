using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WeaponShooting))]
[RequireComponent(typeof(WeaponAmmo))]
[RequireComponent(typeof(WeaponAim))]
[RequireComponent(typeof(WeaponLaserSight))]
public class Weapon : MonoBehaviour
{
    public Sprite Icon;
    
    private GameObject Owner;

    public void Setup(GameObject owner)
    {
        Owner = owner;
    }

    public GameObject GetOwner()
    {
        return Owner;
    }
}
