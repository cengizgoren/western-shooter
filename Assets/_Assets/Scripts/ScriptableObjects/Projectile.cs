using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Projectile", fileName = "Projectile")]
public class Projectile : ScriptableObject
{
    public ProjectileStandard Prefab;
    public int ImpactDamage;
    public float Velocity;
}
