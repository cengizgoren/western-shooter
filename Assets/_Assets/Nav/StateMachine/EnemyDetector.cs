using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDetector : MonoBehaviour
{
    public bool EnemyDetected = false;

    [SerializeField] private CharacterController _playerController;

    // Make it a struct or class
    bool isThereLastKnownPos = false;
    public Vector3 LastKnownPosition;
    public Vector3 LastKnownDirection;
    public Vector3 LastKnownVeloctity;

    [Space(10)]
    [SerializeField] private bool _showFieldOfView = true;
    [SerializeField] private bool _showHearingRadius = true;
    [SerializeField] private bool _showLineToPlayer = true;
    [Tooltip("Cone of player detection in from of an enemy agent. The value is doubled.")]
    [SerializeField] private float _sightDetectionAngle = 90f;
    [SerializeField] private float _sightDetectionRadius = 10f;
    [SerializeField] private float _hearingDetectionRadius = 5f;


    private float timeSinceDetection;

    public void Awake()
    {
        GetComponent<Health>().OnHealthLost += () => EnemyDetected = true;
    }

    private void Update()
    {
        if (!EnemyDetected)
        {
            EnemyDetected = IsPlayerDetected();
        }
        else
        {
            timeSinceDetection += Time.deltaTime;
            if (timeSinceDetection > 3f)
            {
                EnemyDetected = IsPlayerDetected();
                timeSinceDetection = 0f;
            }
        }
    }

    public Vector3 GetNextPositionToSearch()
    {
        // Make it less obvious by randomizing or by having a dedicated algo
        return _playerController.transform.position;
    }

    private bool IsPlayerDetected()
    {
        float distance = Vector3.Distance(transform.position, _playerController.transform.position);
        if (distance < _sightDetectionRadius)
        {
            Vector3 toPlayer = _playerController.transform.position - transform.position;
            if (Vector3.Angle(toPlayer, transform.forward) < _sightDetectionAngle / 2f)
                return true;

            if (distance < _hearingDetectionRadius)
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

        if (_showFieldOfView)
        {
            Gizmos.color = Color.red;
            DebugTools.DrawArc.DrawWireArc(transform.position, transform.forward.normalized, _sightDetectionAngle, _sightDetectionRadius);
        }

        if (_showHearingRadius)
        {
            Gizmos.color = Color.blue;
            DebugTools.DrawArc.DrawWireArc(transform.position, transform.forward.normalized, 360f, _hearingDetectionRadius);
        }

        if (_showLineToPlayer)
        {
            Gizmos.color = Color.yellow;
            Vector3 toPlayer = _playerController.transform.position - transform.position;
            Gizmos.DrawLine(transform.position, transform.position + toPlayer);
        }
    }
}
