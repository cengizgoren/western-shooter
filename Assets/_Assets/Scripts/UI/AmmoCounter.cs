using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoCounter : MonoBehaviour
{
    [SerializeField] private Image weaponImage;
    [SerializeField] private TextMeshProUGUI ammo;
    public IntVariable Variable;

    private PlayerWeaponManager weaponsManager;
    private WeaponController currentWeapon;

    private void Awake()
    {
        weaponsManager = FindObjectOfType<PlayerWeaponManager>();
        weaponsManager.OnSwitchedToWeapon += ChangeWeapon;
    }

    private void Update()
    {
        ammo.SetText(Variable.RuntimeValue.ToString());
    }

    private void ChangeWeapon(WeaponController weapon)
    { 
        currentWeapon = weapon;
        weaponImage.sprite = weapon.Icon;
    }
}
