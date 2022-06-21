using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class RayGun : MonoBehaviour
{
    [SerializeField]
    public Transform spawn;

    public static RayGun Instance;
    public static AudioSource Audio;

    private float secondsBetweenShots;
    private float nextPossibleShootTime;

    [SerializeField]
    public float rpm;

    [SerializeField]
    private TrailRenderer BulletTrail;

    private void Start()
    {
        secondsBetweenShots = 60 / rpm;
    }

    void Awake()
    {
        Instance = GetComponent<RayGun>();
        Audio = GetComponent<AudioSource>();
    }

    public void Shoot()
    {
        if (CanShoot())
        {
            Ray ray = new Ray(spawn.position, spawn.forward);
            RaycastHit hit;
            float shotDistance = 20;
            TrailRenderer trail = Instantiate(BulletTrail, ray.origin, Quaternion.identity);

            // If a ray hit, trail to the hitpoint
            if (Physics.Raycast(ray, out hit, float.MaxValue))
            {
                shotDistance = hit.distance;
                StartCoroutine(SpawnTrail(trail, hit.point));
            }
            // If no ray hit, trail to point some distance away
            else
            {
                StartCoroutine(SpawnTrail(trail, spawn.position + spawn.forward * 60));
            }

            nextPossibleShootTime = Time.time + secondsBetweenShots;
            Audio.PlayOneShot(Audio.clip);
            Debug.DrawRay(ray.origin, ray.direction * shotDistance, Color.blue, 1);
        }
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, Vector3 hitPoint)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while(time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hitPoint, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }
        Destroy(trail.gameObject, trail.time);
    }

    private bool CanShoot()
    {
        bool canShoot = true;
        if (Time.time < nextPossibleShootTime)
        {
            canShoot = false;
        }
        return canShoot;
    }
}
