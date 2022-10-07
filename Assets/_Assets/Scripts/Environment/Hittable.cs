using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hittable : MonoBehaviour, IHittable
{
    public Surface surface;

    public Surface GetSurface()
    {
        return surface;
    }
}
