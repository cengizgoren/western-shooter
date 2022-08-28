using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum GunType {Semi, Burst, Auto};
    
    public AudioSource Audio;
    private Animator animator;
    
    public int weaponDamage;
    public GunType gunType;
    [SerializeField] private int gunID;
    [SerializeField] public float maxDeviation = 2f;
    [SerializeField] public Transform shootingPoint;
    [SerializeField] private float rpm;
    [SerializeField] private TrailRenderer BulletTrail;
    [SerializeField] private ParticleSystem ImpactParticleSystem;
    [SerializeField] private ParticleSystem MuzzleFlashParticleSystem;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float hitscanBulletTrailSpeed;

    protected float secondsBetweenShots;
    protected float nextPossibleShootTime;

    private void Awake()
    {
        Audio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        secondsBetweenShots = 60 / rpm;
        GetComponentInParent<PlayerController>().OnShoot += PlayerShootProjectiles_OnShoot;
        animator = transform.root.GetComponentInChildren<Animator>();
    }

    private void OnDestroy()
    {
        GetComponentInParent<PlayerController>().OnShoot -= PlayerShootProjectiles_OnShoot;
    }

    protected virtual void PlayerShootProjectiles_OnShoot(object sender, PlayerController.OnShootEventArgs e)
    {
        if (CanShoot())
        {
            RaycastHit hit;
            Ray ray = new Ray(shootingPoint.position, GetBulletDeviationVector(shootingPoint.transform.rotation, maxDeviation));
            TrailRenderer trail = Instantiate(BulletTrail, ray.origin, Quaternion.identity);
            Instantiate(MuzzleFlashParticleSystem, ray.origin, transform.rotation);

            if (Physics.Raycast(ray, out hit, float.MaxValue, LayerMask.GetMask("Default")))
            {
                Debug.DrawLine(shootingPoint.position, hit.point, Color.cyan, 1f);
                Debug.Log(hit.collider.name);
                Instantiate(ImpactParticleSystem, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(hit.normal, Vector3.up));
                StartCoroutine(SpawnTrail(trail, hit.point));

                TargetPractice targetPractice = hit.collider.GetComponent<TargetPractice>();
                if (targetPractice != null)
                    targetPractice.DealDamage(weaponDamage);
            }
            else
            {
                StartCoroutine(SpawnTrail(trail, shootingPoint.position + shootingPoint.forward * 60f));
            }
            nextPossibleShootTime = Time.time + secondsBetweenShots;
            Audio.PlayOneShot(Audio.clip);
        }
    }

    protected bool CanShoot()
    {
        bool canShoot = true;
        if (Time.time < nextPossibleShootTime)
        {
            canShoot = false;
        }
        return canShoot;
    }

    protected Vector3 GetBulletDeviationVector(Quaternion rotation, float deviationInDeg)
    {
        Vector3 forwardVector = Vector3.forward;
        float deviation = Random.Range(0f, deviationInDeg);
        float angle = Random.Range(0f, 360f);
        forwardVector = Quaternion.AngleAxis(deviation, Vector3.up) * forwardVector;
        forwardVector = Quaternion.AngleAxis(angle, Vector3.forward) * forwardVector;
        forwardVector = rotation * forwardVector;
        return forwardVector;
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, Vector3 hitPoint)
    {
        float time = 0;
        while (time < 0.5f && trail.transform.position != hitPoint)
        {
            trail.transform.position = Vector3.MoveTowards(trail.transform.position, hitPoint, Time.deltaTime * hitscanBulletTrailSpeed);
            time += Time.deltaTime;

            yield return null;
        }
        Destroy(trail.gameObject);
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying == false)
        {
            return;
        }

        var screenRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(screenRay, out var hitInfo, float.MaxValue, groundMask))
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(shootingPoint.position, hitInfo.point);
        }

        Ray firingPointRay = new Ray(shootingPoint.position, shootingPoint.forward);
        if (Physics.Raycast(firingPointRay, out var hitInfo2, float.MaxValue))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(shootingPoint.position, hitInfo2.point);

            // Trygonometric equation for max bullet spread, b = a * tan(Beta) a| _b \c 
            float radius = hitInfo2.distance * Mathf.Tan(Mathf.Deg2Rad * maxDeviation);

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(hitInfo2.point, radius);
        }
    }
}
