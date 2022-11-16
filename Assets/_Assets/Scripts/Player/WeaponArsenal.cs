using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class WeaponArsenal : MonoBehaviour
{
    [SerializeField] private Transform WeaponParent;
    [SerializeField] private List<Weapon> StartingWeapons = new List<Weapon>();

    private readonly Weapon[] weaponSlots = new Weapon[9];
    private Weapon activeWeapon;
    private int activeWeaponIndex = 0;

    public static readonly int WeaponSlotsNumber = 9;
    public UnityAction<Weapon> OnSwitchedToWeapon;

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
                OnSwitchedToWeapon?.Invoke(activeWeapon);
                break;
            }
            i++;
        }

        Controls.InputActions.Player.Weapon.performed += PerformWeaponSwitchinng;
    }

    private void OnDestroy()
    {
        Controls.InputActions.Player.Weapon.performed -= PerformWeaponSwitchinng;
    }

    private void PerformWeaponSwitchinng(InputAction.CallbackContext context)
    {
        int weaponNumber = (int)context.ReadValue<float>();
        weaponNumber -= 1;
        if (weaponNumber >= 0 && weaponNumber <= 8 && weaponNumber != activeWeaponIndex && weaponSlots[weaponNumber] != null)
        {
            activeWeapon.gameObject.SetActive(false);
            activeWeapon = weaponSlots[weaponNumber];
            activeWeaponIndex = weaponNumber;
            activeWeapon.gameObject.SetActive(true);
            OnSwitchedToWeapon?.Invoke(activeWeapon);
        }
    }

    public void AddWeapon(Weapon weaponPrefab)
    {
        for (int i = 0; i < WeaponSlotsNumber; i++)
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
