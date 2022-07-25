using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIBrain : MonoBehaviour
{
    private enum AIState 
    {
        Idle, Engaged
    }

    [SerializeField] private AIState State = AIState.Idle;
    [SerializeField] private Transform PlayerTransform;
    [SerializeField] private float VisualDetectionConeRange = 10f;
    
    [Tooltip("Cone of player detection in from of an enemy agent. The value is doubled.")]
    [SerializeField] private float VisualDetectionConeAngle = 30f;

    private float coneAngle;

    private NavMeshAgent agent;
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start() {}

    private void Update()
    {
        StartCoroutine(DoCheck());
        if (State == AIState.Engaged)
        {
            agent.destination = PlayerTransform.position;
        }
    }

    private bool ProximityCheck() 
    {
        if (Vector3.Distance(transform.position, PlayerTransform.position) < VisualDetectionConeRange) 
        {
            Vector3 toPlayer = PlayerTransform.position - transform.position;

            float angle = Vector3.Angle(toPlayer, transform.forward);
            if (angle < VisualDetectionConeAngle)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator DoCheck() 
    {
        for(;;)
        {
            if (ProximityCheck())
            {
                State = AIState.Engaged;
            } 
            yield return new WaitForSeconds(.5f);
        }
    }

    // Gizmos
    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, VisualDetectionConeRange);
    }
}
