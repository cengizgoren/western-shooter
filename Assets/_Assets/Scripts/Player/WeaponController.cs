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

    public UnityAction OnShoot;
    public UnityAction<float> OnShootRecoil;

    [SerializeField] private string WeaponName;
    [SerializeField] private GameObject WeaponRoot;
    public Transform WeaponMuzzle;
    
    [Space(10)]
    [SerializeField] private WeaponShootType ShootType;
    [SerializeField] private float DelayBetweenShots = 0.5f;

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
    private Vector3 rotationMask = new Vector3(1f, 0f, 0f);

    private AudioSource shootAudioSource;
    private WeaponAmmo weaponAmmo;

    public GameObject Owner { get; set; }
    public PlayerWeaponManager PlayerWeaponSwitch { get; set; }
    public GameObject SourcePrefab { get; set; }
    public bool IsWeaponActive { get; private set; }

    private void Awake()
    {
        weaponAmmo = GetComponent<WeaponAmmo>();
        shootAudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        Controls.InputActions.Player.Shoot.performed += _ => triggerSqueezed = true;
        Controls.InputActions.Player.Shoot.canceled += _ => triggerSqueezed = false;
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
    }

    private bool TryToShoot()
    {
        if (!weaponAmmo.IsReloading && lastTimeShot + DelayBetweenShots < Time.time)
        {
            Shoot();
            weaponAmmo.Spend(1);
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
}
