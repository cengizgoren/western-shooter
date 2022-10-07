using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth: MonoBehaviour, IDamagable
{
    public IntVariable MaxHP;
    public int CurrentHP;

    public UnityAction<int> OnHpChanged;
    public UnityAction OnHpLost;
    public UnityAction OnHpDepleted;

    private void Start()
    {
        CurrentHP = MaxHP.InitialValue;
        OnHpChanged?.Invoke(CurrentHP);
    }

    public int GetCurrentHP()
    {
        return CurrentHP;
    }

    public void DealDamage(int amount)
    {
        CurrentHP -= amount;
        OnHpChanged?.Invoke(CurrentHP);
        OnHpLost?.Invoke();
        if (CurrentHP <= 0) 
        { 
            OnHpDepleted?.Invoke();
        }
    }
}
