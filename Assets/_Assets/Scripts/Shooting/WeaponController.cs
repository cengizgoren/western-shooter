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

    [SerializeField] private string WeaponName;
    [SerializeField] private GameObject WeaponRoot;
    [SerializeField] private Transform WeaponMuzzle;
    
    [Space(10)]
    [SerializeField] private WeaponShootType ShootType;
    [SerializeField] private float DelayBetweenShots = 0.5f;
    [SerializeField] private int ClipSize;
    [SerializeField] private float AmmoReloadTime;

    [Space(10)]
    [Range(0f, 2f)]
    [SerializeField] private float RecoilForce = 1.0f;
    [SerializeField] private float BulletSpreadAngle = 1f;

    [Space(10)]
    [SerializeField] private ProjectileStandard ProjectilePrefab;
    [SerializeField] private GameObject MuzzleFlashPrefab;
    [SerializeField] private AudioClip ShootSfx;
    [SerializeField] private AudioClip ReloadSfx;
    public Sprite Icon;

    private float lastTimeShot = Mathf.NegativeInfinity;
    private bool triggerSqueezed = false;
    private float reloadStartedTime;

    // Components
    private AudioSource shootAudioSource;
    private InputActions inputActions;

    // Events
    public UnityAction OnShoot;
    public UnityAction<float> OnShootRecoil;

    // Properties
    public GameObject Owner { get; set; }
    public DummyWeaponsManager WeaponManager { get; set; }
    public GameObject SourcePrefab { get; set; }
    public bool IsReloading { get; private set; }
    public bool IsWeaponActive { get; private set; }
    public int CurrentAmmo { get; private set; }

    private void Awake()
    {
        shootAudioSource = GetComponent<AudioSource>();
        inputActions = new InputActions();
        IsReloading = false;
        CurrentAmmo = ClipSize;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Start()
    {
        inputActions.Player.Shoot.performed += _ => PressShootPerformed();
        inputActions.Player.Shoot.canceled += _ => PressShootCancelled();
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

        if (!IsReloading && CurrentAmmo <= 0)
        {
            IsReloading = true;
            reloadStartedTime = Time.time;
            
            if (ReloadSfx)
            {
                shootAudioSource.PlayOneShot(ReloadSfx);
            }
        }

        if (IsReloading && reloadStartedTime + AmmoReloadTime < Time.time)
        {
            CurrentAmmo = ClipSize;
            IsReloading = false;
        }
    }

    bool TryShoot()
    {
        if (!IsReloading && lastTimeShot + DelayBetweenShots < Time.time)
        {
            Shoot();
            CurrentAmmo -= 1;
            OnShootRecoil?.Invoke(RecoilForce);
            return true;
        }
        return false;
    }

    void Shoot()
    {
        lastTimeShot = Time.time;
        Vector3 shotDirection = GetShotDirectionWithinSpread(WeaponMuzzle);
        ProjectileStandard newProjectile = Instantiate(ProjectilePrefab, WeaponMuzzle.position, Quaternion.LookRotation(shotDirection));
        newProjectile.Shoot(this);

        if (MuzzleFlashPrefab != null)
        {
            GameObject muzzleFlashInstance = Instantiate(MuzzleFlashPrefab, WeaponMuzzle.position, WeaponMuzzle.rotation, WeaponMuzzle.transform);
            Destroy(muzzleFlashInstance, 2f);
        }

        if (ShootSfx)
        {
            shootAudioSource.PlayOneShot(ShootSfx);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(WeaponMuzzle.position, WeaponMuzzle.forward * 100f);
    }
}
