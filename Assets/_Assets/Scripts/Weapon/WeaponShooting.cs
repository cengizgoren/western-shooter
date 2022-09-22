using System;
using UnityEngine;
using UnityEngine.Events;
using FMOD.Studio;

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
    [Space(10)]
    public FMODUnity.EventReference WeaponFireSFX;

    private Weapon weapon;
    private WeaponAmmo weaponAmmo;
    private WeaponController weaponController;
    private EventInstance weaponFireSfxInstance;

    private float lastTimeShot = Mathf.NegativeInfinity;

    private void Awake()
    {
        weapon = GetComponent<Weapon>();
        weaponController = GetComponent<WeaponController>();
        weaponAmmo = GetComponent<WeaponAmmo>();
    }

    private void Start()
    {
        weaponFireSfxInstance = FMODUnity.RuntimeManager.CreateInstance(WeaponFireSFX);
        weaponController.OnTriggerPressed += () => { TriggerSqueezed = true; };
        weaponController.OnTriggerReleased += () => { TriggerSqueezed = false; };
        weaponController.OnShootingAllowed += () => SafetyOn = false;
        weaponController.OnShootingForbidden += () => SafetyOn = true;
        TriggerSqueezed = false;
        SafetyOn = true;
    }

    public void Update()
    {
        switch (ShootType)
        {
            case WeaponShootType.Manual:
                if (TriggerSqueezed && !SafetyOn && !weaponAmmo.IsReloading)
                {
                    TryToShoot();
                    TriggerSqueezed = false;
                }
                break;

            case WeaponShootType.Automatic:
                if (TriggerSqueezed && !SafetyOn && !weaponAmmo.IsReloading)
                {
                    TryToShoot();
                }
                else
                {
                    StopSoundLoopIfPlaying();
                }
                break;
        }
    }

    private void TryToShoot()
    {
        if (lastTimeShot + DelayBetweenShots < Time.time)
        {
            weaponFireSfxInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(WeaponMuzzle)); // The sound doesn't follow its source every update, so far it hasn't been a problem.
            ShootProjectile();
            SpawnMuzzleFlash();
            StartSoundLoopIfSilent();
            weaponAmmo.Spend(1);
        }
    }

    private void StartSoundLoopIfSilent()
    {
        weaponFireSfxInstance.getPlaybackState(out PLAYBACK_STATE playbackState);
        if (playbackState == PLAYBACK_STATE.STOPPED || playbackState == PLAYBACK_STATE.STOPPING)
        {
            weaponFireSfxInstance.start();
        }
    }

    private void StopSoundLoopIfPlaying()
    {
        weaponFireSfxInstance.getPlaybackState(out PLAYBACK_STATE playbackState);
        if (playbackState == PLAYBACK_STATE.PLAYING)
        {
            weaponFireSfxInstance.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }

    private void ShootProjectile()
    {
        lastTimeShot = Time.time;
        Vector3 shotDirection = GetShotDirectionWithinSpread(WeaponMuzzle.transform.forward); // TODO: fix spread being unused
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
}
