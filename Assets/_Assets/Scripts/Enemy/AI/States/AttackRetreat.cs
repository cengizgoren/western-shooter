using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AttackRetreat : IState
{
    private readonly Transform playerTransform;
    private readonly TargetPicker targetPicker;
    private readonly NavMeshAgent navMeshAgent;
    private readonly TargetDetector targetDetector;
    private readonly EnemyController enemyController;
    private readonly EnemyInput enemyInput;

    private float cooldown = 0.2f;
    private float lastRepositionTime = Mathf.NegativeInfinity;

    public AttackRetreat(Transform playerTransform, TargetDetector targetDetector, TargetPicker targetPicker, EnemyController enemyController, NavMeshAgent navMeshAgent, EnemyInput enemyInput)
    {
        this.playerTransform = playerTransform;
        this.targetDetector = targetDetector;
        this.targetPicker = targetPicker;
        this.enemyController = enemyController;
        this.navMeshAgent = navMeshAgent;
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

        if (Time.time - lastRepositionTime > cooldown)
        {
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {

                if (targetPicker.TryFindPositionAwayFromPlayer(out Vector3 pos))
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
        navMeshAgent.updateRotation = false;
        navMeshAgent.speed = 4f;
        enemyInput.SetTargetTransform(playerTransform);
    }

    public void OnExit()
    {
        enemyController.OnAttack?.Invoke(false);
        navMeshAgent.updateRotation = true;
        enemyInput.ResetTargetTransform();
    }
}
