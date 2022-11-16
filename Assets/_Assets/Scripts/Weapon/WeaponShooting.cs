using UnityEngine;

public class WeaponShooting : MonoBehaviour
{
    public WeaponStats WeaponStats;
    public GameObject ProjectileOwner;
    public GameObject WeaponRoot;
    public Transform WeaponMuzzle;

    [Header("Debug")]
    public bool SafetyOn;
    public bool TriggerSqueezed;

    private Weapon weapon;
    private WeaponAmmo weaponAmmo;
    private WeaponController weaponController;

    private GameObject muzzleFlashPrefab;
    private ParticleSystem muzzleFlashParticles;

    private float shotTimer;
    private bool isfirstShotInSeries = true;

    private void Awake()
    {
        weapon = GetComponent<Weapon>();
        weaponController = GetComponent<WeaponController>();
        weaponAmmo = GetComponent<WeaponAmmo>();

        if (WeaponStats.MuzzleFlash)
        {
            muzzleFlashPrefab = Instantiate(WeaponStats.MuzzleFlash.Prefab, WeaponMuzzle.position, WeaponMuzzle.rotation, WeaponMuzzle.transform);
            muzzleFlashParticles = muzzleFlashPrefab.GetComponent<ParticleSystem>();
        }
    }

    private void Start()
    {
        if (ProjectileOwner == null)
        {
            ProjectileOwner = weapon.Owner;
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
            if (shotTimer > WeaponStats.TimeBetweenShots)
            {
                if (isfirstShotInSeries)
                {
                    shotTimer = 0f;
                    isfirstShotInSeries = false;
                }
                else
                {
                    shotTimer = shotTimer % WeaponStats.TimeBetweenShots;
                }

                Vector3 shotDirection = GetShotDirectionWithinSpread(WeaponMuzzle.transform.forward);
                ProjectileStandard newProjectile = Instantiate(WeaponStats.Projectile.Prefab, WeaponMuzzle.position + WeaponStats.Projectile.Velocity * shotTimer * WeaponMuzzle.forward, Quaternion.LookRotation(shotDirection));
                newProjectile.Setup(ProjectileOwner, WeaponStats.Projectile);
                newProjectile.Shoot();

                muzzleFlashParticles.Play();
                FMODUnity.RuntimeManager.PlayOneShot(WeaponStats.WeaponFireSFX, WeaponMuzzle.position);
                weaponAmmo.Spend(1);
            }

            if (WeaponStats.ShootType == WeaponShootType.Manual)
            {
                TriggerSqueezed = false;
            }
        }
        else if (!isfirstShotInSeries && shotTimer > WeaponStats.TimeBetweenShots)
        {
            isfirstShotInSeries = true;
        }
    }

    private Vector3 GetShotDirectionWithinSpread(Vector3 aimDirection)
    {
        float spreadAngleRatio = WeaponStats.BulletSpreadAngle / 180f;
        Vector3 spreadWorldDirection = Vector3.Slerp(aimDirection.normalized, UnityEngine.Random.insideUnitSphere, spreadAngleRatio);
        return spreadWorldDirection;
    }
}
