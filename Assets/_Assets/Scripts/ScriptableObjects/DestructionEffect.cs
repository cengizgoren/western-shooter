using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Destruction Effect", fileName = "DestructionEffect")]
public class DestructionEffect : ScriptableObject
{
    public GameObject DestructionVFX;
    public EventReference DestructionSFX;
}
