using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGun : Gun
{
    [SerializeField] private Transform pfBullet;
    
    protected override void PlayerShootProjectiles_OnShoot(object sender, PlayerController.OnShootEventArgs e)
    {
        if (CanShoot())
        {
            Vector3 direction = GetBulletDeviationVector(e.shootPosition, maxDeviation);
            Bullet.Create(e.gunEndPointPosition, e.shootPosition, direction,  weaponDamage);
            nextPossibleShootTime = Time.time + secondsBetweenShots;
            Audio.PlayOneShot(Audio.clip);
        }
    }

    //public void Shoot2()
    //{
    //    if (lastTimeShot + firingSpeed <= Time.time)
    //    {
    //        Projectile _projectile = ProjectilePool.Instance.Instantiate(firingPoint.position, firingPoint.rotation);
    //        _projectile.Move();
    //        lastTimeShot = Time.time;
    //    }
    //}
}
