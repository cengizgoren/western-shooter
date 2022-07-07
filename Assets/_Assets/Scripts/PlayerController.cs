using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent (typeof (CharacterController))]
public class PlayerController : MonoBehaviour
{

    public event EventHandler<OnShootEventArgs> OnShoot;

    public class OnShootEventArgs : EventArgs
    {
        public Vector3 gunEndPointPosition;
        public Quaternion shootPosition;
    }
    [SerializeField]
    private float movementSpeed = 1f;

    public int selectedWeapon = 0;

    [SerializeField]
    private LayerMask groundMask;

    private Animator animator;

    private CharacterController controller;

    public Transform weaponHolder;

    public Gun currentGun;
    public Gun[] guns;

    private float allowFireAfterWeaponSwitchTime;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        selectWeapon();
        //OnSpacePressed += Testing_OnSpacePressed;
    }

    void Update()
    {
        HandleMovementInput();
        HandleRotationInput();
        HandleShootInput();
        HandleWeaponSwitch();
    }

    void HandleMovementInput()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        controller.SimpleMove(movementSpeed * move);

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
        if (IsWeaponFirePossible())
        {
            if (Input.GetButtonDown("Fire1"))
            {
                OnShoot?.Invoke(this, new OnShootEventArgs { gunEndPointPosition = weaponHolder.position, shootPosition = weaponHolder.rotation });
            } 
            else if (Input.GetButton("Fire1") && currentGun.gunType == Gun.GunType.Auto)
            {
                OnShoot?.Invoke(this, new OnShootEventArgs { gunEndPointPosition = weaponHolder.position, shootPosition = weaponHolder.rotation });
            }
        }
    }

    private bool IsWeaponFirePossible()
    {
        bool canShoot = true;
        if (Time.time < allowFireAfterWeaponSwitchTime)
        {
            canShoot = false;
        }
        return canShoot;
    }

    void HandleWeaponSwitch()
    {
        int previousSelectedWeapons = selectedWeapon;

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            selectedWeapon++;
            selectedWeapon = mod(selectedWeapon, transform.childCount);
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            selectedWeapon--;
            selectedWeapon = mod(selectedWeapon, transform.childCount);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
            selectedWeapon = 0;

        if (Input.GetKeyDown(KeyCode.Alpha2) && guns.Length >= 2)
            selectedWeapon = 1;

        if (Input.GetKeyDown(KeyCode.Alpha3) && guns.Length >= 3)
            selectedWeapon = 2;

        if (Input.GetKeyDown(KeyCode.Alpha4) && guns.Length >= 4)
            selectedWeapon = 3;

        if (selectedWeapon != previousSelectedWeapons)
            selectWeapon();
    }

    private void selectWeapon()
    {
        if (currentGun)
        {
            Destroy(currentGun.gameObject);
        }
        allowFireAfterWeaponSwitchTime = Time.time + 1.5f;
        currentGun = Instantiate(guns[selectedWeapon], weaponHolder.position, weaponHolder.rotation);
        currentGun.transform.parent = weaponHolder;
        currentGun.transform.Rotate(90, 0, 0);
        animator.SetTrigger("WeaponSwitch");
    }

    int mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying == false)
        {
            return;
        }

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hitInfo, float.MaxValue, groundMask))
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(ray.origin, 0.01f);
        }
    }
}
