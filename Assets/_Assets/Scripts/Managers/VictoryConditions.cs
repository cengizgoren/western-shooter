using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryConditions : MonoBehaviour
{
    // Interfaces (IDamagable) are not serializable in unity
    public List<Destructable> DestroyTargets = new List<Destructable>();
    public List<EnemyHealth> KillTargets = new List<EnemyHealth>();

    private int destroyTargetsCount;
    private int killTargetsCount;

    private int currentDestroyTargetsCount;
    private int currentKillTargetsCount;

    void Start()
    {
        destroyTargetsCount = DestroyTargets.Count;
        currentDestroyTargetsCount = DestroyTargets.Count;

        killTargetsCount = KillTargets.Count;
        currentKillTargetsCount = KillTargets.Count;

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
        currentKillTargetsCount--;
        if (currentKillTargetsCount <= 0)
        {
            GameManager.Instance.UpdateVictoryCondition(VictoryState.Won);
        }
    }
}
