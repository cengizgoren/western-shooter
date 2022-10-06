using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemySM : MonoBehaviour
{
    private StateMachine _stateMachine;

    [SerializeField] private CharacterController _playerController;

    public void Awake()
    {
        var navMeshAgent = GetComponent<NavMeshAgent>();
        var targetDetector = GetComponent<TargetDetector>();
        var enemyController = GetComponent<EnemyController>();
        var messager = GetComponent<Messager>();

        _stateMachine = new StateMachine();

        var idle = new Idle(messager);
        var chase = new Chase(targetDetector, _playerController.transform, navMeshAgent);
        var attack = new Attack(targetDetector, transform, enemyController, _playerController.transform, navMeshAgent);
        var search = new Search(targetDetector, enemyController, _playerController.transform, navMeshAgent);

        At(idle, chase, TargetDetected());
        At(idle, attack, TargetContact());
        At(chase, attack, TargetContact());
        At(attack, search, TargetObstructed());
        At(search, attack, TargetContact());

        void At(IState to, IState from, Func<bool> condition) => _stateMachine.AddTransition(to, from, condition);

        _stateMachine.SetState(idle);

        Func<bool> TargetDetected() => () => (targetDetector.TargetSighted && !targetDetector.TargetObstructed) || targetDetector.Alerted;
        Func<bool> TargetContact() => () => targetDetector.TargetSighted && !targetDetector.TargetObstructed;
        //Func<bool> TargetTooFarAway() => () => !enemyDetector.EnemyInSightRange;
        //Func<bool> TargetInRange() => () => enemyDetector.EnemyInSightRange;
        Func<bool> TargetObstructed() => () => targetDetector.TargetObstructed;
    }

    private void Update() => _stateMachine.Tick();

}
