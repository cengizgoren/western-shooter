using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Chase : IState
{
    public float TimeTargetLost;

    private NavMeshAgent navMeshAgent;
    private Transform playerTransform;
    private TargetPicker targetPicker;

    private float time;
    private float setDestinationInterval = 0.5f;
    private Vector3 destination;

    public Chase(Transform playerTransform, NavMeshAgent navMeshAgent, TargetPicker targetPicker)
    {
        this.playerTransform = playerTransform;
        this.navMeshAgent = navMeshAgent;
        this.targetPicker = targetPicker;
    }

    public void Tick()
    {
        if (time > setDestinationInterval)
        {
            //if (targetPicker.GetClosestRandomPositionAroundPlayer(out destination))
            //{
                navMeshAgent.SetDestination(playerTransform.position);
                time = 0f;
            //}
        }
        else
        {
            time += Time.deltaTime;
        }
    }

    public void OnEnter()
    {
        time = Mathf.Infinity;
        navMeshAgent.speed = 4f;
        navMeshAgent.updateRotation = true;
    }

    public void OnExit()
    {
        navMeshAgent.ResetPath();
    }
}
