using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileStandard : ProjectileBase
{
    [SerializeField] private Transform Root;
    [SerializeField] private Transform Tip;

    [Space(10)]
    [SerializeField] private float MaxLifeTime = 5f;
    [SerializeField] private float Speed = 20f;
    [SerializeField] private float Radius = 0.01f;
    [SerializeField] private LayerMask HittableLayers = -1;

    [Space(10)]
    [SerializeField] private GameObject ImpactVfx;
    [SerializeField] private float ImpactVfxSpawnOffset = 0.1f;
    [SerializeField] private float ImpactVfxLifetime = 5f;

    [Space(10)]
    [SerializeField] private bool ShowSoundRadius = true;
    [SerializeField] private LayerMask AlertedBySound = -1;
    [SerializeField] private float SoundRadius;


    private float shootTime;
    private Vector3 velocity;
    private Vector3 lastRootPosition;
    private List<Collider> ignoredColliders;

    private const QueryTriggerInteraction k_TriggerInteraction = QueryTriggerInteraction.Collide;

    ProjectileBase m_ProjectileBase;

    void OnEnable()
    {
        m_ProjectileBase = GetComponent<ProjectileBase>();
        m_ProjectileBase.OnShoot += OnShoot;
        Destroy(gameObject, MaxLifeTime);
    }

    new private void OnShoot()
    {
        shootTime = Time.time;
        lastRootPosition = Root.position;
        velocity = transform.forward * Speed;

        ignoredColliders = new List<Collider>();
        Collider[] ownerColliders = m_ProjectileBase.Owner.GetComponentsInChildren<Collider>();
        ignoredColliders.AddRange(ownerColliders);
    }

    private void Update()
    {
        transform.position += velocity * Time.deltaTime;
        {
            RaycastHit closestHit = new RaycastHit();
            closestHit.distance = Mathf.Infinity;
            bool foundHit = false;

            Vector3 displacementSinceLastFrame = Tip.position - lastRootPosition;
            RaycastHit[] hits = Physics.SphereCastAll(lastRootPosition, Radius, displacementSinceLastFrame.normalized, displacementSinceLastFrame.magnitude, HittableLayers,k_TriggerInteraction);

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
        }
    }

    private void OnHit(Vector3 point, Vector3 normal, Collider collider)
    {
        TargetPractice targetPractice = collider.GetComponent<TargetPractice>();
        if (targetPractice)
        {
            targetPractice.Damage(10);
        }

        Health health = collider.GetComponent<Health>();
        if (health)
        {
            health.Damage(10);
        }

        Destructable destructable = collider.GetComponent<Destructable>();
        if (destructable)
        {
            destructable.Damage(10);
        }

        if (ImpactVfx)
        {
            GameObject impactVfxInstance = Instantiate(ImpactVfx, point + (normal * ImpactVfxSpawnOffset), Quaternion.LookRotation(normal));
            if (ImpactVfxLifetime > 0)
            {
                Destroy(impactVfxInstance, ImpactVfxLifetime);
            }
        }

        Collider[] hitColliders = Physics.OverlapSphere(point, SoundRadius, AlertedBySound);
        if (ShowSoundRadius)
        {
            DebugTools.DrawArc.DrawTimedCircle(point, SoundRadius, Color.red);
        }

        foreach (Collider hitCollider in hitColliders)
        {
            //Debug.Log(hitCollider.name);
            EnemyDetector enemyDetector = hitCollider.GetComponent<EnemyDetector>();
            if (enemyDetector)
            {
                enemyDetector.EnemyDetected = true;
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
