using System;
using UnityEngine;
using UnityEngine.Events;
using FMOD.Studio;
using Unity.VisualScripting;

public class WeaponShooting : MonoBehaviour
{
    public enum WeaponShootType
    {
        Manual,
        Automatic,
    }

    public GameObject projectileOwner;
    public GameObject WeaponRoot;
    public Transform WeaponMuzzle;

    [Space(10)]
    public bool SafetyOn;
    public bool TriggerSqueezed;
    [Space(10)]
    public WeaponShootType ShootType;
    public float TimeBetweenShots;
    public float BulletSpreadAngle = 1f;
    [Space(10)]
    public ProjectileStandard ProjectilePrefab;
    public ProjectileStats projectileStats;
    [Space(10)]
    public MuzzleFlashEffect FallbackMuzzleFlashEffect;
    public MuzzleFlashEffect MuzzleFlashEffect;
    [Space(10)]
    public FMODUnity.EventReference WeaponFireSFX;

    private Weapon weapon;
    private WeaponAmmo weaponAmmo;
    private WeaponController weaponController;

    private float shotTimer;
    private bool isfirstShotInSeries = true;

    private void Awake()
    {
        weapon = GetComponent<Weapon>();
        weaponController = GetComponent<WeaponController>();
        weaponAmmo = GetComponent<WeaponAmmo>();
    }

    private void Start()
    {
        if (projectileOwner == null)
        {
            projectileOwner = weapon.GetOwner();
        }

        weaponController.OnTriggerPressed += SqueezeTrigger;
        weaponController.OnTriggerReleased += ReleaseTrigger;
        weaponController.OnShootingAllowed += PutSafetyOff;
        weaponController.OnShootingForbidden += PutSafetyOn;
        TriggerSqueezed = false;
        SafetyOn = false; // If not controlled by superior object, always allow to shoot
    }

    private void OnDestroy()
    {
        weaponController.OnTriggerPressed -= SqueezeTrigger;
        weaponController.OnTriggerReleased -= ReleaseTrigger;
        weaponController.OnShootingAllowed -= PutSafetyOff;
        weaponController.OnShootingForbidden -= PutSafetyOn;
    }

    private void PutSafetyOn() => SafetyOn = true;
    private void PutSafetyOff() => SafetyOn = false;
    private void SqueezeTrigger() => TriggerSqueezed = true;
    private void ReleaseTrigger() => TriggerSqueezed = false;

    public void Update()
    {
        shotTimer += Time.deltaTime;
        if (TriggerSqueezed && !SafetyOn && !weaponAmmo.IsReloading)
        {
            if (shotTimer > TimeBetweenShots)
            {
                if (isfirstShotInSeries)
                {
                    shotTimer = 0f;
                    isfirstShotInSeries = false;
                }
                else
                {
                    shotTimer = shotTimer % TimeBetweenShots;
                }

                Vector3 shotDirection = GetShotDirectionWithinSpread(WeaponMuzzle.transform.forward);
                ProjectileStandard newProjectile = Instantiate(ProjectilePrefab, WeaponMuzzle.position + projectileStats.Velocity * shotTimer * WeaponMuzzle.forward, Quaternion.LookRotation(shotDirection));
                newProjectile.Setup(projectileOwner, projectileStats);
                newProjectile.Shoot();

                FMODUnity.RuntimeManager.PlayOneShot(WeaponFireSFX, WeaponMuzzle.position);
                weaponAmmo.Spend(1);
            }

            if (ShootType == WeaponShootType.Manual)
            {
                TriggerSqueezed = false;
            }
        }
        else if (!isfirstShotInSeries && shotTimer > TimeBetweenShots)
        {
            isfirstShotInSeries = true;
        }
    }

    private Vector3 GetShotDirectionWithinSpread(Vector3 aimDirection)
    {
        float spreadAngleRatio = BulletSpreadAngle / 180f;
        Vector3 spreadWorldDirection = Vector3.Slerp(aimDirection.normalized, UnityEngine.Random.insideUnitSphere, spreadAngleRatio);
        return spreadWorldDirection;
    }
}
