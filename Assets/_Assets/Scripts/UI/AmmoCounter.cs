using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoCounter : MonoBehaviour
{
    [SerializeField] private Image weaponImage;
    [SerializeField] private TextMeshProUGUI ammo;

    private DummyPlayer weaponsManager;
    private WeaponController currentWeapon;

    private void Awake()
    {
        weaponsManager = FindObjectOfType<DummyPlayer>();
        weaponsManager.OnSwitchedToWeapon += ChangeWeapon;

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
}
