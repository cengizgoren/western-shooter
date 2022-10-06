using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Messager : MonoBehaviour
{
    public UnityAction OnAlert;

    public Message message;
    [Header("Gizmos")]
    public bool ShowMessageRadius;


    // Make Messager hold mesages to sent for now, as an experiment
    public void SendMessage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, message.BroadCastRadius, message.MessageReceivers);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.TryGetComponent(out Messager messagrereceiver))
            {
                messagrereceiver.ProcessMessage(message);
            } else
            {
                Debug.LogWarningFormat("Message ({0}) broadcasted by object {1} received by object without Messager component: {2}", Enum.GetName(typeof(MessageType), message.messageType), transform.name, hitCollider.name);
            }
        }
    }

    public void ProcessMessage(Message message)
    {
        switch (message.messageType)
        {
            case MessageType.ALERT:
                OnAlert?.Invoke();
                break;
        }
    }

    private void OnDrawGizmos()
    {
        if (ShowMessageRadius)
        {
            Gizmos.color = Color.yellow;
            DebugTools.Draw.DrawWireArc(transform.position, transform.forward.normalized, 360f, message.BroadCastRadius);
        }
    }
}
