using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Impact System/Impact Effect", fileName = "ImpactEffect")]
public class ImpactEffect : ScriptableObject
{
    public GameObject VFX;
    public EventReference SFX;
}
