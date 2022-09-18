using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerWeaponController : WeaponController
{
    //[SerializeField]  // Debug trick, good to remember
    private Weapon playerWeapon;
    //[SerializeField]
    private PlayerWeaponSwitcher weaponSwitcher;

    private void Start()
    {
        playerWeapon = GetComponent<Weapon>();
        weaponSwitcher = playerWeapon.GetOwner().GetComponent<PlayerWeaponSwitcher>();

        Controls.InputActions.Player.Shoot.performed += _ => base.OnTriggerPressed?.Invoke();
        Controls.InputActions.Player.Shoot.canceled += _ => base.OnTriggerReleased?.Invoke();
        weaponSwitcher.OnWeaponReady += isReady => CheckReadiness(isReady);
    }

    private void CheckReadiness(bool isReady)
    {
        if (isReady)
        {
            base.OnShootingAllowed?.Invoke();
        } 
        else
        {
            base.OnShootingForbidden?.Invoke();
        }
    }
}
