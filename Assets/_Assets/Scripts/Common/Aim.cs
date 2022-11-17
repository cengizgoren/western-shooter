using UnityEngine;

public class Aim : MonoBehaviour
{
    [SerializeField] protected Transform AimPoint;
    [SerializeField] protected float AngularSpeed = 360f;
    [SerializeField][Range(1.0f, 5.0f)] protected float OveraimFactor = 1f;

    protected Quaternion targetRotation;
    protected Input input;

    private void Awake()
    {
        input = GetComponent<Input>();
    }

    protected virtual void Update()
    {
        if (input.IsAimingAllowed)
        {
            Rotate();
        }
    }

    protected void Rotate()
    {
        // Rotation
        targetRotation = Quaternion.LookRotation(new Vector3(input.DirectionToMouse.x, 0f, input.DirectionToMouse.z), Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * AngularSpeed);

        // AimPoint
        AimPoint.position = transform.position + input.DirectionToMouse.magnitude * OveraimFactor * transform.forward;
    }
}
