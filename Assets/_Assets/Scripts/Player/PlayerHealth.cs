using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : Damagable
{
    [SerializeField] private MaxHP MaxHpAsset;
    [SerializeField] private bool Invincible = false;

    public UnityAction OnHpLost;
    public UnityAction OnHpDepleted;

    public int MaxHP { get; private set; }
    public int HP { get; private set; }

    private void Start()
    {
        MaxHP = MaxHpAsset.Value;
        HP = MaxHpAsset.Value;
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
