using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Chase : IState
{
    public float TimeTargetLost;

    private NavMeshAgent navMeshAgent;
    private Transform playerTransform;

    public Chase(Transform playerTransform, NavMeshAgent navMeshAgent)
    {
        this.playerTransform = playerTransform;
        this.navMeshAgent = navMeshAgent;
    }

    public void Tick()
    {
        navMeshAgent.SetDestination(playerTransform.position);
    }

    public void OnEnter()
    {
        navMeshAgent.speed = 4f;
        navMeshAgent.updateRotation = true;
    }

    public void OnExit()
    {
        navMeshAgent.ResetPath();
    }
}
