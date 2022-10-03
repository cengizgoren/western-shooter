using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hittable : MonoBehaviour, IHittable
{
    public Surface surface;

    public GameObject GetImpactVFX(ImpactType impactType)
    {
        foreach (Surface.SurfaceImpactEffect impactTypeEffect in surface.ImpactTypeEffects)
        {
            if (impactTypeEffect.ImpactType == impactType)
            {
                return impactTypeEffect.SurfaceVFX;
            }
        }
        Debug.LogWarningFormat("Impact type: {0} not found", impactType.name);
        return null;

    }
}
