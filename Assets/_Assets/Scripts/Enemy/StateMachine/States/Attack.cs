using UnityEngine;
using UnityEngine.AI;

public class Attack : IState
{
    private readonly DebugAI debug;
    private readonly Transform playerTransform;
    private readonly NavMeshAgent navMeshAgent;
    private readonly TargetDetector targetDetector;
    private readonly EnemyController enemyController;

    public Attack(DebugAI debug, TargetDetector targetDetector, EnemyController enemyController, Transform playerTransform, NavMeshAgent navMeshAgent)
    {
        this.debug = debug;
        this.playerTransform = playerTransform;
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
    }

    public void OnEnter()
    {
        navMeshAgent.ResetPath();
        navMeshAgent.updateRotation = false;
        navMeshAgent.speed = 2f;
        enemyController.SetTargetTransform(playerTransform);
    }

    public void OnExit()
    {
        enemyController.OnAttack?.Invoke(false);
        navMeshAgent.updateRotation = true;
        enemyController.ResetTargetTransform();
    }
}
