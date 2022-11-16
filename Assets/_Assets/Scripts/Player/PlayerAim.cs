using UnityEngine;
using UnityEngine.Windows;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] private float AngularSpeed = 720f;
    [SerializeField] private Transform AimPoint;
    [SerializeField][Range(1.0f, 5.0f)] private float OveraimFactor = 1.25f;

    private Vector3 forwardAim = new Vector3(0f, 0f, 0f);

    private PlayerInput input;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        AimPoint.localPosition = Vector3.zero;
    }

    private void Update()
    {
        Rotation();
    }

    private void Rotation()
    {
        // Rotation
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(input.DirectionToMouse.x, 0f, input.DirectionToMouse.z), Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * AngularSpeed);

        // Aim Point
        forwardAim.z = input.DirectionToMouse.magnitude * OveraimFactor;
        AimPoint.localPosition = forwardAim;
    }
}
