using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed = 1f;

    [SerializeField]
    private LayerMask groundMask;

    private Animator animator;

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleMovementInput();
        HandleRotationInput();
        HandleShootInput();
    }

    void HandleMovementInput()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        controller.Move(move * Time.deltaTime * movementSpeed);

        // Velocity calculation
        Vector3 horizontalVelocity = controller.velocity;
        Vector3 relVel = transform.InverseTransformDirection(horizontalVelocity);

        horizontalVelocity = new Vector3(relVel.x, 0, relVel.z);
        float horizontalSpeed = horizontalVelocity.magnitude;

        animator.SetFloat("SpeedZ", relVel.z);
        animator.SetFloat("SpeedX", relVel.x);
        animator.SetFloat("SpeedHorizontal", horizontalSpeed);
    }

    void HandleRotationInput()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask))
        {
            transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
        }
    }

    void HandleShootInput()
    {
        if (Input.GetButton("Fire1"))
        {
            //PlayerGun.Instance.Shoot();
            RayGun.Instance.Shoot();
        }
        animator.SetBool("IsShooting", Input.GetButton("Fire1"));
    }
        
    private void OnDrawGizmos()
    {
        if (Application.isPlaying == false)
        {
            return;
        }

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hitInfo, float.MaxValue))
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(ray.origin, 0.01f);
        }
    }
}
