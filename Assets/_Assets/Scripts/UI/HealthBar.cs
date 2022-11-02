using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthBar;

    private PlayerHealth playerHealth;

    private void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        UpdateHealthBar();
        playerHealth.OnHpLost += UpdateHealthBar;
    }

    private void UpdateHealthBar()
    {
        healthBar.maxValue = playerHealth.MaxHP.Value;
        healthBar.value = playerHealth.HP;
    }

    private void OnDestroy()
    {
        playerHealth.OnHpLost -= UpdateHealthBar;
    }
}
