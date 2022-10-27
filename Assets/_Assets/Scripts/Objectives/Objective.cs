using UnityEngine;
using UnityEngine.Events;

public class Objective : MonoBehaviour
{
    public UnityAction OnCompletion;
    private Task task;

    private void Awake()
    {
        task = GetComponent<Task>();
    }

    private void OnDestroy()
    {
        task.OnCompletion -= Complete;
    }

    public ObjectiveUI Activate()
    {
        task.OnCompletion += Complete;
        return task.Activate();
    }

    private void Complete()
    {
        OnCompletion?.Invoke();
    }
}
