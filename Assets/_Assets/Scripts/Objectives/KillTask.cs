using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Task))]
public class KillTask : Task
{
    public List<Health> KillList;
    public string ObjectiveText;

    private int killsCurrent;
    private int killsTotal;
    private ObjectiveUI objectiveUIInstance;

    private void Awake()
    {
        if (KillList == null || KillList.Capacity == 0)
        {
            //Debug.LogErrorFormat("There are no designated targets in {0}", name);
            KillList = FindObjectsOfType<EnemyController>().Select(enemy => enemy.GetComponent<Health>()).ToList();
        }
    }

    private void OnDestroy()
    {
        foreach (Health target in KillList)
        {
            if (target != null)
            {
                target.OnHpDepleted -= TargetKilled;
            }
        }
    }

    public override ObjectiveUI Activate()
    {
        foreach (Health target in KillList)
        {
            target.OnHpDepleted += TargetKilled;
        }

        killsCurrent = 0;
        killsTotal = KillList.Capacity;
        objectiveUIInstance = Instantiate(ObjectiveUIPrefab);
        objectiveUIInstance.Setup(
            ObjectiveText, 
            killsCurrent.ToString(), 
            "/", 
            killsTotal.ToString());
        return objectiveUIInstance;
    }

    private void TargetKilled()
    {
        killsCurrent++;
        objectiveUIInstance.ObjectivesCurrent.SetText(killsCurrent.ToString());

        if (killsCurrent >= killsTotal)
        {
            objectiveUIInstance.MarkAsCompleted();
            RuntimeManager.PlayOneShot(ObjectiveComplete);
            OnCompletion?.Invoke();
        }
    }
}
