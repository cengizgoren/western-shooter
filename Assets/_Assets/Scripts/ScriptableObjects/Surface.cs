using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

[CreateAssetMenu(menuName = "Impact System/Surface", fileName = "Surface")]
public class Surface : ScriptableObject
{
    [Serializable]
    public class SurfaceImpactEffect
    {
        public ImpactType ImpactType;
        public GameObject SurfaceVFX;
        public EventReference SurfaceSFX;
    }

    public List<SurfaceImpactEffect> ImpactTypeEffects = new List<SurfaceImpactEffect>();
}
