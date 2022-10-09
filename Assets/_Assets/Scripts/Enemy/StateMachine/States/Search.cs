using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Search : IState
{
    private EnemyController enemyController;
    private Transform playerTransform;
    private NavMeshAgent navMeshAgent;
    private TargetDetector enemyDetector;

    private float _prevSpeed;
    private float time = 0;

    public Search(TargetDetector enemyDetector, EnemyController enemyController, Transform playerTransform, NavMeshAgent navMeshAgent)
    {
        this.enemyDetector = enemyDetector;
        this.enemyController = enemyController;
        this.playerTransform = playerTransform;
        this.navMeshAgent = navMeshAgent;
    }

    public void Tick()
    {
        if (time > 0.3f)
        {
            // Carefull! The distance might not be ready yet here!
            if (navMeshAgent.remainingDistance < 0.2f)
            {
                navMeshAgent.SetDestination(enemyDetector.GetNextPositionToSearch());
            }
            time = 0;
        }
        time += Time.deltaTime;
    }

    public void OnEnter()
    {
        navMeshAgent.SetDestination(enemyDetector.LastKnownPosition);
    }

    public void OnExit()
    {
        navMeshAgent.ResetPath();
        enemyController.StopAllCoroutines();
    }
}
