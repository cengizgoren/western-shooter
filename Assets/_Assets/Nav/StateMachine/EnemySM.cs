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

        At(idle, chase, TargetDetected());
        //At(chase, idle, TargetLost());
        At(chase, attack, TargetContact());
        At(attack, search, TargetObstructed());
        At(search, attack, TargetContact());

        void At(IState to, IState from, Func<bool> condition) => _stateMachine.AddTransition(to, from, condition);

        _stateMachine.SetState(idle);

        Func<bool> TargetDetected() => () => enemyDetector.EnemyInRange;
        //Func<bool> TargetLost() => () => chase.TimeTargetLost > 2f;
        Func<bool> TargetContact() => () => Vector3.Distance(transform.position, _playerController.transform.position) < 10f && !navMeshAgent.Raycast(_playerController.transform.position, out NavMeshHit hit);
        Func<bool> TargetObstructed() => () => navMeshAgent.Raycast(_playerController.transform.position, out NavMeshHit hit);
    }

    private void Update() => _stateMachine.Tick();

}
