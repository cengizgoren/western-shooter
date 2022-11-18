using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(TargetDetector))]
[RequireComponent(typeof(TargetPicker))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyController))]
[RequireComponent(typeof(Messager))]
public class EnemyStateMachine : MonoBehaviour
{
    public CharacterController PlayerController;

    private StateMachine stateMachine;
    private TargetDetector targetDetector;
    private NavMeshAgent navMeshAgent;

    [Header("Debug")]
    public DebugItem NavMeshAgentPath;
    public bool StateLabel;

    public void Awake()
    {
        targetDetector = GetComponent<TargetDetector>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        var targetPicker = GetComponent<TargetPicker>();
        var enemyController = GetComponent<EnemyController>();
        var enemyAim = GetComponent<EnemyInput>();
        var messager = GetComponent<Messager>();

        stateMachine = new StateMachine();

        var idle = new Idle(messager);
        var chase = new Chase(PlayerController.transform, navMeshAgent, targetPicker);
        var attackStandStill = new AttackStandStill(targetDetector, enemyController, PlayerController.transform, navMeshAgent, enemyAim);
        var attackAdvance = new AttackAdvance(targetDetector, targetPicker, enemyController, PlayerController.transform, navMeshAgent, enemyAim);
        var attackRetreat = new AttackRetreat(targetDetector, targetPicker, enemyController, PlayerController.transform, navMeshAgent, enemyAim);

        At(idle, chase, TargetDetected());
        At(idle, attackAdvance, TargetContact());
        
        At(chase, attackAdvance, TargetContact());

        At(attackAdvance, chase, TargetObstructed());
        At(attackAdvance, attackStandStill, ReachedTheirDestination());
        At(attackAdvance, attackRetreat, TargetTooClose());

        At(attackRetreat, chase, TargetObstructed());
        At(attackRetreat, attackStandStill, Retreated());
        At(attackRetreat, attackAdvance, TargetTooFarAway());

        At(attackStandStill, chase, TargetObstructed());
        At(attackStandStill, attackAdvance, TargetTooFarAway());
        At(attackStandStill, attackRetreat, TargetTooClose());

        void At(IState to, IState from, Func<bool> condition) => stateMachine.AddTransition(to, from, condition);

        Func<bool> TargetDetected() => () => (targetDetector.TargetSighted && !targetDetector.TargetObstructed) || targetDetector.Alerted;
        Func<bool> TargetContact() => () => targetDetector.TargetSighted && !targetDetector.TargetObstructed;
        Func<bool> TargetObstructed() => () => targetDetector.TargetObstructed;
        Func<bool> TargetTooFarAway() => () => targetDetector.TooFarAway;
        Func<bool> TargetTooClose() => () => targetDetector.TooClose;
        Func<bool> ReachedTheirDestination() => () => !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance;
        Func<bool> Retreated() => () => (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) || targetDetector.TooFarAway;
        
        stateMachine.SetState(idle);
    }

    public void Tick()
    {
        targetDetector.Tick();
        stateMachine.Tick();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (StateLabel)
            {
                Handles.Label(transform.position + Vector3.up, stateMachine.CurrentState.GetType().Name);
            }

            if (NavMeshAgentPath.Show)
            {
                if (navMeshAgent.hasPath)
                {
                    NavMeshPath path = navMeshAgent.path;

                    if (path.corners.Length >= 2)
                    {
                        Debug.DrawLine(path.corners[0], path.corners[1], NavMeshAgentPath.Color);
                    }

                    for (int i = 1; i < path.corners.Length - 1; i++)
                    {
                        Debug.DrawLine(path.corners[i], path.corners[i + 1], NavMeshAgentPath.Color);
                    }
                }
            }
        }
    }
#endif
}
