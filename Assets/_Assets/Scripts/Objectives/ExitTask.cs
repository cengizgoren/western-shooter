using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Task))]
public class ExitTask : Task
{
    public ExitZone exit;
    public string ObjectiveText;

    private ObjectiveUI objectiveUIInstance;

    private void OnDestroy()
    {
        if (exit != null)
        {
            exit.OnPlayerInExitZone -= ExitReached;
        }
    }

    public override ObjectiveUI Activate()
    {
        exit.gameObject.SetActive(true);
        exit.OnPlayerInExitZone += ExitReached;
        objectiveUIInstance = Instantiate(ObjectiveUIPrefab);
        objectiveUIInstance.Setup(ObjectiveText, "", "", "");
        return objectiveUIInstance;
    }

    private void ExitReached()
    {
        objectiveUIInstance.MarkAsCompleted();
        RuntimeManager.PlayOneShot(ObjectiveComplete);
        OnCompletion?.Invoke();
    }
}
