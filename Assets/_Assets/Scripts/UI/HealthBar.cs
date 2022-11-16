using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private Health playerHealth;

    private void Start()
    {
        healthBar.maxValue = playerHealth.MaxHP;
        healthBar.value = playerHealth.MaxHP;
        playerHealth.OnHpLost += UpdateHealthBar;
    }

    private void UpdateHealthBar()
    {
        healthBar.maxValue = playerHealth.MaxHP;
        healthBar.value = playerHealth.HP;
    }

    private void OnDestroy()
    {
        playerHealth.OnHpLost -= UpdateHealthBar;
    }
}
