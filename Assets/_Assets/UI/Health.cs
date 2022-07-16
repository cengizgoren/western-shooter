using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private int MaxHealthPoints = 100;
    
    private int currentHealthPoints;

    public static UnityAction<int> OnHealthChanged;

    private void Start()
    {
        currentHealthPoints = MaxHealthPoints;
        OnHealthChanged?.Invoke(currentHealthPoints);
    }

    public void Damage(int amount)
    {
        currentHealthPoints -= amount;
        OnHealthChanged?.Invoke(currentHealthPoints);
    }
}
