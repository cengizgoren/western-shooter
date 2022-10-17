using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Task : MonoBehaviour
{
    public UnityAction OnCompletion;

    public ObjectiveUI ObjectiveUIPrefab;
    public EventReference ObjectiveComplete;

    public abstract ObjectiveUI Activate();
}
