using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VictoryConditions : MonoBehaviour
{
    public UnityAction OnTargetKilled;

    // Interfaces (IDamagable) are not serializable in unity
    public List<EnemyHealth> KillTargets = new List<EnemyHealth>();
    public ExitZone exit;

    public int KillTargetsTotal;
    public int KillTargetsCurrent;

    private void Awake()
    {
        KillTargetsTotal = KillTargets.Count;
        KillTargetsCurrent = KillTargets.Count;

        GameManager.Instance.UpdateVictoryCondition(VictoryState.ObjectiveInProgress);

        foreach (EnemyHealth target in KillTargets)
        {
            target.OnHpDepleted += TargetKilled;
        }

        exit.OnPlayerInExitZone += PlayerHasReachedExit;
        
    }

    private void Start()
    {
        exit.gameObject.SetActive(false);
    }

    private void TargetKilled()
    {
        KillTargetsCurrent--;
        if (KillTargetsCurrent <= 0)
        {
            exit.gameObject.SetActive(true);

        }
        OnTargetKilled?.Invoke();
    }

    private void PlayerHasReachedExit()
    {
        GameManager.Instance.UpdateVictoryCondition(VictoryState.Won);
    }
}
