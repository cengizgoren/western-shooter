using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Attack : IState
{
    private Transform _playerTransform;
    private Transform _transform;
    private NavMeshAgent _navMeshAgent;
    private EnemyDetector _enemyDetector;
    private EnemyController _enemyController;
    private Transform transform;
    private Transform playerTransform;
    private NavMeshAgent navMeshAgent;

    public Attack(EnemyDetector enemyDetector, Transform transform, EnemyController enemyController, Transform playerTransform, NavMeshAgent navMeshAgent)
    {
        _transform = transform;
        _playerTransform = playerTransform;
        _navMeshAgent = navMeshAgent;
        _enemyDetector = enemyDetector;
        _enemyController = enemyController;
    }

    public void Tick()
    {
        LookAt(_playerTransform.position);
    }

    public void OnEnter()
    {
        Debug.Log("Attack");
        _enemyController.SetAttack(true);
    }

    public void OnExit()
    {
        _enemyDetector.SaveLastKnownPosAndDir();
        _enemyController.SetAttack(false);
    }

    private void LookAt(Vector3 lookPoint)
    {
        Vector3 direction = (lookPoint - _transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0.0f, direction.z));
        _transform.rotation = Quaternion.Slerp(_transform.rotation, lookRotation, Time.deltaTime * 100f);
    }

}
