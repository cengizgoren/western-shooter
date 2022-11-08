using FMODUnity;
using UnityEngine;

public enum WeaponShootType
{
    Manual,
    Automatic,
}

[CreateAssetMenu(menuName = "WeaponStats")]
public class WeaponStats : ScriptableObject
{
    [Header("Firing and projectile")]
    public Projectile Projectile;
    public WeaponShootType ShootType;
    public float TimeBetweenShots;
    public float BulletSpreadAngle;

    [Header("Ammo")]
    public int ClipSize;
    public float ReloadTime;

    [Header("Effects")]
    public MuzzleFlash MuzzleFlash;
    public EventReference WeaponFireSFX;
    public bool Looping;
}
