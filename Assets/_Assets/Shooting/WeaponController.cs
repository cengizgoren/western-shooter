using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
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
    public float BulletSpreadAngle = 1f;
    public ProjectileBase ProjectilePrefab;
    public GameObject MuzzleFlashPrefab;

    [Range(0f, 2f)] public float RecoilForce = 1.0f;

    public float AmmoReloadTime;
    public int ClipSize;
    public float maxDeviation;

    public AudioClip ShootSfx;
    public AudioClip ReloadSfx;

    public UnityAction OnShoot;
    public event EventHandler OnShootRecoil;

    private int m_CurrentAmmo;
    private float m_LastTimeShot = Mathf.NegativeInfinity;
    private InputActions _inputActions;
    private bool triggerSqueezed = false;

    public bool IsReloading { get; private set; }
    public GameObject Owner { get; set; }
    public DummyWeaponsManager WeaponManager { get; set; }
    public GameObject SourcePrefab { get; set; }
    public bool IsWeaponActive { get; private set; }


    private float m_ReloadStartedTime;

    AudioSource m_ShootAudioSource;

    private void Awake()
    {
        m_ShootAudioSource = GetComponent<AudioSource>();
        _inputActions = new InputActions();
        IsReloading = false;
        m_CurrentAmmo = ClipSize;
    }

    private void Start()
    {
        _inputActions.Player.Shoot.performed += _ => PressShootPerformed();
        _inputActions.Player.Shoot.canceled += _ => PressShootCancelled();
    }

    private void OnEnable()
    {
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }

    public void Update()
    {
        switch (ShootType)
        {
            case WeaponShootType.Manual:
                if (triggerSqueezed && WeaponManager.SwitchState == DummyWeaponsManager.WeaponSwitchState.Up)
                {
                    TryShoot();
                    triggerSqueezed = false;
                }
                break;

            case WeaponShootType.Automatic:
                if (triggerSqueezed && WeaponManager.SwitchState == DummyWeaponsManager.WeaponSwitchState.Up)
                {
                    TryShoot();
                }
                break;
        }

        if (!IsReloading && m_CurrentAmmo <= 0)
        {
            IsReloading = true;
            m_ReloadStartedTime = Time.time;
            
            if (ReloadSfx)
            {
                m_ShootAudioSource.PlayOneShot(ReloadSfx);
            }
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
        Vector3 shotDirection = GetShotDirectionWithinSpread(WeaponMuzzle);
        ProjectileBase newProjectile = Instantiate(ProjectilePrefab, WeaponMuzzle.position, Quaternion.LookRotation(shotDirection));
        newProjectile.Shoot(this);

        if (MuzzleFlashPrefab != null)
        {
            GameObject muzzleFlashInstance = Instantiate(MuzzleFlashPrefab, WeaponMuzzle.position, WeaponMuzzle.rotation, WeaponMuzzle.transform);
            Destroy(muzzleFlashInstance, 2f);
        }

        if (ShootSfx)
        {
            m_ShootAudioSource.PlayOneShot(ShootSfx);
        }

        OnShoot?.Invoke();
    }

    public Vector3 GetShotDirectionWithinSpread(Transform shootTransform)
    {
        float spreadAngleRatio = BulletSpreadAngle / 180f;
        Vector3 spreadWorldDirection = Vector3.Slerp(shootTransform.forward, UnityEngine.Random.insideUnitSphere, spreadAngleRatio);
        return spreadWorldDirection;
    }

    public void ShowWeapon(bool show)
    {
        WeaponRoot.SetActive(show);
        IsWeaponActive = show;
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
