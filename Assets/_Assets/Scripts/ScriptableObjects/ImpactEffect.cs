using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Impact System/Impact Effect", fileName = "ImpactEffect")]
public class ImpactEffect : ScriptableObject
{
    public ImpactType ImpactType;
    public GameObject SurfaceVFX;
    public EventReference SurfaceSFX;
}
