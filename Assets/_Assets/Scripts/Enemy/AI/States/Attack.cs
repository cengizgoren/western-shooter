using UnityEngine;
using UnityEngine.AI;

public class Attack : IState
{
    private readonly Transform playerTransform;
    private readonly NavMeshAgent navMeshAgent;
    private readonly TargetDetector targetDetector;
    private readonly EnemyController enemyController;
    private readonly EnemyInput enemyInput;

    public Attack(TargetDetector targetDetector, EnemyController enemyController, Transform playerTransform, NavMeshAgent navMeshAgent, EnemyInput enemyInput)
    {
        this.playerTransform = playerTransform;
        this.navMeshAgent = navMeshAgent;
        this.targetDetector = targetDetector;
        this.enemyController = enemyController;
        this.enemyInput = enemyInput;
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
    }

    public void OnEnter()
    {
        navMeshAgent.ResetPath();
        navMeshAgent.updateRotation = false;
        navMeshAgent.speed = 2f;
        enemyInput.SetTargetTransform(playerTransform);
    }

    public void OnExit()
    {
        enemyController.OnAttack?.Invoke(false);
        navMeshAgent.updateRotation = true;
        enemyInput.ResetTargetTransform();
    }
}
