using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AICommander : MonoBehaviour
{
    public List<EnemyStateMachine> enemyStateMachines;

    private int i = -1;

    private void Awake()
    {
        enemyStateMachines = new List<EnemyStateMachine>(FindObjectsOfType<EnemyStateMachine>());
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
