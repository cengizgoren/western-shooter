using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Idle : IState
{
    private readonly Messager messager;

    public Idle(Messager messager)
    {
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
        messager.SendMessage(Messager.Messages.ALERT);
    }
}
