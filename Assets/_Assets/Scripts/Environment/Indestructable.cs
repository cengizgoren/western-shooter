using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshObstacle))]
public class Indestructable : Damagable
{
    public override void DealDamage(int amount)
    {
        // Ignore damage
    }

    public override int GetCurrentHP()
    {
        // Return fake HP
        return 1;
    }
}
