using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPractice : MonoBehaviour
{
    void Start()
    {

    }

    private void Update()
    {
       
    }

    internal void Damage(int damageAmount)
    {
        DamagePopup.Create(transform.position, damageAmount);
    }
}
