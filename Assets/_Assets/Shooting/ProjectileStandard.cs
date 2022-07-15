using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileStandard : ProjectileBase
{
    public float MaxLifeTime = 5f;
    public Transform Root;
    public float Speed = 20f;
    public Transform Tip;
    public float Radius = 0.01f;
    public LayerMask HittableLayers = -1;
    public float ImpactVfxSpawnOffset = 0.1f;
    public float ImpactVfxLifetime = 5f;
    public GameObject ImpactVfx;

    private float m_ShootTime;
    private Vector3 m_Velocity;
    private Vector3 m_LastRootPosition;
    List<Collider> m_IgnoredColliders;

    const QueryTriggerInteraction k_TriggerInteraction = QueryTriggerInteraction.Collide;

    ProjectileBase m_ProjectileBase;

    void OnEnable()
    {
        m_ProjectileBase = GetComponent<ProjectileBase>();
        m_ProjectileBase.OnShoot += OnShoot;
        Destroy(gameObject, MaxLifeTime);
    }

    new void OnShoot()
    {
        m_ShootTime = Time.time;
        m_LastRootPosition = Root.position;
        m_Velocity = transform.forward * Speed;

        m_IgnoredColliders = new List<Collider>();
        Collider[] ownerColliders = m_ProjectileBase.Owner.GetComponentsInChildren<Collider>();
        m_IgnoredColliders.AddRange(ownerColliders);
    }

    void Update()
    {
        transform.position += m_Velocity * Time.deltaTime;
        {
            RaycastHit closestHit = new RaycastHit();
            closestHit.distance = Mathf.Infinity;
            bool foundHit = false;

            Vector3 displacementSinceLastFrame = Tip.position - m_LastRootPosition;
            RaycastHit[] hits = Physics.SphereCastAll(m_LastRootPosition, Radius, displacementSinceLastFrame.normalized, displacementSinceLastFrame.magnitude, HittableLayers,k_TriggerInteraction);

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

        void OnHit(Vector3 point, Vector3 normal, Collider collider)
        {
            TargetPractice targetPractice = collider.GetComponent<TargetPractice>();
            if (targetPractice)
            {
                targetPractice.Damage(10);
            }

            if (ImpactVfx)
            {
                GameObject impactVfxInstance = Instantiate(ImpactVfx, point + (normal * ImpactVfxSpawnOffset), Quaternion.LookRotation(normal));
                if (ImpactVfxLifetime > 0)
                {
                    Destroy(impactVfxInstance, ImpactVfxLifetime);
                }
            }

            Destroy(gameObject);
        }
    }

    private bool IsHitValid(RaycastHit hit)
    {
        if (m_IgnoredColliders != null && m_IgnoredColliders.Contains(hit.collider))
        {
            return false;
        }

        return true;
    }
}
