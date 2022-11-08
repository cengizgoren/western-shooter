using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : Damagable
{
    public MaxHP MaxHP;
    public int HP;
    public bool Invincible = false;

    public UnityAction OnHpLost;
    public UnityAction OnHpDepleted;

    private void Start()
    {
        HP = MaxHP.Value;
    }

    public override int GetCurrentHP()
    {
        return HP;
    }

    public override void DealDamage(int amount)
    {
        if (!Invincible)
        {
            HP -= amount;
            OnHpLost?.Invoke();
            if (HP <= 0)
            {
                OnHpDepleted?.Invoke();
            }
        }
    }

}
