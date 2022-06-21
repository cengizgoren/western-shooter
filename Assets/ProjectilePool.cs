using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{

    [SerializeField]
    float poolSize;

    [SerializeField]
    GameObject projectilePrefab;

    private List<Projectile> projectilesInPool;

    public static ProjectilePool Instance;


    private void Awake()
    {
        Instance = GetComponent<ProjectilePool>();
    }

    void Start()
    {
        InitializePool();
    }

    public Projectile Instantiate(Vector3 position, Quaternion rotation)
    {
        Projectile _projectile = projectilesInPool[0];
        _projectile.transform.position = position;
        _projectile.transform.rotation = rotation;
        projectilesInPool.Remove(_projectile);

        return _projectile;
    }

    public void ReturnToPool(Projectile _projectile)
    {
        _projectile.transform.position = transform.position;
        projectilesInPool.Add(_projectile);
    }

    void InitializePool()
    {
        projectilesInPool = new List<Projectile>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject _projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
            projectilesInPool.Add(_projectile.GetComponent<Projectile>());
        }
    }

}
