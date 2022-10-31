using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(WeaponShooting))]
public class WeaponAmmo : MonoBehaviour
{
    public UnityAction<int> OnAmmoChange;

    private WeaponStats weaponStats;
    private float reloadStartedTime;

    public bool IsReloading { get; private set; }
    public int CurrentAmmo { get; private set; }

    private void Awake()
    {
        weaponStats = GetComponent<WeaponShooting>().WeaponStats;
        IsReloading = false;
        UpdateAmmo(weaponStats.ClipSize);
    }

    private void Update()
    {
        CheckIfReloadNeeded();
        CheckIfReloadEnded();
    }

    public void Spend(int amount)
    {
        UpdateAmmo(CurrentAmmo - amount);
    }

    private void CheckIfReloadNeeded()
    {
        if (!IsReloading && CurrentAmmo <= 0)
        {
            reloadStartedTime = Time.time;
            IsReloading = true;
        }
    }

    private void CheckIfReloadEnded()
    {
        if (IsReloading && reloadStartedTime + weaponStats.ReloadTime < Time.time)
        {
            UpdateAmmo(weaponStats.ClipSize);
            IsReloading = false;
        }
    }

    private void UpdateAmmo(int newValue)
    {
        CurrentAmmo = newValue;
        OnAmmoChange?.Invoke(CurrentAmmo);
    }
}
