using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoCounter : MonoBehaviour
{
    public TextMeshProUGUI ammoCount;

    private WeaponArsenal playersWeaponsManager;
    private WeaponAmmo activeWeaponAmmo;

    private void Awake()
    {
        playersWeaponsManager = FindObjectOfType<Player>().GetComponent<WeaponArsenal>();
        playersWeaponsManager.OnSwitchedToWeapon += UpdateActiveWeapon;
    }

    private void OnDestroy()
    {
        playersWeaponsManager.OnSwitchedToWeapon -= UpdateActiveWeapon;
        activeWeaponAmmo.OnAmmoChange -= UpdateAmmoText;
    }

    private void UpdateAmmoText(int number)
    {
        ammoCount.SetText(number.ToString());
    }

    private void UpdateActiveWeapon(Weapon weapon)
    {
        activeWeaponAmmo = weapon.GetComponent<WeaponAmmo>();
        UpdateAmmoText(activeWeaponAmmo.CurrentAmmo);
        activeWeaponAmmo.OnAmmoChange += UpdateAmmoText;
    }
}
