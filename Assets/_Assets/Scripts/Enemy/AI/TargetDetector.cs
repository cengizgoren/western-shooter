using UnityEngine;

public partial class TargetDetector : MonoBehaviour
{
    public bool TargetSighted = false;
    public bool TargetObstructed = false;
    public bool Alerted = false;
    public bool TooFarAway = false;
    public bool TooClose = false;

    [Space(10)]
    [Tooltip("Cone of player detection in from of an enemy agent. The value is doubled.")]
    public float SightDetectionAngle = 90f;
    public float SightDetectionRadius = 10f;
    public float HearingDetectionRadius = 5f;
    public float FriendlySphereCastThickness = 0.5f;
    public float KeepMaxDistanceFromTarget = 40f;
    public float KeepMinDistanceFromTarget = 20f;
    public LayerMask ObstructsVision;
    public LayerMask PreventsWeaponFire;

    [Header("Debug")]
    public DebugItem FieldOfView;
    public DebugItem HearingRadius;
    public DebugItem LineToPlayer;
    public DebugItem VisionRaycasts;
    public DebugItem WeaponCone;
    public DebugItem FriendlyRaycasts;

    private Transform playerTransform;
    private Messager messager;
    private Health enemyHealth;

    public void Awake()
    {
        playerTransform = GetComponent<EnemyStateMachine>().PlayerController.transform;
        enemyHealth = GetComponent<Health>();
        messager = GetComponent<Messager>();

        enemyHealth.OnHpLost += Alert;
        messager.OnAlert += Alert;
    }

    private void OnDestroy()
    {
        enemyHealth.OnHpLost -= Alert;
        messager.OnAlert -= Alert;
    }

    private void Alert() => Alerted = true;
 
    public void Tick()
    {
        TargetSighted = IsPlayerInSightRange();
        TargetObstructed = IsPlayerObstructed();
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        TooFarAway = distance > KeepMaxDistanceFromTarget;
        TooClose = distance < KeepMinDistanceFromTarget;
    }

    private bool IsPlayerInSightRange()
    {
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        if (distance < SightDetectionRadius)
        {
            Vector3 toPlayer = playerTransform.position - transform.position;
            return Vector3.Angle(toPlayer, transform.forward) < SightDetectionAngle / 2f;
        }
        return false;
    }

    private bool IsPlayerObstructed()
    {
        Vector3 position = new Vector3(transform.position.x, 1.5f, transform.position.z);
        Vector3 toPlayer = playerTransform.position - transform.position;

        if (Physics.Raycast(position, toPlayer, out RaycastHit hit, Mathf.Infinity, ObstructsVision))
        {
            if (VisionRaycasts.Show)
            {
                Debug.DrawRay(position, hit.point - position, VisionRaycasts.Color, 1.0f);
                Debug.LogWarningFormat("Enemy {0} direct line of sight check: {1}", transform.name, hit.collider.name);
            }
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                return false;
            }
        }
        return true;
    }

    public bool IsPlayerInSights()
    {
        float WeaponAimedAtPlayerAngle = 2f;
        Vector3 toPlayer = playerTransform.position - transform.position;

        return Vector3.Angle(toPlayer, transform.forward) < WeaponAimedAtPlayerAngle / 2f;
    }

    public bool IsFriendlyInSights()
    {
        Vector3 position = new Vector3(transform.position.x, 1.5f, transform.position.z);

        if (Physics.SphereCast(position, FriendlySphereCastThickness, transform.forward, out RaycastHit hit, Vector3.Distance(transform.position, playerTransform.position), PreventsWeaponFire))
        {
            if (FriendlyRaycasts.Show)
            {
                Debug.DrawRay(position, transform.forward * hit.distance, FriendlyRaycasts.Color, 1.0f);
            }
            return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        //if (isThereLastKnownPos)
        //{
        //    Gizmos.color = Color.cyan;
        //    DebugTools.Draw.GizmoArrow(LastKnownPosition, LastKnownDirection);

        //    Gizmos.color = Color.magenta;
        //    DebugTools.Draw.GizmoArrow(LastKnownPosition, LastKnownVeloctity);
        //}

        if (FieldOfView.Show)
        {
            Gizmos.color = FieldOfView.Color;
            DebugTools.Draw.DrawWireArc(transform.position, transform.forward.normalized, SightDetectionAngle, SightDetectionRadius, FieldOfView.Color);
        }

        if (WeaponCone.Show)
        {
            Gizmos.color = WeaponCone.Color;
            DebugTools.Draw.DrawWireArc(transform.position, transform.forward.normalized, 2f, 100f, WeaponCone.Color);
        }

        if (HearingRadius.Show)
        {
            Gizmos.color = HearingRadius.Color;
            DebugTools.Draw.DrawWireArc(transform.position, transform.forward.normalized, 360f, HearingDetectionRadius, HearingRadius.Color);
        }

        if (LineToPlayer.Show)
        {
            Gizmos.color = LineToPlayer.Color;
            Vector3 toPlayer = playerTransform.position - transform.position;
            Gizmos.DrawLine(transform.position, transform.position + toPlayer);
        }
    }
}
