using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Chase : IState
{
    public float TimeTargetLost;

    private NavMeshAgent navMeshAgent;
    private Transform targetTransform;
    private TargetDetector enemyDetector;
    private float time;

    public Chase(TargetDetector enemyDetector, Transform targetTransform, NavMeshAgent navMeshAgent)
    {
        this.enemyDetector = enemyDetector;
        this.targetTransform = targetTransform;
        this.navMeshAgent = navMeshAgent;
    }

    public void Tick()
    {
        if (time > 0.1f)
        {
            navMeshAgent.SetDestination(targetTransform.position);
            time = 0;
        }

        time += Time.deltaTime;
    }

    public void OnEnter()
    {
        TimeTargetLost = 0f;
        navMeshAgent.SetDestination(targetTransform.position);
    }

    public void OnExit()
    {
        navMeshAgent.ResetPath();
    }
}
