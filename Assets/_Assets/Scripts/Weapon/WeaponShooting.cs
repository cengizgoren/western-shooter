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
    public float DelayBetweenShots = 0.5f;
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
    private EventInstance weaponFireSfxInstance;
    private Animator animator;

    private float lastTimeShot = Mathf.NegativeInfinity;

    private GameObject muzzleFlashInstance;
    private GameObject singleMuzzleFlashInstance;

    private ParticleSystem gunshot;

    private void Awake()
    {
        weapon = GetComponent<Weapon>();
        weaponController = GetComponent<WeaponController>();
        weaponAmmo = GetComponent<WeaponAmmo>();
        animator = transform.root.GetComponent<Animator>();

        if (ShootType == WeaponShootType.Automatic)
        {
            if (MuzzleFlashEffect)
            {
                muzzleFlashInstance = Instantiate(MuzzleFlashEffect.ContiniousFire, WeaponMuzzle.position, WeaponMuzzle.rotation, WeaponMuzzle.transform);
            }
            else
            {
                muzzleFlashInstance = Instantiate(FallbackMuzzleFlashEffect.ContiniousFire, WeaponMuzzle.position, WeaponMuzzle.rotation, WeaponMuzzle.transform);
            }
            gunshot = muzzleFlashInstance.GetComponent<ParticleSystem>();
        }

        if (ShootType == WeaponShootType.Manual)
        {
            if (MuzzleFlashEffect)
            {
                singleMuzzleFlashInstance = MuzzleFlashEffect.SingleShot;
            }
            else
            {
                singleMuzzleFlashInstance = FallbackMuzzleFlashEffect.SingleShot;
            }
        }
    }

    private void Start()
    {
        if (projectileOwner == null)
        {
            projectileOwner = weapon.GetOwner();
        }

        weaponFireSfxInstance = FMODUnity.RuntimeManager.CreateInstance(WeaponFireSFX);
        weaponController.OnTriggerPressed += () => { TriggerSqueezed = true; };
        weaponController.OnTriggerReleased += () => { TriggerSqueezed = false; };
        weaponController.OnShootingAllowed += () => SafetyOn = false;
        weaponController.OnShootingForbidden += () => SafetyOn = true;
        TriggerSqueezed = false;
        SafetyOn = false; // If not controlled by superior object, always allow to shoot
    }

    public void Update()
    {
        switch (ShootType)
        {
            case WeaponShootType.Manual:
                if (TriggerSqueezed && !SafetyOn && !weaponAmmo.IsReloading)
                {
                    TryToSemiFire();
                    TriggerSqueezed = false;
                }
                break;

            case WeaponShootType.Automatic:
                if (TriggerSqueezed && !SafetyOn && !weaponAmmo.IsReloading)
                {
                    TryToAutoFire();
                }
                else
                {
                    StopSoundLoopIfPlaying();
                    animator.SetBool("IsShooting", false);
                    gunshot.Stop();
                }
                break;
        }
    }

    private void TryToSemiFire()
    {
        if (lastTimeShot + DelayBetweenShots < Time.time)
        {
            weaponFireSfxInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(WeaponMuzzle));
            ShootProjectile();
            Instantiate(singleMuzzleFlashInstance, WeaponMuzzle.position, WeaponMuzzle.rotation, WeaponMuzzle.transform);

            FMODUnity.RuntimeManager.PlayOneShot(WeaponFireSFX, WeaponMuzzle.position);
            weaponAmmo.Spend(1);
        }
    }

    // This class is a candidate for spliting into base class and inheriting classes.
    private void TryToAutoFire()
    {
        if (lastTimeShot + DelayBetweenShots < Time.time)
        {
            weaponFireSfxInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(WeaponMuzzle)); // The sound doesn't follow its source every update, so far it hasn't been a problem.
            ShootProjectile();

            gunshot.Play();
            StartSoundLoopIfSilent();
            weaponAmmo.Spend(1);
            animator.SetBool("IsShooting", true);
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
        Vector3 shotDirection = GetShotDirectionWithinSpread(WeaponMuzzle.transform.forward);
        ProjectileStandard newProjectile = Instantiate(ProjectilePrefab, WeaponMuzzle.position, Quaternion.LookRotation(shotDirection));
        newProjectile.Setup(projectileOwner, projectileStats);
        newProjectile.Shoot();
    }

    private Vector3 GetShotDirectionWithinSpread(Vector3 aimDirection)
    {
        float spreadAngleRatio = BulletSpreadAngle / 180f;
        Vector3 spreadWorldDirection = Vector3.Slerp(aimDirection.normalized, UnityEngine.Random.insideUnitSphere, spreadAngleRatio);
        return spreadWorldDirection;
    }

    private void OnDisable()
    {
        StopSoundLoopIfPlaying();
    }

    private void OnDestroy()
    {
        StopSoundLoopIfPlaying();
    }
}
