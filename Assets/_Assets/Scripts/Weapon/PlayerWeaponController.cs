using System;
using UnityEngine.InputSystem;

public class PlayerWeaponController : WeaponController
{
    private Weapon playerWeapon;
    private WeaponSwitcher weaponSwitcher;

    private Action ReleaseTrigger() => () => OnTriggerReleased?.Invoke();

    private void Start()
    {
        playerWeapon = GetComponent<Weapon>();
        Controls.InputActions.Player.Shoot.performed += PressTrigger;
        Controls.InputActions.Player.Shoot.canceled += ReleaseTrigger;

        if (weaponSwitcher)
        {
            weaponSwitcher.OnWeaponReady += CheckReadiness;
        }
    }

    private void OnDestroy()
    {
        Controls.InputActions.Player.Shoot.performed -= PressTrigger;
        Controls.InputActions.Player.Shoot.canceled -= ReleaseTrigger;

        if (weaponSwitcher)
        {
            weaponSwitcher.OnWeaponReady -= CheckReadiness;
        }
    }

    private void PressTrigger(InputAction.CallbackContext _) => OnTriggerPressed?.Invoke();

    private void ReleaseTrigger(InputAction.CallbackContext _) => OnTriggerReleased?.Invoke();

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
