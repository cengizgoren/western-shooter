using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExitZone : MonoBehaviour
{
    public UnityAction OnPlayerInExitZone;

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogFormat("{0} has entered exit zone {1}", other.name, gameObject.name);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Invoking OnPlayerInExitZone");
            OnPlayerInExitZone?.Invoke();
        }
    }
}
