using UnityEngine;
using UnityEngine.AI;

public class TargetPicker : MonoBehaviour
{
    [Range(0, 100)]
    public int RandomPointsCount = 10;
    public float RandomCircleSize = 15f;
    public float RandomRangeFromCircle = 2f;
    public LayerMask ObstructsDirectPathClearCheck;

    private const int MAX_SAMPLE_FAILS = 5;

    private Transform playerTransform;

    private void Awake()
    {
        playerTransform = GetComponent<EnemyStateMachine>().PlayerController.transform;
    }

    public bool TryFindPositionAwayFromPlayer(out Vector3 pos)
    {
        return TryFindPosition(-(playerTransform.position - transform.position), out pos);
    }

    public bool TryFindPositionCloseToPlayer(out Vector3 closesPoint)
    {
        return TryFindPosition(playerTransform.position, out closesPoint);
    }

    public bool TryFindPosition(Vector3 position, out Vector3 closesPoint)
    {
        bool atLeastOnePointFound = false;
        bool unobstructedPointFound = false;
        float minDistance = Mathf.Infinity;
        closesPoint = Vector3.zero;

        for (int i = 0; i < RandomPointsCount; i++)
        {
            if (RandomPointOnCircle(position, RandomCircleSize, RandomRangeFromCircle, out Vector3 randomPoint))
            {
                Vector3 dirToPoint = randomPoint - position;
                if (!Physics.Raycast(position, dirToPoint, dirToPoint.magnitude, ObstructsDirectPathClearCheck))
                {
                    Debug.DrawRay(randomPoint, Vector3.up, Color.green, 1f);

                    float distance = Vector3.Distance(transform.position, randomPoint);

                    if (unobstructedPointFound)
                    {
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closesPoint = randomPoint;
                        }
                    }
                    else
                    {
                        closesPoint = randomPoint;
                        minDistance = distance;
                        unobstructedPointFound = true;
                    }
                }
                else
                {
                    if (!unobstructedPointFound)
                    {
                        Debug.DrawRay(randomPoint, Vector3.up, Color.red, 1f);

                        float distance = Vector3.Distance(transform.position, randomPoint);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closesPoint = randomPoint;
                        }
                    }
                }
            }
            atLeastOnePointFound = true;
        }
        return atLeastOnePointFound;
    }

    private bool RandomPointOnCircle(Vector3 center, float distance, float range, out Vector3 result)
    {
        for (int i = 0; i < MAX_SAMPLE_FAILS; i++)
        {
            Vector2 randomPointOnCircle = Random.insideUnitCircle.normalized;
            Vector3 randomPointOnSphereBelt = new Vector3(randomPointOnCircle.x, 0f, randomPointOnCircle.y);
            Vector3 randomPoint = center + randomPointOnSphereBelt * (distance + Random.Range(-range, +range));
            //Debug.DrawRay(randomPoint, Vector3.up, Color.blue, 0.5f);

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.5f, NavMesh.AllAreas))
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
