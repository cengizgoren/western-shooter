using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerWeaponArsenal : MonoBehaviour
{
    public static readonly int WEAPON_SLOTS_NUMBER = 9;

    public Transform WeaponSocket;
    public List<Weapon> StartingWeapons = new List<Weapon>();

    private readonly Weapon[] weaponSlots = new Weapon[9];

    private void Awake()
    {
        foreach (Weapon weapon in StartingWeapons)
        {
            AddWeapon(weapon);
        }
    }

    public void AddWeapon(Weapon weaponPrefab)
    {
        for (int i = 0; i < WEAPON_SLOTS_NUMBER; i++)
        {
            if (weaponSlots[i] == null)
            {
                Weapon weaponInstance = Instantiate(weaponPrefab, WeaponSocket);
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
