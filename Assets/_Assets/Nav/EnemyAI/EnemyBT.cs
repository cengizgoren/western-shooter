using System.Collections;
using System.Collections.Generic;
using BehaviorTree;
using UnityEngine.AI;

public class EnemyBT : Tree
{
    public UnityEngine.Transform[] waypoints;

    public static float speed = 2f;

    private NavMeshAgent agent;

    private void Awake()
    {
        
    }
    protected override Node SetupTree()
    {
        Node root = new TaskPatrol(transform, waypoints);

        return root;
    }

}
