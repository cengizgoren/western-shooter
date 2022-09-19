using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Chase : IState
{
    public float TimeTargetLost;

    private NavMeshAgent _navMeshAgent;
    private Transform _targetTransform;
    private TargetDetector _enemyDetector;

    public Chase(TargetDetector enemyDetector, Transform targetTransform, NavMeshAgent navMeshAgent)
    {
        _enemyDetector = enemyDetector;
        _targetTransform = targetTransform;
        _navMeshAgent = navMeshAgent;
    }

    public void Tick()
    {
        if (!_navMeshAgent.Raycast(_targetTransform.position, out NavMeshHit hit))
            TimeTargetLost += Time.deltaTime;
    }

    public void OnEnter()
    {
        TimeTargetLost = 0f;
        _navMeshAgent.SetDestination(_targetTransform.position);
    }

    public void OnExit()
    {
        _navMeshAgent.ResetPath();
    }

}
