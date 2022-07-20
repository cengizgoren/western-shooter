using System;
using UnityEngine;
using UnityEngine.UI;

public class HUDHealthBarManager : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image weaponImage;

    private Health health;
    private DummyWeaponsManager weaponsManager;

    private int healthPoints;

    private void Awake()
    {
        health = GetComponent<Health>();
        weaponsManager = GetComponent<DummyWeaponsManager>();
        if (health)
            health.OnHealthChanged += ChangeValue;
        if (weaponsManager)
            weaponsManager.OnSwitchedToWeapon += ChangeIcon;
    }

    private void Update()
    {
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        healthBar.value = healthPoints;
    }

    private void ChangeValue(int amount)
    {
        healthPoints = amount;
    }

    private void ChangeIcon(WeaponController weapon) 
    {
        weaponImage.sprite = weapon.Icon;
    }
}
