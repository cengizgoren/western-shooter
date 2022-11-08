using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class AICommander : MonoBehaviour
{
    private const int AVOIDANCE_PRIORITY_RANGE = 100;
    public List<EnemyStateMachine> enemyStateMachines;

    private void Awake()
    {
        int i = 0;
        enemyStateMachines = new List<EnemyStateMachine>(FindObjectsOfType<EnemyStateMachine>());
        foreach(var e in enemyStateMachines)
        {
            e.GetComponent<NavMeshAgent>().avoidancePriority = i % AVOIDANCE_PRIORITY_RANGE;
            i++;
        }
    }

    private void Update()
    {
        foreach (var e in enemyStateMachines)
        {
            if (e)
            {
                e.Tick();
            }
        }
    }
}
