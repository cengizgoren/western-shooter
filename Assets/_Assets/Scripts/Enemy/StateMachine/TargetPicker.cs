using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TargetPicker : MonoBehaviour
{
    [Range(0, 100)]
    public int RandomPointsCount = 10;
    public float RandomCircleSize = 15f;

    private NavMeshAgent navMeshAgent;
    private Transform playerTransform;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerTransform = GetComponent<EnemyStateMachine>().PlayerController.transform;
    }

    public bool GetClosestRandomPositionAroundPlayer(out Vector3 closesPoint)
    {
        closesPoint = Vector3.zero;
        float minDistance = Mathf.Infinity;
        bool sucessFlag = false;

        for (int i = 0; i < RandomPointsCount; i++)
        {
            if (RandomPointOnCircle(playerTransform.position, RandomCircleSize, 2f, out Vector3 randomPoint))
            {
                float distance = Vector3.Distance(transform.position, randomPoint);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closesPoint = randomPoint;
                }
                sucessFlag = true;
            }
        }

        return sucessFlag;
    }

    private bool RandomPointOnCircle(Vector3 center, float distance, float range, out Vector3 result)
    {
        for (int i = 0; i < 20; i++)
        {
            Vector2 randomPointOnCircle = Random.insideUnitCircle.normalized;
            Vector3 randomPointOnSphereBelt = new Vector3(randomPointOnCircle.x, 0f, randomPointOnCircle.y);
            Vector3 randomPoint = center + randomPointOnSphereBelt * (distance + Random.Range(-range, +range));
            Debug.DrawRay(randomPoint, Vector3.up, Color.blue, 0.5f);

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.5f, NavMesh.AllAreas) && !navMeshAgent.Raycast(hit.position, out _))
            {
                result = hit.position;
                return true;
            }
        }
        Debug.LogWarning("Could not find viable random point!");
        result = Vector3.zero;
        return false;
    }
}
