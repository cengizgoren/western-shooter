using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Search : IState
{
    private const float WALKING_SPEED = 2f;

    private Transform _transform;
    private Transform _playerTransform;
    private NavMeshAgent _navMeshAgent;
    private EnemyDetector _enemyDetector;

    private float _prevSpeed;

    public Search(EnemyDetector enemyDetector, Transform transform, Transform playerTransform, NavMeshAgent navMeshAgent)
    {
        _enemyDetector = enemyDetector;
        _transform = transform;
        _playerTransform = playerTransform;
        _navMeshAgent = navMeshAgent;
    }

    public void Tick()
    {
        if (_navMeshAgent.remainingDistance <= 0.1f)
        {
            _navMeshAgent.SetDestination(_enemyDetector.LastKnownPosition + _enemyDetector.LastKnownVeloctity);
        }
    }

    public void OnEnter()
    {
        Debug.Log("Search");
        _prevSpeed = _navMeshAgent.speed;
        _navMeshAgent.speed = WALKING_SPEED;
        _navMeshAgent.SetDestination(_enemyDetector.LastKnownPosition);
    }

    public void OnExit()
    {
        _navMeshAgent.speed = _prevSpeed;
    }
}
