using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponAmmo : MonoBehaviour
{
    public UnityAction<int> OnAmmoChange;

    public int ClipSize;
    public float AmmoReloadTime;
    public AudioClip ReloadSfx;

    private AudioSource audioSource;
    private float reloadStartedTime;

    public bool IsReloading { get; private set; }
    public int CurrentAmmo { get; private set; }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        IsReloading = false;
        UpdateAmmo(ClipSize);
    }

    private void Update()
    {
        CheckIfReloadNeeded();
        CheckIfReloadEnded();
    }

    public void Spend(int amount)
    {
        UpdateAmmo(CurrentAmmo - amount);
    }

    private void CheckIfReloadNeeded()
    {
        if (!IsReloading && CurrentAmmo <= 0)
        {
            reloadStartedTime = Time.time;
            IsReloading = true;

            if (ReloadSfx)
            {
                audioSource.PlayOneShot(ReloadSfx);
            }
        }
    }

    private void CheckIfReloadEnded()
    {
        if (IsReloading && reloadStartedTime + AmmoReloadTime < Time.time)
        {
            UpdateAmmo(ClipSize);
            IsReloading = false;
        }
    }

    private void UpdateAmmo(int newValue)
    {
        CurrentAmmo = newValue;
        OnAmmoChange?.Invoke(CurrentAmmo);
    }
}
