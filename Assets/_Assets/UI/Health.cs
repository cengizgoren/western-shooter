using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private int MaxHealthPoints = 100;
    
    private int currentHealthPoints;

    public UnityAction<int> OnHealthChanged;
    public UnityAction OnHealthLost;
    public UnityAction OnHealthDepleted;

    private void Start()
    {
        currentHealthPoints = MaxHealthPoints;
        OnHealthChanged?.Invoke(currentHealthPoints);
    }

    public void Damage(int amount)
    {
        currentHealthPoints -= amount;
        OnHealthChanged?.Invoke(currentHealthPoints);
        OnHealthLost?.Invoke();
        if (currentHealthPoints <= 0)
            OnHealthDepleted?.Invoke();
    }
}
