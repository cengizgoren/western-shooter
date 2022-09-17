using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoCounter : MonoBehaviour
{
    [SerializeField] private Image weaponImage;
    [SerializeField] private TextMeshProUGUI ammo;
    public IntVariable Variable;

    private PlayerWeaponSwitcher weaponsManager;
    private Weapon currentWeapon;

    private void Awake()
    {
        weaponsManager = FindObjectOfType<PlayerWeaponSwitcher>();
        weaponsManager.OnSwitchedToWeapon += ChangeWeapon;
    }

    private void Update()
    {
        ammo.SetText(Variable.RuntimeValue.ToString());
    }

    private void ChangeWeapon(Weapon weapon)
    { 
        currentWeapon = weapon;
        weaponImage.sprite = weapon.Icon;
    }
}
