using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackReposition : IState
{
    private Transform playerTransform;
    private NavMeshAgent navMeshAgent;
    private TargetDetector targetDetector;
    private TargetPicker targetPicker;
    private EnemyController enemyController;

    public AttackReposition(TargetDetector targetDetector, TargetPicker targetPicker, EnemyController enemyController, Transform playerTransform, NavMeshAgent navMeshAgent)
    {
        this.playerTransform = playerTransform;
        this.targetPicker = targetPicker;
        this.navMeshAgent = navMeshAgent;
        this.targetDetector = targetDetector;
        this.enemyController = enemyController;
    }

    public void Tick()
    {
        if (targetDetector.IsPlayerInSights() && !targetDetector.IsFriendlyInSights())
        {
            enemyController.OnAttack?.Invoke(true);
        }
        else
        {
            enemyController.OnAttack?.Invoke(false);
        }

        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            navMeshAgent.ResetPath();
            if (targetPicker.GetClosestRandomPositionAroundPlayer(out Vector3 pos))
            {
                navMeshAgent.SetDestination(pos);
            }
        }
    }

    public void OnEnter()
    {
        navMeshAgent.ResetPath();
        navMeshAgent.speed = 4f;
        navMeshAgent.updateRotation = false;
        enemyController.SetTargetTransform(playerTransform);
    }

    public void OnExit()
    {
        enemyController.OnAttack?.Invoke(false);
        enemyController.ResetTargetTransform();
    }
}
