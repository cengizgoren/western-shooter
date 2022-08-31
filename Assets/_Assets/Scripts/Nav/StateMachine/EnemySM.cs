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
        var enemyDetector = GetComponent<EnemyDetector>();
        var enemyController = GetComponent<EnemyController>();

        _stateMachine = new StateMachine();

        var idle = new Idle();
        var chase = new Chase(enemyDetector, _playerController.transform, navMeshAgent);
        var attack = new Attack(enemyDetector, transform, enemyController, _playerController.transform, navMeshAgent);
        var search = new Search(enemyDetector, enemyController, _playerController.transform, navMeshAgent);

        At(idle, attack, TargetDetected());
        At(attack, search, TargetObstructed());
        At(search, attack, TargetDetected());

        void At(IState to, IState from, Func<bool> condition) => _stateMachine.AddTransition(to, from, condition);

        _stateMachine.SetState(idle);

        Func<bool> TargetDetected() => () => enemyDetector.EnemyInSightRange && !enemyDetector.EnemyObstructed;
        Func<bool> TargetTooFarAway() => () => !enemyDetector.EnemyInSightRange;
        Func<bool> TargetInRange() => () => enemyDetector.EnemyInSightRange;
        Func<bool> TargetObstructed() => () => enemyDetector.EnemyObstructed;
    }

    private void Update() => _stateMachine.Tick();

}
