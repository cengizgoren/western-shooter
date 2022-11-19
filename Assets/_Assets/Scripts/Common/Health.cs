using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private MaxHP MaxHpAsset;
    [SerializeField] private bool Invincible = false;
    [SerializeField] private bool DoNotDestroy = false;
    [SerializeField] private ImpactEffect ImpactEffectAsset;
    [SerializeField] private DestructionEffect DestructionEffectAsset;

    public UnityAction OnHpLost;
    public UnityAction OnHpDepleted;

    public int MaxHP { get; private set; }
    public int HP { get; private set; }
    public ImpactEffect ImpactEffect { get; private set; }

    private void Awake()
    {
        MaxHP = MaxHpAsset.Value;
        HP = MaxHpAsset.Value;
        ImpactEffect = ImpactEffectAsset;
    }

    public virtual void Damage(int amount)
    {
        if (!Invincible)
        {
            HP -= amount;
            OnHpLost?.Invoke();

            if (HP <= 0)
            {
                if (DoNotDestroy)
                    OnHpDepleted?.Invoke();
                else
                    Die();
            }
        }
    }
    private void Die()
    {
        OnHpDepleted?.Invoke();
        Instantiate(DestructionEffectAsset.DestructionVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
