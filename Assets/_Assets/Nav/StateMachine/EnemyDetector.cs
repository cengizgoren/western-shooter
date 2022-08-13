using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    public bool EnemyInRange = false;
    
    [SerializeField] private CharacterController _playerController;

    // Make it a struct or class
    bool isThereLastKnownPos = false;
    public Vector3 LastKnownPosition;
    public Vector3 LastKnownDirection;
    public Vector3 LastKnownVeloctity;

    [Tooltip("Cone of player detection in from of an enemy agent. The value is doubled.")]
    [SerializeField] private float _visualDetectionConeAngle = 30f;
    [SerializeField] private float _visualDetectionConeRange = 10f;

    private float timeSinceDetection;

    private void Update()
    {
        if (!EnemyInRange)
        {
            EnemyInRange = IsPlayerInSight();
        }
        else
        {
            timeSinceDetection += Time.deltaTime;
            if (timeSinceDetection > 3f)
            {
                EnemyInRange = IsPlayerInSight();
                timeSinceDetection = 0f;
            }
        }
    }

    private bool IsPlayerInSight()
    {
        if (Vector3.Distance(transform.position, _playerController.transform.position) < _visualDetectionConeRange)
        {
            Vector3 toPlayer = _playerController.transform.position - transform.position;
            if (Vector3.Angle(toPlayer, transform.forward) < _visualDetectionConeAngle)
                return true;
        }
        return false;
    }

    public void SaveLastKnownPosAndDir()
    {
        LastKnownPosition = _playerController.transform.position;
        LastKnownDirection = _playerController.transform.forward.normalized;
        LastKnownVeloctity = _playerController.velocity;
        isThereLastKnownPos = true;
    }

    private void OnDrawGizmos()
    {
        if (isThereLastKnownPos)
        {
            Gizmos.color = Color.cyan;
            DebugTools.DrawArrow.ForGizmo(LastKnownPosition, LastKnownDirection);

            Gizmos.color = Color.magenta;
            DebugTools.DrawArrow.ForGizmo(LastKnownPosition, LastKnownVeloctity);
        }
    }
}
