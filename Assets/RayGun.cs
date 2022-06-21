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

            if (Physics.Raycast(ray, out hit))
            {
                shotDistance = hit.distance;
            }

            nextPossibleShootTime = Time.time + secondsBetweenShots;
            Audio.PlayOneShot(Audio.clip);
            // Debug.DrawRay(ray.origin, ray.direction * shotDistance, Color.blue, 1);
        }

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
