using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private ParticleSystem ImpactParticleSystem;
    [SerializeField] private float projectileSpeed;

    private int damage;
    private Vector3 direction;

    public static Bullet Create(Vector3 position, Quaternion rotation, Vector3 direction, int damage)
    {
        Transform bulletTransform = Instantiate(GameAssets.i.pfBullet, position, rotation);
        Bullet bullet = bulletTransform.GetComponent<Bullet>();
        bullet.Setup(damage, direction);

        return bullet;
    }

    private void Update()
    {
        transform.position += projectileSpeed * Time.deltaTime * direction;
        Destroy(gameObject, 2f);
    }

    private void Setup(int damage, Vector3 direction)
    {
        this.damage = damage;
        this.direction = direction;
    }

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            //Debug.DrawRay(contact.point, contact.normal, Color.white);
            Instantiate(ImpactParticleSystem, contact.point + contact.normal * 0.01f, Quaternion.LookRotation(contact.normal, Vector3.up));
            TargetPractice targetPractice = contact.otherCollider.GetComponent<TargetPractice>();
            if (targetPractice != null)
            {
                targetPractice.Damage(damage);
            }

        }
        Destroy(gameObject);
    }
}