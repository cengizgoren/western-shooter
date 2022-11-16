using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject Owner { get; private set; }

    public void Setup(GameObject owner)
    {
        Owner = owner;
    }

}
