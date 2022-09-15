using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamagable
{
    [SerializeField] private int MaxHealthPoints = 100;

    [SerializeField] private int CurrentHealthPoints;

    public UnityAction<int> OnHealthChanged;
    public UnityAction OnHealthLost;
    public UnityAction OnHealthDepleted;

    private void Start()
    {
        CurrentHealthPoints = MaxHealthPoints;
        OnHealthChanged?.Invoke(CurrentHealthPoints);
    }

    public void DealDamage(int amount)
    {
        CurrentHealthPoints -= amount;
        OnHealthChanged?.Invoke(CurrentHealthPoints);
        OnHealthLost?.Invoke();
        if (CurrentHealthPoints <= 0)
            OnHealthDepleted?.Invoke();
    }

    public int GetCurrentHP()
    {
        return CurrentHealthPoints;
    }
}
