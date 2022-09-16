using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour, IDamagable
{
    public IntVariable MaxHP;
    public IntVariable CurrentHP;

    public UnityAction OnHpLost;
    public UnityAction OnHpDepleted;

    private void Start()
    {
        CurrentHP.RuntimeValue = CurrentHP.InitialValue;
    }

    public int GetCurrentHP()
    {
        return CurrentHP.RuntimeValue;
    }

    public void DealDamage(int amount)
    {
        CurrentHP.RuntimeValue -= amount;
        OnHpLost?.Invoke();
        if (CurrentHP.RuntimeValue <= 0)
        {
            OnHpDepleted?.Invoke();
        }
    }

}
