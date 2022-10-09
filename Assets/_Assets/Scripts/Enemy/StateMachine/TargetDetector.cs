using UnityEngine;
using UnityEngine.AI;

public class TargetDetector : MonoBehaviour
{
    public bool TargetSighted = false;
    //public bool TargetHeard = false;
    public bool TargetObstructed = false;
    public bool Alerted = false;

    [Header("Senses Range")]
    [Tooltip("Cone of player detection in from of an enemy agent. The value is doubled.")]
    public float SightDetectionAngle = 90f;
    public float SightDetectionRadius = 10f;
    public float HearingDetectionRadius = 5f;
    public LayerMask CanBeSeen;

    // Make it a struct or class Maybe?
    [Header("Last known Position")]
    public CharacterController PlayerController;
    public bool isThereLastKnownPos = false;
    public Vector3 LastKnownPosition;
    public Vector3 LastKnownDirection;
    public Vector3 LastKnownVeloctity;

    [Header("Debug")]
    public bool ShowFieldOfView = true;
    public bool ShowHearingRadius = true;
    public bool ShowLineToPlayer = true;
    public bool ShowVisionRaycasts = true;

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
        if (time > 0.1f)
        {
            TargetSighted = IsPlayerInSightRange();
            //TargetHeard = IsPlayerHearingRange();
            TargetObstructed = IsPlayerObstructed();
            time = 0;
        }

        time += Time.deltaTime;
    }

    public Vector3 GetNextPositionToSearch()
    {
        // Make it less obvious by randomizing or by having a dedicated algo
        RandomPointOnCircle(transform.position, 10f, out Vector3 result);
        return result;
    }

    private bool RandomPointOnCircle(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 randomPointOnCircle = Random.insideUnitCircle.normalized;
            Vector3 randomPoint = center + new Vector3(randomPointOnCircle.x, 0f, randomPointOnCircle.y) * range;
            NavMeshHit hit;
            // Also check if line of sight is going to be maintained
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas) && !navMeshAgent.Raycast(hit.position, out _))
            {
                Debug.DrawRay(hit.position, Vector3.up, Color.blue, 3f);
                result = hit.position;
                return true;
            }
        }
        Debug.LogWarning("Could not find viable random point!");
        result = Vector3.zero;
        return false;
    }

    private bool IsPlayerInSightRange()
    {
        float distance = Vector3.Distance(transform.position, PlayerController.transform.position);
        if (distance < SightDetectionRadius)
        {
            Vector3 toPlayer = PlayerController.transform.position - transform.position;
            return Vector3.Angle(toPlayer, transform.forward) < SightDetectionAngle / 2f;
        }
        return false;
    }

    //private bool IsPlayerHearingRange()
    //{
    //    return HearingDetectionRadius > Vector3.Distance(transform.position, PlayerController.transform.position);
    //}

    private bool IsPlayerObstructed()
    {
        Vector3 position = new Vector3(transform.position.x, 1.5f, transform.position.z);
        Vector3 toPlayer = PlayerController.transform.position - transform.position;

        if (Physics.Raycast(position, toPlayer, out RaycastHit hit))
        {
            if (ShowVisionRaycasts)
            {
                Debug.DrawRay(position, hit.point - position, Color.yellow, 1.0f);
                Debug.LogWarningFormat("Enemy {0} direct line of sight check: {1}", transform.name, hit.collider.name);
            }
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                return false;
            }
        }
        return true;
    }

    public void SaveLastKnownPosAndDir()
    {
        LastKnownPosition = PlayerController.transform.position;
        LastKnownDirection = PlayerController.transform.forward.normalized;
        LastKnownVeloctity = PlayerController.velocity;
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

        if (ShowFieldOfView)
        {
            Gizmos.color = Color.red;
            DebugTools.Draw.DrawWireArc(transform.position, transform.forward.normalized, SightDetectionAngle, SightDetectionRadius);
        }

        if (ShowHearingRadius)
        {
            Gizmos.color = Color.blue;
            DebugTools.Draw.DrawWireArc(transform.position, transform.forward.normalized, 360f, HearingDetectionRadius);
        }

        if (ShowLineToPlayer)
        {
            Gizmos.color = Color.yellow;
            Vector3 toPlayer = PlayerController.transform.position - transform.position;
            Gizmos.DrawLine(transform.position, transform.position + toPlayer);
        }
    }
}
