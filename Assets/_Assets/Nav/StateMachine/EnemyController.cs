using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private bool _rotationOrdered = false;
    [SerializeField] private float timeStartedWeaponSwitch;
    [SerializeField] private Quaternion _startingRotation;
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private Quaternion _finalRotation;

    [SerializeField] private EnemyWeaponController weapon;

    public void Awake()
    {
        GetComponent<Health>().OnHealthDepleted += Die;
    }

    public void SetAttack(bool value) 
    {
        weapon.triggerSqueezed = value;
    }

    public void Rotate()
    {
        _startingRotation = transform.rotation;
        _finalRotation = Quaternion.Euler(0, 90f, 0) * _startingRotation;
        _rotationOrdered = true;
        timeStartedWeaponSwitch = Time.time;
    }


    void Update()
    {
        if (_rotationOrdered)
        {
            HandleRotation();
        }
    }

    private void HandleRotation()
    {
        float switchingTimeFactor = Mathf.Clamp01((Time.time - timeStartedWeaponSwitch) / 1.5f);
        transform.rotation = Quaternion.Lerp(_startingRotation, _finalRotation, switchingTimeFactor);
        if (switchingTimeFactor >= 1.0f)
        {
            Debug.Log("Done");
            _rotationOrdered = false;
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
