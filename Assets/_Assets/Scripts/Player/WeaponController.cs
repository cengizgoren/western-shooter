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
    public Transform WeaponMuzzle;
    
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

    // Events
    public UnityAction OnShoot;
    public UnityAction<float> OnShootRecoil;

    // Properties
    public GameObject Owner { get; set; }
    public PlayerRotation PlayerRotation { get; set; }

    public PlayerWeaponManager PlayerWeaponSwitch { get; set; }
    public GameObject SourcePrefab { get; set; }
    public bool IsReloading { get; private set; }
    public bool IsWeaponActive { get; private set; }
    public int CurrentAmmo { get; private set; }

    private void Awake()
    {
        shootAudioSource = GetComponent<AudioSource>();
        IsReloading = false;
        CurrentAmmo = ClipSize;
    }

    private void Start()
    {
        Controls.InputActions.Player.Shoot.performed += _ => PressShootPerformed();
        Controls.InputActions.Player.Shoot.canceled += _ => PressShootCancelled();
    }

    public void Update()
    {
        switch (ShootType)
        {
            case WeaponShootType.Manual:
                if (triggerSqueezed && PlayerWeaponSwitch.SwitchState == PlayerWeaponManager.WeaponSwitchState.Up)
                {
                    TryToShoot();
                    triggerSqueezed = false;
                }
                break;

            case WeaponShootType.Automatic:
                if (triggerSqueezed && PlayerWeaponSwitch.SwitchState == PlayerWeaponManager.WeaponSwitchState.Up)
                {
                    TryToShoot();
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

        Vector3 rotationMask = new Vector3(1f, 0f, 0f);
        Vector3 muzzleToMousePoint = PlayerRotation.AimPoint - WeaponMuzzle.transform.position;
        Vector3 lookAtRotation = Quaternion.LookRotation(muzzleToMousePoint).eulerAngles;
        WeaponMuzzle.transform.localRotation = Quaternion.Euler(Vector3.Scale(lookAtRotation, rotationMask));

        Ray ray = new Ray(WeaponMuzzle.transform.position, WeaponMuzzle.transform.forward);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, 0f, 0f));
        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 groundPoint = ray.GetPoint(rayDistance);
            PlayerRotation.LaserSight.SetPosition(0, WeaponMuzzle.position);
            PlayerRotation.LaserSight.SetPosition(1, groundPoint);
        }
    }

    private bool TryToShoot()
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

    private void Shoot()
    {
        lastTimeShot = Time.time;
        Vector3 shotDirection = GetShotDirectionWithinSpread(WeaponMuzzle.transform.forward);
        ProjectileStandard newProjectile = Instantiate(ProjectilePrefab, WeaponMuzzle.position, WeaponMuzzle.rotation);
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

    public Vector3 GetShotDirectionWithinSpread(Vector3 aimDirection)
    {
        float spreadAngleRatio = BulletSpreadAngle / 180f;
        Vector3 spreadWorldDirection = Vector3.Slerp(aimDirection.normalized, UnityEngine.Random.insideUnitSphere, spreadAngleRatio);
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
