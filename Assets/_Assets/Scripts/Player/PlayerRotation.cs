using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    public float AngularSpeed = 720f;
    public Transform CameraRoot;
    public Transform AimPoint;
    public Transform DirectionArrow;

    [Space(10)]
    [Range(1.0f, 5.0f)] public float OveraimFactor = 1.25f;
    [Range(0.0f, 1.0f)] public float MouseWeight = 0.6f;
    [Range(0.0f, 100.0f)] public float MaxCameraOffset = 25f;

    private Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, 0f, 0f));
    private Vector3 forwardAim = new Vector3(0f, 0f, 0f);
    private Vector2 pos;
    private Vector3 MousePoint;

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
        pos = Controls.InputActions.Player.Look.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(pos);
        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            MousePoint = ray.GetPoint(rayDistance);
            Vector3 playersDirectionToMouse = MousePoint - transform.position;

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
                DirectionArrow.position = new Vector3(MousePoint.x, DirectionArrow.position.y, MousePoint.z);
                DirectionArrow.rotation = Quaternion.LookRotation(playersDirectionToMouse);
            }
        }
    }
}
