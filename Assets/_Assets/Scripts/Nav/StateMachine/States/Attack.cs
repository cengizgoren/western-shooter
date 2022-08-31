using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Attack : IState
{
    private Transform _playerTransform;
    private Transform _transform;
    private NavMeshAgent _navMeshAgent;
    private EnemyDetector _enemyDetector;
    private EnemyController _enemyController;

    private float time;
    private int directionFactor = 1;

    public Attack(EnemyDetector enemyDetector, Transform transform, EnemyController enemyController, Transform playerTransform, NavMeshAgent navMeshAgent)
    {
        _transform = transform;
        _playerTransform = playerTransform;
        _navMeshAgent = navMeshAgent;
        _enemyDetector = enemyDetector;
        _enemyController = enemyController;
    }

    public void Tick()
    {
        Quaternion targetRotation = Quaternion.LookRotation(_playerTransform.position - _transform.position);
        _transform.rotation = Quaternion.RotateTowards(_transform.rotation, targetRotation, Time.deltaTime * 90f);

        Vector3 toPlayerDir = _playerTransform.position - _transform.position;
        Vector3 toSide = Vector3.Cross(toPlayerDir, Vector3.up).normalized;
        DebugTools.Draw.DebugArrow(_transform.position, toSide * 3);

        bool success;
        if (time > 2f)
        {
            Vector3 strafeToPoint;
            success = RandomPointOnCircle(_transform.position + toSide * 5 * directionFactor, 2f, out strafeToPoint);

            if (!success)
            {
                directionFactor *= -1;
                RandomPointOnCircle(_transform.position + toSide * 5 * directionFactor, 2f, out strafeToPoint);
            }
            _navMeshAgent.SetDestination(strafeToPoint);
            time = 0f;
        }

        time += Time.deltaTime;
    }

    public void OnEnter()
    {
        Debug.Log("Attack");
        time = 0;
        _enemyController.SetAttack(true);
        _navMeshAgent.ResetPath();
        _navMeshAgent.updateRotation = false;
        _navMeshAgent.speed = 2f;
    }

    public void OnExit()
    {
        _enemyDetector.SaveLastKnownPosAndDir();
        _enemyController.SetAttack(false);
        _navMeshAgent.updateRotation = true;
        _navMeshAgent.speed = 4f;
    }

    private void LookAt(Vector3 lookPoint)
    {
        Vector3 direction = (lookPoint - _transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0.0f, direction.z));
        _transform.rotation = Quaternion.Slerp(_transform.rotation, lookRotation, Time.deltaTime * 10f);
    }

    private bool RandomPointOnCircle(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 randomPointOnCircle = Random.insideUnitCircle.normalized;
            Vector3 randomPoint = center + new Vector3(randomPointOnCircle.x, 0f, randomPointOnCircle.y) * range;
            NavMeshHit hit;
            // Also check if line of sight is going to be maintained
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas) && !_navMeshAgent.Raycast(hit.position, out _))
            {
                Debug.DrawRay(hit.position, Vector3.up, Color.blue, 3f);
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

}
