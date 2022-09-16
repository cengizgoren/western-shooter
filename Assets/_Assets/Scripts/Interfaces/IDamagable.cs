using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    public void DealDamage(int amount);
    public int GetCurrentHP();
}
