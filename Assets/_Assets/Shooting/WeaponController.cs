using System;
using UnityEngine;
using UnityEngine.Events;

public class WeaponController : MonoBehaviour
{
    public enum WeaponShootType
    {
        Manual,
        Automatic,
    }

    public string WeaponName;
    public GameObject WeaponRoot;
    public Transform WeaponMuzzle;

    public WeaponShootType ShootType;
    public float DelayBetweenShots = 0.5f;
    public ProjectileBase ProjectilePrefab;
    public GameObject MuzzleFlashPrefab;

    [Range(0f, 2f)] public float RecoilForce = 1.0f;

    public float AmmoReloadTime;
    public int ClipSize;

    public UnityAction OnShoot;
    public event EventHandler OnShootRecoil;

    private int m_CurrentAmmo;
    private float m_LastTimeShot = Mathf.NegativeInfinity;
    private InputActions _inputActions;
    private bool triggerSqueezed = false;

    public bool IsReloading { get; private set; }

    private float m_ReloadStartedTime;

    private void Awake()
    {
        _inputActions = new InputActions();
        IsReloading = false;
        m_CurrentAmmo = ClipSize;
    }

    private void OnEnable()
    {
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }

    private void Start()
    {
        _inputActions.Player.Shoot.performed += _ => PressShootPerformed();
        _inputActions.Player.Shoot.canceled += _ => PressShootCancelled();
    }

    public void Update()
    {
        switch (ShootType)
        {
            case WeaponShootType.Manual:
                if (triggerSqueezed)
                {
                    TryShoot();
                    triggerSqueezed = false;
                }
                break;

            case WeaponShootType.Automatic:
                if (triggerSqueezed)
                {
                    TryShoot();
                }
                break;
        }

        if (!IsReloading && m_CurrentAmmo <= 0)
        {
            IsReloading = true;
            m_ReloadStartedTime = Time.time;
        }

        if (IsReloading && m_ReloadStartedTime + AmmoReloadTime < Time.time)
        {
            m_CurrentAmmo = ClipSize;
            IsReloading = false;
        }
    }

    bool TryShoot()
    {
        if (!IsReloading && m_LastTimeShot + DelayBetweenShots < Time.time)
        {
            Shoot();
            m_CurrentAmmo -= 1;
            OnShootRecoil?.Invoke(this, EventArgs.Empty);
            return true;
        }
        return false;
    }

    void Shoot()
    {
        m_LastTimeShot = Time.time;
        ProjectileBase newProjectile = Instantiate(ProjectilePrefab, WeaponMuzzle.position, WeaponMuzzle.rotation);
        newProjectile.Shoot(this);

        if (MuzzleFlashPrefab != null)
        {
            GameObject muzzleFlashInstance = Instantiate(MuzzleFlashPrefab, WeaponMuzzle.position, WeaponMuzzle.rotation, WeaponMuzzle.transform);
            Destroy(muzzleFlashInstance, 2f);
        }

        OnShoot?.Invoke();
    }

    public void PressShootPerformed()
    {
        triggerSqueezed = true;
    }

    public void PressShootCancelled()
    {
        triggerSqueezed = false;
    }
}
