using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

[CreateAssetMenu(menuName = "Impact System/Surface", fileName = "Surface")]
public class Surface : ScriptableObject
{

    public List<ImpactEffect> ImpactEffects = new List<ImpactEffect>();
}
