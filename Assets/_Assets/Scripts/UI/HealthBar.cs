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
