using UnityEngine;

public class PlayerAim : Aim
{
    [SerializeField] private Transform CameraRoot;
    [SerializeField] private Transform DirectionArrow;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float MouseWeight = 0.6f;

    [Range(0.0f, 100.0f)]
    [SerializeField] private float MaxCameraOffset = 25f;

    private void Awake()
    {
        input = GetComponent<Input>();
        AimPoint.localPosition = Vector3.zero;
    }

    protected override void Update()
    {
        if (input.IsAimingAllowed)
        {
            Camera();
            Rotate();
            Arrow();
        }
    }

    private void Camera()
    {
        CameraRoot.position = transform.position + Vector3.ClampMagnitude(MouseWeight * input.DirectionToMouse, MaxCameraOffset);
    }

    private void Arrow()
    {
        DirectionArrow.SetPositionAndRotation(new Vector3(input.MouseWorldPosition.x, DirectionArrow.position.y, input.MouseWorldPosition.z), targetRotation);
    }
}
