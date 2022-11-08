using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : Damagable
{
    public MaxHP MaxHP;
    public int CurrentHP;

    public UnityAction<int> OnHpChanged;
    public UnityAction OnHpLost;
    public UnityAction OnHpDepleted;

    private void Start()
    {
        CurrentHP = MaxHP.Value;
        OnHpChanged?.Invoke(CurrentHP);
    }

    public override int GetCurrentHP()
    {
        return CurrentHP;
    }

    public override void DealDamage(int amount)
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
