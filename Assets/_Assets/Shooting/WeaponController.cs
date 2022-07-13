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

    public UnityAction OnShoot;
    public event EventHandler OnShootRecoil;

    private float m_LastTimeShot = Mathf.NegativeInfinity;
    private InputActions _inputActions;
    private bool triggerSqueezed = false;

    private void Awake()
    {
        _inputActions = new InputActions();
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
    }

    public void PressShootPerformed()
    {
        triggerSqueezed = true;
    }

    public void PressShootCancelled()
    {
        triggerSqueezed = false;
    }

    bool TryShoot()
    {
        if (m_LastTimeShot + DelayBetweenShots < Time.time)
        {
            Shoot();
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
}
