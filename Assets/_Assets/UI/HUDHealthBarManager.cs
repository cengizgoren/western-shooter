using System;
using UnityEngine;
using UnityEngine.UI;

public class HUDHealthBarManager : MonoBehaviour
{
    [SerializeField] private Slider healthBar;

    private Health health;

    private int healthPoints;

    private void Awake()
    {
        health = GetComponent<Health>();
        if (health)
        {
            health.OnHealthChanged += ChangeValue;
        }
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
}
