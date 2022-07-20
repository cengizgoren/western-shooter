using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoCounter : MonoBehaviour
{
    [SerializeField] private Image weaponImage;
    [SerializeField] private TextMeshProUGUI ammo;

    private DummyWeaponsManager weaponsManager;
    private WeaponController currentWeapon;

    private void Awake()
    {
        weaponsManager = FindObjectOfType<DummyWeaponsManager>();
        weaponsManager.OnSwitchedToWeapon += ChangeWeapon;
        // weaponsManager.OnAddedWeapon += AddWeapon;
        // weaponsManager.OnRemovedWeapon += RemoveWeapon;
    }

    private void Update()
    {
        UpdateAmmmo();
    }

    private void UpdateAmmmo() 
    {
        if (currentWeapon)
            ammo.SetText(currentWeapon.CurrentAmmo.ToString());
    }

    private void ChangeWeapon(WeaponController weapon)
    { 
        currentWeapon = weapon;
        weaponImage.sprite = weapon.Icon;
    }

    // void AddWeapon(WeaponController newWeapon, int weaponIndex)
    // { }

    // void RemoveWeapon(WeaponController newWeapon, int weaponIndex)
    // { }
}
