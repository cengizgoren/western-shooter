using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemySM : MonoBehaviour
{
    public CharacterController PlayerController;
    public string State;

    private StateMachine StateMachine;

    public void Awake()
    {
        var navMeshAgent = GetComponent<NavMeshAgent>();
        var targetDetector = GetComponent<TargetDetector>();
        var enemyController = GetComponent<EnemyController>();
        var messager = GetComponent<Messager>();

        StateMachine = new StateMachine();

        var idle = new Idle(messager);
        var chase = new Chase(targetDetector, PlayerController.transform, navMeshAgent);
        var attack = new Attack(targetDetector, transform, enemyController, PlayerController.transform, navMeshAgent);
        var search = new Search(targetDetector, enemyController, PlayerController.transform, navMeshAgent);

        At(idle, chase, TargetDetected());
        At(idle, attack, TargetContact());
        At(chase, attack, TargetContact());
        At(chase, search, TargetObstructed());
        At(attack, search, TargetObstructed());
        At(search, attack, TargetContact());

        void At(IState to, IState from, Func<bool> condition) => StateMachine.AddTransition(to, from, condition);

        StateMachine.SetState(idle);

        Func<bool> TargetDetected() => () => (targetDetector.TargetSighted && !targetDetector.TargetObstructed) || targetDetector.Alerted;
        Func<bool> TargetContact() => () => targetDetector.TargetSighted && !targetDetector.TargetObstructed;
        //Func<bool> TargetTooFarAway() => () => !enemyDetector.EnemyInSightRange;
        //Func<bool> TargetInRange() => () => enemyDetector.EnemyInSightRange;
        Func<bool> TargetObstructed() => () => targetDetector.TargetObstructed;
    }

    private void Update()
    {
        StateMachine.Tick();
        State = StateMachine.CurrentState.GetType().Name;
    }

}
