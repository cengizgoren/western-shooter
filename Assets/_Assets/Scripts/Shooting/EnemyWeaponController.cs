using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponController : MonoBehaviour
{
    public enum WeaponShootType
    {
        Manual,
        Automatic,
    }

    [SerializeField] private string WeaponName;
    [SerializeField] private Transform WeaponMuzzle;

    [Space(10)]
    [SerializeField] private WeaponShootType ShootType;
    [SerializeField] private int BulletsPerBurst = 1;
    [SerializeField] private float DelayBetweenShots = 0.5f;
    [SerializeField] private float DelayBetweenBursts = 3f;

    [SerializeField] private float BulletSpreadAngle = 1f;

    [Space(10)]
    [SerializeField] private ProjectileStandard ProjectilePrefab;
    [SerializeField] private GameObject MuzzleFlashPrefab;
    [SerializeField] private AudioClip ShootSfx;

    private int timesShot = 0;
    private float lastTimeShot = Mathf.NegativeInfinity;
    private float timeBurstEnded = Mathf.NegativeInfinity;
    public bool triggerSqueezed = false;

    // Components
    private AudioSource shootAudioSource;

    // Properties
    public GameObject Owner { get; set; }
    public GameObject SourcePrefab { get; set; }

    private void Awake()
    {
        shootAudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (triggerSqueezed)
        {
            TryShoot();
        }
    }

    void TryShoot()
    {
        if (timesShot <= BulletsPerBurst)
        {
            if (lastTimeShot + DelayBetweenShots < Time.time)
            {
                Shoot();
                timesShot++;
            }
        }
        else
        {
            if (lastTimeShot + DelayBetweenBursts < Time.time)
            {
                timesShot = 0;
            }
        }

    }

    private void Shoot()
    {
        lastTimeShot = Time.time;
        Vector3 shotDirection = GetShotDirectionWithinSpread(WeaponMuzzle);
        ProjectileStandard newProjectile = Instantiate(ProjectilePrefab, WeaponMuzzle.position, Quaternion.LookRotation(shotDirection));

        // Hack
        this.Owner = gameObject;


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
    }

    public Vector3 GetShotDirectionWithinSpread(Transform shootTransform)
    {
        float spreadAngleRatio = BulletSpreadAngle / 180f;
        Vector3 spreadWorldDirection = Vector3.Slerp(shootTransform.forward, UnityEngine.Random.insideUnitSphere, spreadAngleRatio);
        return spreadWorldDirection;
    }
}
