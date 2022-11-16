using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [SerializeField] private float AngularSpeed = 720f;
    [SerializeField] private Transform CameraRoot;
    [SerializeField] private Transform AimPoint;
    [SerializeField] private Transform DirectionArrow;

    [Range(1.0f, 5.0f)]
    [SerializeField] private float OveraimFactor = 1.25f;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float MouseWeight = 0.6f;

    [Range(0.0f, 100.0f)]
    [SerializeField] private float MaxCameraOffset = 25f;

    private Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, 0f, 0f));
    private Vector3 forwardAim = new Vector3(0f, 0f, 0f);
    private Vector2 mouseScreenPosition;
    private Vector3 mouseWorldPosition;

    private void Update()
    {
        Rotation();
    }

    private void Awake()
    {
        AimPoint.localPosition = Vector3.zero;
    }

    private void Rotation()
    {
        mouseScreenPosition = Controls.InputActions.Player.Look.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);
        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            mouseWorldPosition = ray.GetPoint(rayDistance);
            Vector3 playersDirectionToMouse = mouseWorldPosition - transform.position;

            if (Controls.InputActions.Player.enabled)
            {
                // Camera
                CameraRoot.position = transform.position + Vector3.ClampMagnitude(MouseWeight * playersDirectionToMouse, MaxCameraOffset);

                // Rotation
                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(playersDirectionToMouse.x, 0f, playersDirectionToMouse.z), Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * AngularSpeed);

                // Aim Point
                forwardAim.z = playersDirectionToMouse.magnitude * OveraimFactor;
                AimPoint.localPosition = forwardAim;

                // Direction Arrow
                DirectionArrow.SetPositionAndRotation(new Vector3(mouseWorldPosition.x, DirectionArrow.position.y, mouseWorldPosition.z), Quaternion.LookRotation(playersDirectionToMouse));
            }
        }
    }
}
