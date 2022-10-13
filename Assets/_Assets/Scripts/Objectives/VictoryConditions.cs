using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


public class VictoryConditions : MonoBehaviour
{
    public List<Objective> objectivesChain;
    public Transform objectivesParentUI;

    private IEnumerator<Objective> iterator;

    private void Start()
    {
        iterator = objectivesChain.GetEnumerator();
        if (!iterator.MoveNext())
        {
            Debug.LogError("There are no objectives present!");
        }
        ActivateFirstObjective();
    }

    private void ActivateFirstObjective()
    {
        ObjectiveUI objectiveUI = iterator.Current.Activate();
        objectiveUI.transform.SetParent(objectivesParentUI);
        iterator.Current.OnCompletion += ProgressToNextObjective;
        GameManager.Instance.UpdateVictoryCondition(VictoryState.ObjectiveInProgress);
    }

    private void ProgressToNextObjective()
    {
        iterator.Current.OnCompletion -= ProgressToNextObjective;
        if (iterator.MoveNext())
        {
            ObjectiveUI objectiveUI = iterator.Current.Activate();
            objectiveUI.transform.SetParent(objectivesParentUI);
            iterator.Current.OnCompletion += ProgressToNextObjective;
        } 
        else
        {
            GameManager.Instance.UpdateVictoryCondition(VictoryState.Won);
        }
    }
}