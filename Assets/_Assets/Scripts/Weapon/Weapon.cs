using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Sprite WeaponIconSprite;
    private GameObject Owner;

    public void Setup(GameObject owner)
    {
        Owner = owner;
    }

    public GameObject GetOwner()
    {
        return Owner;
    }
}
