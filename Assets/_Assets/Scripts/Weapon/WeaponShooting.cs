using System;
using UnityEngine;
using UnityEngine.Events;

public class WeaponShooting : MonoBehaviour
{
    public enum WeaponShootType
    {
        Manual,
        Automatic,
    }

    public GameObject WeaponRoot;
    public Transform WeaponMuzzle;

    [Space(10)]
    public bool SafetyOn;
    public bool TriggerSqueezed;
    [Space(10)]
    public WeaponShootType ShootType;
    public float DelayBetweenShots = 0.5f;
    public float BulletSpreadAngle = 1f;
    [Space(10)]
    public ProjectileStandard ProjectilePrefab;
    public GameObject MuzzleFlashPrefab;
    public AudioClip ShootSfx;

    private AudioSource shootAudioSource;
    private Weapon weapon;
    private WeaponAmmo weaponAmmo;
    private WeaponController weaponController;


    private float lastTimeShot = Mathf.NegativeInfinity;

    private void Awake()
    {
        weapon = GetComponent<Weapon>();
        weaponController = GetComponent<WeaponController>();
        weaponAmmo = GetComponent<WeaponAmmo>();
        shootAudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        weaponController.OnTriggerPressed += () => TriggerSqueezed = true;
        weaponController.OnTriggerReleased += () => TriggerSqueezed = false;
        weaponController.OnShootingAllowed += () => SafetyOn = false;
        weaponController.OnShootingForbidden += () => SafetyOn = true;
        SafetyOn = true;
        TriggerSqueezed = false;
    }

    public void Update()
    {
        switch (ShootType)
        {
            case WeaponShootType.Manual:
                if (TriggerSqueezed && !SafetyOn)
                {
                    TryToShoot();
                    TriggerSqueezed = false;
                }
                break;

            case WeaponShootType.Automatic:
                if (TriggerSqueezed && !SafetyOn)
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
            ShootProjectile();
            SpawnMuzzleFlash();
            PlayGunShot();
            weaponAmmo.Spend(1);
            return true;
        }
        return false;
    }

    private void ShootProjectile()
    {
        lastTimeShot = Time.time;
        Vector3 shotDirection = GetShotDirectionWithinSpread(WeaponMuzzle.transform.forward);

        ProjectileStandard newProjectile = Instantiate(ProjectilePrefab, WeaponMuzzle.position, WeaponMuzzle.rotation);
        newProjectile.Setup(weapon.GetOwner());
        newProjectile.Shoot();
    }

    private Vector3 GetShotDirectionWithinSpread(Vector3 aimDirection)
    {
        float spreadAngleRatio = BulletSpreadAngle / 180f;
        Vector3 spreadWorldDirection = Vector3.Slerp(aimDirection.normalized, UnityEngine.Random.insideUnitSphere, spreadAngleRatio);
        return spreadWorldDirection;
    }

    private void SpawnMuzzleFlash()
    {
        if (MuzzleFlashPrefab != null)
        {
            GameObject muzzleFlashInstance = Instantiate(MuzzleFlashPrefab, WeaponMuzzle.position, WeaponMuzzle.rotation, WeaponMuzzle.transform);
            Destroy(muzzleFlashInstance, 2f);
        }
    }

    private void PlayGunShot()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/AK_Shoot", transform.position);
    }
}
