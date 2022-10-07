using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyController : MonoBehaviour
{
    public UnityAction<bool> OnAttack;

    [SerializeField] private bool _rotationOrdered = false;
    [SerializeField] private float timeStartedWeaponSwitch;
    [SerializeField] private Quaternion _startingRotation;
    //[SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private Quaternion _finalRotation;

    [Space(10)]
    [SerializeField] private GameObject DeathVfx;
    [SerializeField] private float DeathVfxSpawnOffset = 0f;
    [SerializeField] private float DeathVfxLifetime = 5f;
    // Is this setting meaningful? If so which rotation to choose?
    [SerializeField] private Quaternion DeathVfxRotation;

    private EnemyAim enemyAim;

    public void Awake()
    {
        GetComponent<EnemyHealth>().OnHpDepleted += Die;
        enemyAim = GetComponent<EnemyAim>();
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

    public void SetTargetTransform(Transform target)
    {
        enemyAim.Target = target;
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
        if (DeathVfx)
        {
            GameObject impactVfxInstance = Instantiate(DeathVfx, transform.position + (transform.up * DeathVfxSpawnOffset), DeathVfxRotation);
            if (DeathVfxLifetime > 0)
            {
                Destroy(impactVfxInstance, DeathVfxLifetime);
            }
        }
        Destroy(gameObject);
    }
}
