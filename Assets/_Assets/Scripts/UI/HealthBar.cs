using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthBar;

    private PlayerHealth playerHealth;

    private void Awake()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    private void Start()
    {
        healthBar.maxValue = playerHealth.MaxHP.Value;
        healthBar.value = playerHealth.MaxHP.Value;
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
