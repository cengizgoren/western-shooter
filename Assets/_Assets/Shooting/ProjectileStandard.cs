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
                if (hit.distance < closestHit.distance)
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
}
