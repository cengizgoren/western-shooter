using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPlayer : MonoBehaviour
{
    [SerializeField] private float speed = 2f;

    private CharacterController controller;

    private Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        var velocity = Vector3.forward * Input.GetAxis("Vertical") * speed;
        controller.Move(velocity * Time.deltaTime);
        animator.SetFloat("Speed", velocity.magnitude);
    }
}
