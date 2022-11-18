using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class AICommander : MonoBehaviour
{
    private const int AVOIDANCE_PRIORITY_RANGE = 100;
    public List<EnemyStateMachine> enemyStateMachines;

    private int i;

    private void Awake()
    {
        int i = 0;
        enemyStateMachines = new List<EnemyStateMachine>(FindObjectsOfType<EnemyStateMachine>());
        foreach (var e in enemyStateMachines)
        {
            e.GetComponent<NavMeshAgent>().avoidancePriority = i % AVOIDANCE_PRIORITY_RANGE;
            i++;
        }
    }

    private void Start()
    {
        i = 0;
    }

    private void Update()
    {
        if (i >= enemyStateMachines.Count)
            i = 0;

        // TODO: Account for null elements skipped
        // TODO: Make more roboust solution for a list of references to enemies

        if (enemyStateMachines[i])
            enemyStateMachines[i].Tick();

        i++;
    }
}
