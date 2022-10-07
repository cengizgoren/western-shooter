using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Search : IState
{
    private const float WALKING_SPEED = 2f;

    private EnemyController _enemyController;
    private Transform _playerTransform;
    private NavMeshAgent _navMeshAgent;
    private TargetDetector _enemyDetector;

    private float _prevSpeed;
    private bool _reachedLastKnownPos = false;
    private float _time = 0;

    public Search(TargetDetector enemyDetector, EnemyController enemyController, Transform playerTransform, NavMeshAgent navMeshAgent)
    {
        _enemyDetector = enemyDetector;
        _enemyController = enemyController;
        _playerTransform = playerTransform;
        _navMeshAgent = navMeshAgent;
    }

    public void Tick()
    {
        if (!_reachedLastKnownPos)
        {
            if (_navMeshAgent.remainingDistance <= 0.1f)
            {
                _reachedLastKnownPos = true;
            }
        } 
        else
        {
            _time += Time.deltaTime;
            if (_time > 3.0f)
            {
                _time = 0;
                Vector3 nextPos = _enemyDetector.GetNextPositionToSearch();
                _navMeshAgent.SetDestination(nextPos);
                _reachedLastKnownPos = false;
                
            }
        }

        // look for a patrol route?
        // investigate doors/breaches?
        // go to another sector? return to original pos?
    }

    public void OnEnter()
    {
        _reachedLastKnownPos = false;
        //_prevSpeed = _navMeshAgent.speed;
        //_navMeshAgent.speed = WALKING_SPEED;
        _navMeshAgent.SetDestination(_enemyDetector.LastKnownPosition);
    }

    public void OnExit()
    {
        _navMeshAgent.ResetPath();
        //_navMeshAgent.speed = _prevSpeed;
        _enemyController.StopAllCoroutines();
    }
}
