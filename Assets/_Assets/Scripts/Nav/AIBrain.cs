using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIBrain : MonoBehaviour
{
    private enum AIState
    {
        Idle, Chase, BeginAttack, Reposition
    }

    [SerializeField] private AIState State = AIState.Idle;
    [SerializeField] private Transform PlayerTransform;
    [SerializeField] private float VisualDetectionConeRange = 10f;
    [SerializeField] private float AttackRange = 100f;

    [Tooltip("Cone of player detection in from of an enemy agent. The value is doubled.")]
    [SerializeField] private float VisualDetectionConeAngle = 30f;

    private float coneAngle;

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start() { }

    private void Update()
    {
        if (State == AIState.Idle)
        {
            if (IsPlayerInSight())
            {
                State = AIState.Chase;
            }
        }

        if (State == AIState.Chase)
        {
            agent.SetDestination(PlayerTransform.position);
            if (IsPlayerInAttackRange() && IsPlayerUnobstructed())
            {
                State = AIState.BeginAttack;
            }
        }

        if (State == AIState.BeginAttack)
        {
            AssignRepositionSpot();
            State = AIState.Reposition;
        }

        if (State == AIState.Reposition)
        {
            LookAt(PlayerTransform.position);

            if (agent.remainingDistance <= agent.stoppingDistance)
                State = AIState.BeginAttack;

            if (IsPlayerInAttackRange() && IsPlayerUnobstructed())
            {}
            else
                State = AIState.Chase;
        }

    }

    private bool IsPlayerInSight()
    {
        if (Vector3.Distance(transform.position, PlayerTransform.position) < VisualDetectionConeRange)
        {
            Vector3 toPlayer = PlayerTransform.position - transform.position;
            if (Vector3.Angle(toPlayer, transform.forward) < VisualDetectionConeAngle)
                return true;
        }
        return false;
    }

    private bool IsPlayerInAttackRange()
    {
        return Vector3.Distance(transform.position, PlayerTransform.position) < AttackRange;
    }

    private bool IsPlayerUnobstructed()
    {
        NavMeshHit hit;
        return !agent.Raycast(PlayerTransform.position, out hit);
    }

    private void AssignRepositionSpot()
    {
        Vector3 point;
        RandomPointOnCircle(PlayerTransform.position, 10f, out point);
        agent.SetDestination(point);
    }

    private void LookAt(Vector3 lookPoint)
    {
        Vector3 direction = (lookPoint - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0.0f, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 100f);
    }

    private bool RandomPointOnCircle(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 1; i++)
        {
            Vector2 randomPointOnCircle = Random.insideUnitCircle.normalized;
            Vector3 randomPoint = PlayerTransform.position + new Vector3(randomPointOnCircle.x, 0f, randomPointOnCircle.y) * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    // Gizmos
    // private void OnDrawGizmos() 
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(transform.position, VisualDetectionConeRange);
    //     Gizmos.color = Color.green;
    //     Gizmos.DrawWireSphere(transform.position, AttackRange);
    // }
}
