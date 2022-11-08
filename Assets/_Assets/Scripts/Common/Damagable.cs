using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Damagable : MonoBehaviour
{
    public Surface surface;
    public DestructionEffect DestructionEffect;

    public abstract void DealDamage(int amount);
    public abstract int GetCurrentHP();

}
