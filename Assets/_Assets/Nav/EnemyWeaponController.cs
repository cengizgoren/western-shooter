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
    [SerializeField] private float DelayBetweenShots = 0.5f;

    [SerializeField] private float BulletSpreadAngle = 1f;

    [Space(10)]
    [SerializeField] private ProjectileBase ProjectilePrefab;
    [SerializeField] private GameObject MuzzleFlashPrefab;
    [SerializeField] private AudioClip ShootSfx;

    private float lastTimeShot = Mathf.NegativeInfinity;
    private bool triggerSqueezed = false;

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
        if (lastTimeShot + DelayBetweenShots < Time.time)
        {
            Shoot();
        }

    }

    private void Shoot()
    {
        lastTimeShot = Time.time;
        Vector3 shotDirection = GetShotDirectionWithinSpread(WeaponMuzzle);
        ProjectileBase newProjectile = Instantiate(ProjectilePrefab, WeaponMuzzle.position, Quaternion.LookRotation(shotDirection));
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
