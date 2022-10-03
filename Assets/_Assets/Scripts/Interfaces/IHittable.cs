using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHittable
{
    public GameObject GetImpactVFX(ImpactType impactType);
}
