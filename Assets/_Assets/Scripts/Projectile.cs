using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    private Vector3 firingPoint;

    [SerializeField]
    private float projectileSpeed;

    [SerializeField]
    private float maxProjectileDistance;

    private bool shouldMove = false;

    void Start()
    {
        firingPoint = transform.position;
    }

    void Update()
    {
        if (shouldMove)
        {
            MoveProjectile();
        }
    }

    public void Move()
    {
        firingPoint = transform.position;
        shouldMove = true;
    }

    void MoveProjectile()
    {
        if (Vector3.Distance(firingPoint, transform.position) > maxProjectileDistance)
        {
            ProjectilePool.Instance.ReturnToPool(this);
            shouldMove = false;
        } 
        else
        {
            transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime);
        }
    }
}
