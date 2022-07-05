using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPlayer : MonoBehaviour
{
    [SerializeField] private float speed = 1f;

    private CharacterController controller;

    private Animator animator;

    [SerializeField]
    private LayerMask groundMask;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        controller.Move(move * Time.deltaTime * speed);

        // Velocity calculation
        Vector3 horizontalVelocity = controller.velocity;
        Vector3 relVel = transform.InverseTransformDirection(horizontalVelocity);

        horizontalVelocity = new Vector3(relVel.x, 0, relVel.z);
        float horizontalSpeed = horizontalVelocity.magnitude;

        animator.SetFloat("SpeedZ", relVel.z);
        animator.SetFloat("SpeedX", relVel.x);
        animator.SetFloat("SpeedHorizontal", horizontalSpeed);

        ///

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask))
        {
            transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
        }
    }
}
