using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponArsenal : MonoBehaviour
{
    public static readonly int WEAPON_SLOTS_NUMBER = 9;

    public Transform WeaponParent;
    public List<Weapon> StartingWeapons = new List<Weapon>();

    private readonly Weapon[] weaponSlots = new Weapon[9];
    private Weapon activeWeapon;
    private int activeWeaponIndex = 0;

    private void Awake()
    {
        foreach (Weapon weapon in StartingWeapons)
        {
            AddWeapon(weapon);
        }
    }

    private void Start()
    {
        int i = 0;

        foreach (Weapon weapon in weaponSlots)
        {
            if (weapon)
            {
                weapon.gameObject.SetActive(true);
                activeWeapon = weapon;
                activeWeaponIndex = i;
                break;
            }
            i++;
        }

        Controls.InputActions.Player.Weapon.performed += ctx =>
        {
            int weaponNumber = (int)ctx.ReadValue<float>();
            weaponNumber -= 1;
            if (weaponNumber >= 0 && weaponNumber <= 8 && weaponNumber != activeWeaponIndex && weaponSlots[weaponNumber] != null)
            {
                activeWeapon.gameObject.SetActive(false);
                activeWeapon = weaponSlots[weaponNumber];
                activeWeaponIndex = weaponNumber;
                activeWeapon.gameObject.SetActive(true);
            }
        };
    }

    public void AddWeapon(Weapon weaponPrefab)
    {
        for (int i = 0; i < WEAPON_SLOTS_NUMBER; i++)
        {
            if (weaponSlots[i] == null)
            {
                Weapon weaponInstance = Instantiate(weaponPrefab, WeaponParent);
                weaponInstance.Setup(gameObject);
                weaponInstance.gameObject.SetActive(false);
                weaponSlots[i] = weaponInstance;
                break;
            }
        }
    }

    public Weapon GetWeaponAtSlotIndex(int index)
    {
        if (index >= 0 && index < weaponSlots.Length)
        {
            return weaponSlots[index];
        }
        return null;
    }
}
