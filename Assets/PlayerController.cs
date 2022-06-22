using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (CharacterController))]
public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float movementSpeed;

    [SerializeField]
    private LayerMask groundMask;


    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleMovementInput();
        HandleRotationInput();
        HandleShootInput();
    }

    void HandleMovementInput()
    {
        float _horizontal = Input.GetAxis("Horizontal");
        float _vertical = Input.GetAxis("Vertical");
        Vector3 _movemement = new Vector3(_horizontal, 0, _vertical);
        controller.Move(movementSpeed * Time.deltaTime * _movemement);
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
