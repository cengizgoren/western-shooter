using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Idle : IState
{
    private readonly DebugAI debug;
    private readonly Messager messager;

    public Idle(DebugAI debug, Messager messager)
    {
        this.debug = debug;
        this.messager = messager;
    }

    public void Tick()
    {

    }

    public void OnEnter()
    {

    }

    public void OnExit()
    {
        messager.SendMessage();
    }
}
