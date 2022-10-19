using System;
using UnityEngine;
using UnityEngine.AI;

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

    public void Awake()
    {
        targetDetector = GetComponent<TargetDetector>();
        var navMeshAgent = GetComponent<NavMeshAgent>();
        var targetPicker = GetComponent<TargetPicker>();
        var enemyController = GetComponent<EnemyController>();
        var messager = GetComponent<Messager>();

        stateMachine = new StateMachine();

        var idle = new Idle(messager);
        var chase = new Chase(PlayerController.transform, navMeshAgent);
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
}
