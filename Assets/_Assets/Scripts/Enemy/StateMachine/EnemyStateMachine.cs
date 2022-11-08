using System;
using UnityEngine;
using UnityEngine.AI;
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
        var messager = GetComponent<Messager>();

        stateMachine = new StateMachine();

        var idle = new Idle(messager);
        var chase = new Chase(PlayerController.transform, navMeshAgent, targetPicker);
        var attack = new Attack(targetDetector, enemyController, PlayerController.transform, navMeshAgent);
        var attackReposition = new AttackReposition(targetDetector, targetPicker, enemyController, PlayerController.transform, navMeshAgent);

        At(idle, chase, TargetDetected());
        At(idle, attackReposition, TargetContact());
        At(chase, attackReposition, TargetContact());
        At(attack, chase, TargetObstructed());
        At(attackReposition, attack, ReachedTheirDestination());
        At(attack, attackReposition, TargetTooFarAway());

        void At(IState to, IState from, Func<bool> condition) => stateMachine.AddTransition(to, from, condition);

        Func<bool> TargetDetected() => () => (targetDetector.TargetSighted && !targetDetector.TargetObstructed) || targetDetector.Alerted;
        Func<bool> TargetContact() => () => targetDetector.TargetSighted && !targetDetector.TargetObstructed;
        Func<bool> TargetObstructed() => () => targetDetector.TargetObstructed;
        Func<bool> TargetTooFarAway() => () => targetDetector.TooFarAway;
        Func<bool> ReachedTheirDestination() => () => !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance;

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
