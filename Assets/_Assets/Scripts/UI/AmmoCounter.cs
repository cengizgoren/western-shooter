using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoCounter : MonoBehaviour
{
    public Image weaponImage;
    public TextMeshProUGUI ammoCount;

    private WeaponSwitcher playersWeaponsManager;
    private WeaponAmmo activeWeaponAmmo;

    private void Awake()
    {
        playersWeaponsManager = FindObjectOfType<Player>().GetComponent<WeaponSwitcher>();
        playersWeaponsManager.OnSwitchedToWeapon += UpdateActiveWeapon;
    }

    private void UpdateAmmoText(int number)
    {
        ammoCount.SetText(number.ToString());
    }

    private void UpdateActiveWeapon(Weapon weapon)
    {
        activeWeaponAmmo =  weapon.GetComponent<WeaponAmmo>();
        UpdateAmmoText(activeWeaponAmmo.CurrentAmmo);
        activeWeaponAmmo.OnAmmoChange += UpdateAmmoText;
        weaponImage.sprite = weapon.WeaponIconSprite;
    }
}
