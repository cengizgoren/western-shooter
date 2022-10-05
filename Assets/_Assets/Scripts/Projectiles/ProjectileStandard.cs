using FMODUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileStandard : MonoBehaviour
{
    private class DamagableObject
    {
        public Collider Collider;
        public int Damage;
        public IDamagable Damagable = null;

        public DamagableObject(Collider collider, int damage, IDamagable damagable)
        {
            Collider = collider;
            Damage = damage;
            Damagable = damagable;
        }

        public void CommitDamage()
        {
            if (Damagable != null && Damage > 0)
            {
                Damagable.DealDamage(Damage);
            }
        }
    }

    [SerializeField] private Transform Root;
    [SerializeField] private Transform Tip;
    [SerializeField] private ImpactType ImpactType;
    [SerializeField] private ImpactEffect FallbackImpactEffect;

    [Space(10)]
    [SerializeField] private int ImpactDamage;
    [SerializeField] private bool IsExplosive = false;
    [SerializeField] private bool ShowExplosionRadius = true;
    [SerializeField] private int ExplosionDamageAmount;
    [SerializeField] private float ExplosionRange;

    [Space(10)]
    [SerializeField] private float MaxLifeTime = 5f;
    [SerializeField] private float Velocity = 20f;
    [SerializeField] private float Radius = 0.01f;
    [SerializeField] private LayerMask HittableLayers = -1;

    [Space(10)]
    [SerializeField] private float ImpactVfxLifetime = 5f;

    [Space(10)]
    //[SerializeField] private bool ShowSoundRadius = true;
    [SerializeField] private LayerMask AffectedLayers = -1;
    [SerializeField] private float SoundRadius;

    private float shootTime;
    private Vector3 velocity;
    private Vector3 lastRootPosition;
    private List<Collider> ignoredColliders;
    private const QueryTriggerInteraction k_TriggerInteraction = QueryTriggerInteraction.Collide;

    public GameObject Owner { get; private set; }
    public Vector3 InitialPosition { get; private set; }
    public Vector3 InitialDirection { get; private set; }

    private void OnEnable()
    {
        Destroy(gameObject, MaxLifeTime);
    }

    private void Update()
    {
        transform.position += velocity * Time.deltaTime;

        RaycastHit closestHit = new RaycastHit();
        closestHit.distance = Mathf.Infinity;
        bool foundHit = false;

        Vector3 displacementSinceLastFrame = Tip.position - lastRootPosition;
        RaycastHit[] hits = Physics.SphereCastAll(lastRootPosition, Radius, displacementSinceLastFrame.normalized, displacementSinceLastFrame.magnitude, HittableLayers, k_TriggerInteraction);

        foreach (var hit in hits)
        {
            if (IsHitValid(hit) && hit.distance < closestHit.distance)
            {
                foundHit = true;
                closestHit = hit;
            }
        }

        if (foundHit)
        {
            // Handle case of casting while already inside a collider
            if (closestHit.distance <= 0f)
            {
                closestHit.point = Root.position;
                closestHit.normal = -transform.forward;
            }

            OnHit(closestHit.point, closestHit.normal, closestHit.collider);
        }

        lastRootPosition = Root.position;

    }

    public void Setup(GameObject owner, ProjectileStats projectileStats)
    {
        Owner = owner;
        Velocity = projectileStats.Velocity;
        ImpactDamage = projectileStats.ImpactDamage;
    }

    public void Shoot()
    {
        InitialPosition = transform.position;
        InitialDirection = transform.forward;
        OnShoot();
    }

    private void OnShoot()
    {
        shootTime = Time.time;
        lastRootPosition = Root.position;
        velocity = transform.forward * Velocity;
        ignoredColliders = new List<Collider>();
        Collider[] ownerColliders = Owner.GetComponentsInChildren<Collider>();
        ignoredColliders.AddRange(ownerColliders);
    }

    private void OnHit(Vector3 point, Vector3 normal, Collider collider)
    {
        IDamagable damagable = collider.GetComponent<IDamagable>();
        damagable?.DealDamage(ImpactDamage);

        if (collider.TryGetComponent(out IHittable hittable))
        {
            Surface surface = hittable.GetSurface();
            GameObject vfx = FallbackImpactEffect.SurfaceVFX;
            EventReference sfx = FallbackImpactEffect.SurfaceSFX;

            foreach (ImpactEffect impactEffect in surface.ImpactEffects)
            {
                if (ImpactType == impactEffect.ImpactType)
                {
                    vfx = impactEffect.SurfaceVFX;
                    sfx = impactEffect.SurfaceSFX;
                }
                break;
            }

            GameObject impactVfxInstance = Instantiate(vfx, point, Quaternion.LookRotation(Vector3.Reflect(transform.forward, normal)));
            RuntimeManager.PlayOneShot(sfx, transform.position);
            if (ImpactVfxLifetime > 0)
            {
                Destroy(impactVfxInstance, ImpactVfxLifetime);
            }
        }

        /*Collider[] hitBySoundColliders = Physics.OverlapSphere(inFrontPosition, SoundRadius, AffectedLayers);

        if (ShowSoundRadius)
        {
            DebugTools.Draw.DrawTimedCircle(point, SoundRadius, Color.blue);
        }

        foreach (Collider soundCollider in hitBySoundColliders)
        {
            EnemyDetector enemyDetector = soundCollider.GetComponent<EnemyDetector>();

            if (enemyDetector)
                enemyDetector.EnemyInLineOfSight = true;
        } */

        Vector3 inFrontPosition = point + normal;
        if (IsExplosive)
        {
            Collider[] hitByExplosionColliders = Physics.OverlapSphere(inFrontPosition, ExplosionRange, HittableLayers);
            Dictionary<int, DamagableObject> collidersDictionary = hitByExplosionColliders.ToDictionary(x => x.GetInstanceID(), x => new DamagableObject(x, -1, x.GetComponent<IDamagable>()));

            if (ShowExplosionRadius)
            {
                DebugTools.Draw.DrawTimedCircle(point, ExplosionRange, Color.red);
            }

            foreach (Collider explosionCollider in hitByExplosionColliders)
            {
                Vector3 direction = (explosionCollider.bounds.center - inFrontPosition).normalized;
                RaycastHit[] hitInfos = Physics.RaycastAll(inFrontPosition, new Vector3(direction.x, 0, direction.z), ExplosionRange, HittableLayers);
                hitInfos = hitInfos.OrderBy(hit => hit.distance).ToArray();
                int expolsionDamagePointsLeft = ExplosionDamageAmount;

                foreach (RaycastHit raycastHit in hitInfos)
                {
                    int id = raycastHit.collider.GetInstanceID();
                    if (expolsionDamagePointsLeft > 0 && collidersDictionary[id].Damagable != null)
                    {
                        int damageToDeal = Mathf.Min(collidersDictionary[id].Damagable.GetCurrentHP(), expolsionDamagePointsLeft);
                        collidersDictionary[id].Damage = Mathf.Max(collidersDictionary[id].Damage, damageToDeal);
                        expolsionDamagePointsLeft -= damageToDeal;
                    }
                }

            }

            foreach (DamagableObject damagableObject in collidersDictionary.Values)
            {
                damagableObject.CommitDamage();
            }
        }

        Destroy(gameObject);
    }

    private bool IsHitValid(RaycastHit hit)
    {
        if (ignoredColliders != null && ignoredColliders.Contains(hit.collider))
        {
            return false;
        }

        return true;
    }
}
