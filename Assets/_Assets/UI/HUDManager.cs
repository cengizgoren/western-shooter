using System;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [SerializeField] private Slider healthBar;

    private int healthPoints;

    private void Awake()
    {
        Health.OnHealthChanged += ChangeValue;
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
