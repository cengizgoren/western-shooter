using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class TargetDetector : MonoBehaviour
{
    public bool TargetSighted = false;
    public bool TargetHeard = false;
    public bool TargetObstructed = false;
    public bool Alerted = false;

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

    private NavMeshAgent navMeshAgent;
    private Messager messager;

    private float time = 0;

    public void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        messager = GetComponent<Messager>();

        messager.OnAlert += () => Alerted = true;
        GetComponent<EnemyHealth>().OnHpLost += () => Alerted = true;
    }

    private void Update() 
    {
        if (time > 1.5f)
        {
            TargetSighted = IsPlayerInSightRange();
            TargetHeard = IsPlayerHearingRange();
            TargetObstructed = IsPlayerObstructed();
            time = 0;
        }

        time += Time.deltaTime;
    }

    public Vector3 GetNextPositionToSearch()
    {
        // Make it less obvious by randomizing or by having a dedicated algo
        return _playerController.transform.position;
    }

    private bool IsPlayerInSightRange()
    {
        float distance = Vector3.Distance(transform.position, _playerController.transform.position);
        if (distance < _sightDetectionRadius)
        {
            Vector3 toPlayer = _playerController.transform.position - transform.position;
            return Vector3.Angle(toPlayer, transform.forward) < _sightDetectionAngle / 2f;
        }
        return false;
    }

    private bool IsPlayerHearingRange()
    {
        return _hearingDetectionRadius > Vector3.Distance(transform.position, _playerController.transform.position);
    }

    private bool IsPlayerObstructed()
    {
        return navMeshAgent.Raycast(_playerController.transform.position, out NavMeshHit _);
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
            DebugTools.Draw.GizmoArrow(LastKnownPosition, LastKnownDirection);

            Gizmos.color = Color.magenta;
            DebugTools.Draw.GizmoArrow(LastKnownPosition, LastKnownVeloctity);
        }

        if (_showFieldOfView)
        {
            Gizmos.color = Color.red;
            DebugTools.Draw.DrawWireArc(transform.position, transform.forward.normalized, _sightDetectionAngle, _sightDetectionRadius);
        }

        if (_showHearingRadius)
        {
            Gizmos.color = Color.blue;
            DebugTools.Draw.DrawWireArc(transform.position, transform.forward.normalized, 360f, _hearingDetectionRadius);
        }

        if (_showLineToPlayer)
        {
            Gizmos.color = Color.yellow;
            Vector3 toPlayer = _playerController.transform.position - transform.position;
            Gizmos.DrawLine(transform.position, transform.position + toPlayer);
        }
    }
}
