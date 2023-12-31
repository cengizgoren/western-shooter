using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AttackReposition : IState
{
    private Transform playerTransform;
    private NavMeshAgent navMeshAgent;
    private TargetDetector targetDetector;
    private TargetPicker targetPicker;
    private EnemyController enemyController;
    private EnemyInput enemyInput;

    public AttackReposition(TargetDetector targetDetector, TargetPicker targetPicker, EnemyController enemyController, Transform playerTransform, NavMeshAgent navMeshAgent, EnemyInput enemyInput)
    {
        this.playerTransform = playerTransform;
        this.targetPicker = targetPicker;
        this.navMeshAgent = navMeshAgent;
        this.targetDetector = targetDetector;
        this.enemyController = enemyController;
        this.enemyInput = enemyInput;
    }

    private float cooldown = 0.2f;
    private float lastRepositionTime = Mathf.NegativeInfinity;

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

        if (Time.time - lastRepositionTime > cooldown)
        {
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance || targetDetector.TooFarAway || targetDetector.TooClose)
            {
                //navMeshAgent.ResetPath();
                if (targetPicker.TryFindPositionCloseToPlayer(out Vector3 pos))
                {
                    navMeshAgent.SetDestination(pos);
                    lastRepositionTime = Time.time;
                }
            }
        }
    }

    public void OnEnter()
    {
        navMeshAgent.ResetPath();
        navMeshAgent.speed = 4f;
        navMeshAgent.updateRotation = false;
        enemyInput.SetTargetTransform(playerTransform);
    }

    public void OnExit()
    {
        enemyController.OnAttack?.Invoke(false);
        enemyInput.ResetTargetTransform();
    }
}
