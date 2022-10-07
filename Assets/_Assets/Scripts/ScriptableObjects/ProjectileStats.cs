using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Projectile Stats", fileName = "ProjectileStats")]
public class ProjectileStats : ScriptableObject
{
    public int ImpactDamage;
    public float Velocity;
}
