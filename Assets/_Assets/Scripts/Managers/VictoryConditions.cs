using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VictoryConditions : MonoBehaviour
{
    public UnityAction OnTargetKilled;

    // Interfaces (IDamagable) are not serializable in unity
    public List<EnemyHealth> KillTargets = new List<EnemyHealth>();
    public List<Destructable> DestroyTargets = new List<Destructable>();

    public int KillTargetsTotal;
    public int KillTargetsCurrent;

    private int destroyTargetsCount;
    private int currentDestroyTargetsCount;

    private ObjectiveTracker objectiveTracker;

    private void Awake()
    {
        destroyTargetsCount = DestroyTargets.Count;
        currentDestroyTargetsCount = DestroyTargets.Count;

        KillTargetsTotal = KillTargets.Count;
        KillTargetsCurrent = KillTargets.Count;

        GameManager.Instance.UpdateVictoryCondition(VictoryState.ObjectiveInProgress);

        foreach (Destructable target in DestroyTargets)
        {
            target.OnHealthDepleted += TargetDestroyed;
        }

        foreach (EnemyHealth target in KillTargets)
        {
            target.OnHpDepleted += TargetKilled;
        }
    }

    private void TargetDestroyed()
    {
        currentDestroyTargetsCount--;
        if (currentDestroyTargetsCount <= 0)
        {
            GameManager.Instance.UpdateVictoryCondition(VictoryState.Won);
        }
    }

    private void TargetKilled()
    {
        KillTargetsCurrent--;
        if (KillTargetsCurrent <= 0)
        {
            GameManager.Instance.UpdateVictoryCondition(VictoryState.Won);
        }
        OnTargetKilled?.Invoke();
    }
}
