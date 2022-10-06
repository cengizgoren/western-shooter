using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Messager : MonoBehaviour
{
    public UnityAction OnAlert;

    public enum Messages
    {
        ALERT
    }

    public LayerMask MessagableLayers = -1;
    public float MessageRadius;
    [Header("Gizmos")]
    public bool ShowMessageRadius;

    public void SendMessage(Messages message)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, MessageRadius, MessagableLayers);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.TryGetComponent(out Messager messagrereceiver))
            {
                messagrereceiver.ProcessMessage(message);
            } else
            {
                Debug.LogWarningFormat("Message ({0}) broadcasted by object {1} received by object without Messager component: {2}", Enum.GetName(typeof(Messages), message), transform.name, hitCollider.name);
            }
        }
    }

    public void ProcessMessage(Messages message)
    {
        switch (message)
        {
            case Messages.ALERT:
                OnAlert?.Invoke();
                break;
        }
    }

    private void OnDrawGizmos()
    {
        if (ShowMessageRadius)
        {
            Gizmos.color = Color.yellow;
            DebugTools.Draw.DrawWireArc(transform.position, transform.forward.normalized, 360f, MessageRadius);
        }
    }
}
