using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class WeaponShooting : MonoBehaviour
{
    public enum WeaponShootType
    {
        Manual,
        Automatic,
    }

    public GameObject WeaponRoot;
    public Transform WeaponMuzzle;
    public BoolVariable AllowedToShoot;
    [Space(10)]
    public WeaponShootType ShootType;
    public float DelayBetweenShots = 0.5f;
    public float BulletSpreadAngle = 1f;
    [Space(10)]
    public ProjectileStandard ProjectilePrefab;
    public GameObject MuzzleFlashPrefab;
    public AudioClip ShootSfx;

    private AudioSource shootAudioSource;
    private WeaponAmmo weaponAmmo;
    private Weapon weapon;

    private float lastTimeShot = Mathf.NegativeInfinity;
    private bool triggerSqueezed = false;

    private void Awake()
    {
        weapon = GetComponent<Weapon>();
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
                if (triggerSqueezed && AllowedToShoot.Value)
                {
                    TryToShoot();
                    triggerSqueezed = false;
                }
                break;

            case WeaponShootType.Automatic:
                if (triggerSqueezed && AllowedToShoot.Value)
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
        if (ShootSfx)
        {
            shootAudioSource.PlayOneShot(ShootSfx);
        }
    }
}
