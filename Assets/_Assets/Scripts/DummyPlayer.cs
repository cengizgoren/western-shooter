using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DummyPlayer : MonoBehaviour
{
    [Header("Player")]
    public float MoveSpeed = 2.0f;
    public float SprintSpeed = 5.335f;
    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;
    public float RotationSpeed = 720f;
    [Tooltip("Offset player movement direction by this angle to make it relative to the camera position")]
    public float CameraYRotationDeg = 45.0f;
    public float TurnSpeed = 10.0f;
    public float JumpHeight = 1.2f;
    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -15.0f;
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.50f;
    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;
    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;
    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;
    public LayerMask GroundLayers;

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public Transform CameraRoot;
    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;

    [Space(10)]
    public Transform AimArrow;
    public LineRenderer LaserSight;


    [Header("Weapons Manager")]
    [SerializeField] private List<WeaponController> StartingWeapons = new List<WeaponController>();

    [Header("Weapon Switching")]
    [SerializeField] private Transform WeaponParentSocket;
    [SerializeField] private Transform DefaultWeaponPosition;
    [SerializeField] private Transform DownWeaponPosition;
    [SerializeField] private float WeaponSwitchDelay = 1f;
    [SerializeField] private AnimationCurve weaponSwitchCurve;

    [Header("Recoil")]
    [SerializeField] private float MaxRecoilDistance = 0.5f;
    [SerializeField] private float RecoilSharpness = 50f;
    [SerializeField] private float RecoilRestitutionSharpness = 10f;

    // Debug switches

    [Header("Debug")]
    [SerializeField] private bool muzzleToAimRay;

    [SerializeField] private bool aimPoint;
    [SerializeField][Range(0.0f, 10.0f)] private float aimPointRadius;

    [SerializeField] private bool cameraRootPoint;
    [SerializeField][Range(0.0f, 10.0f)] private float cameraRootPointRadius;


    private float _speed;
    private float _terminalVelocity = 53.0f;
    private float _verticalVelocity;
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private Animator _animator;
    private CharacterController _controller;
    private Health _health;

    private Transform weaponMuzzle;
    private readonly WeaponController[] weaponSlots = new WeaponController[9];
    private float timeStartedWeaponSwitch;
    private int meaponSwitchNewWeaponIndex;
    private WeaponController activeWeapon;
    private Vector3 weaponMainLocalPosition;
    private Quaternion weaponMainLocalRotation;
    private Vector3 accumulatedRecoil;
    private Vector3 weaponRecoilLocalPosition;

    public UnityAction<WeaponController> OnSwitchedToWeapon;
    public UnityAction<WeaponController, int> OnAddedWeapon;
    public UnityAction<WeaponController, int> OnRemovedWeapon;

    [Range(-10.0f, 10.0f)]
    public float MouseAndAimPointDistance = 0f;
    public Vector3 MousePoint;
    public Vector3 AimPoint;

    public int ActiveWeaponIndex { get; private set; }
    public WeaponSwitchState SwitchState { get; private set; }

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();

        ActiveWeaponIndex = -1;
        SwitchState = WeaponSwitchState.Down;
        weaponMainLocalPosition = DefaultWeaponPosition.localPosition;
        weaponMainLocalRotation = DefaultWeaponPosition.localRotation;

        OnSwitchedToWeapon += OnWeaponSwitched;
        _health.OnHealthDepleted += DieIGuess;

        Controls.InputActions.Player.Weapon.performed += ctx =>
        {
            SwitchWeaponByNumber(ctx.ReadValue<float>());
        };

        foreach (var weapon in StartingWeapons)
        {
            AddWeapon(weapon);
        }

        SwitchWeaponAscending(true);

        GameManager.Instance.UpdateGameState(GameState.Active);
    }

    private void DieIGuess()
    {
        GameManager.Instance.PlayerHasDied();
    }

    void Update()
    {
        PlayerRotation();
        JumpAndGravity();
        GroundedCheck();
        Move();
    }


    void LateUpdate() 
    {
        UpdateWeaponSwitching();
        UpdateWeaponRecoil();

        WeaponParentSocket.localPosition = weaponMainLocalPosition + weaponRecoilLocalPosition;
        WeaponParentSocket.localRotation = weaponMainLocalRotation;
    }

    private void PlayerRotation()
    {
        Ray ray = Camera.main.ScreenPointToRay(Controls.InputActions.Player.Look.ReadValue<Vector2>());
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, 0f, 0f));
        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            MousePoint = ray.GetPoint(rayDistance);
            Vector3 cameraRootPoint = transform.position + (MousePoint - transform.position) / 5;
            CameraRoot.position = cameraRootPoint;
            Vector3 playersDirectionToMouse = MousePoint - transform.position;
            AimPoint = MousePoint + new Vector3(playersDirectionToMouse.x, 0f, playersDirectionToMouse.z).normalized * MouseAndAimPointDistance;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(playersDirectionToMouse.x, 0f, playersDirectionToMouse.z), Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * RotationSpeed);

            // Position aim arrow
            AimArrow.position = new Vector3(MousePoint.x, AimArrow.position.y, MousePoint.z);

        }
    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
    }

    public bool AddWeapon(WeaponController weaponPrefab)
    {
        if (HasWeapon(weaponPrefab) != null)
        {
            return false;
        }

        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i] == null)
            {
                WeaponController weaponInstance = Instantiate(weaponPrefab, WeaponParentSocket);
                weaponInstance.transform.localPosition = Vector3.zero;
                weaponInstance.transform.localRotation = Quaternion.identity;

                weaponInstance.Owner = gameObject;
                weaponInstance.DummyPlayer = this;
                weaponInstance.SourcePrefab = weaponPrefab.gameObject;
                weaponInstance.ShowWeapon(false);

                weaponSlots[i] = weaponInstance;

                if (OnAddedWeapon != null)
                {
                    OnAddedWeapon.Invoke(weaponInstance, i);
                }

                return true;
            }
        }

        if (GetActiveWeapon() == null)
        {
            SwitchWeaponAscending(true);
        }

        return false;
    }

    public WeaponController HasWeapon(WeaponController weaponPrefab)
    {
        for (var index = 0; index < weaponSlots.Length; index++)
        {
            var w = weaponSlots[index];
            if (w != null && w.SourcePrefab == weaponPrefab.gameObject)
            {
                return w;
            }
        }

        return null;
    }

    public WeaponController GetActiveWeapon()
    {
        return GetWeaponAtSlotIndex(ActiveWeaponIndex);
    }

    public void SwitchWeaponAscending(bool ascendingOrder)
    {
        int newWeaponIndex = -1;
        int closestSlotDistance = weaponSlots.Length;
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (i != ActiveWeaponIndex && GetWeaponAtSlotIndex(i) != null)
            {
                int distanceToActiveIndex = GetDistanceBetweenWeaponSlots(ActiveWeaponIndex, i, ascendingOrder);

                if (distanceToActiveIndex < closestSlotDistance)
                {
                    closestSlotDistance = distanceToActiveIndex;
                    newWeaponIndex = i;
                }
            }
        }

        SwitchToWeaponIndex(newWeaponIndex);
    }

    public void SwitchToWeaponIndex(int newWeaponIndex, bool force = false)
    {
        if (force || (newWeaponIndex != ActiveWeaponIndex && newWeaponIndex >= 0))
        {
            meaponSwitchNewWeaponIndex = newWeaponIndex;
            timeStartedWeaponSwitch = Time.time;

            if (GetActiveWeapon() == null)
            {
                weaponMainLocalPosition = DownWeaponPosition.localPosition;
                SwitchState = WeaponSwitchState.PutUpNew;
                ActiveWeaponIndex = meaponSwitchNewWeaponIndex;

                WeaponController newWeapon = GetWeaponAtSlotIndex(meaponSwitchNewWeaponIndex);


                if (OnSwitchedToWeapon != null)
                {
                    OnSwitchedToWeapon.Invoke(newWeapon);
                }
            }
            else
            {
                SwitchState = WeaponSwitchState.PutDownPrevious;
            }
        }
    }

    private void SwitchWeaponByNumber(float weaponNumber)
    {
        if (SwitchState == WeaponSwitchState.Up || SwitchState == WeaponSwitchState.Down)
        {
            int switchWeaponInput = (int)weaponNumber;
            if (switchWeaponInput != 0)
            {
                if (GetWeaponAtSlotIndex(switchWeaponInput - 1) != null)
                {
                    SwitchToWeaponIndex(switchWeaponInput - 1);
                }
            }
        }
    }

    public WeaponController GetWeaponAtSlotIndex(int index)
    {
        if (index >= 0 &&
            index < weaponSlots.Length)
        {
            return weaponSlots[index];
        }

        return null;
    }

    int GetDistanceBetweenWeaponSlots(int fromSlotIndex, int toSlotIndex, bool ascendingOrder)
    {
        int distanceBetweenSlots = 0;

        if (ascendingOrder)
            distanceBetweenSlots = toSlotIndex - fromSlotIndex;
        else
            distanceBetweenSlots = -1 * (toSlotIndex - fromSlotIndex);

        if (distanceBetweenSlots < 0)
            distanceBetweenSlots = weaponSlots.Length + distanceBetweenSlots;

        return distanceBetweenSlots;
    }

    void OnWeaponSwitched(WeaponController newWeapon)
    {
        if (newWeapon != null)
        {
            newWeapon.ShowWeapon(true);
            activeWeapon = newWeapon;
        }
    }

    private void UpdateWeaponSwitching()
    {
        float switchingTimeFactor = 0f;
        if (WeaponSwitchDelay == 0f)
        {
            switchingTimeFactor = 1f;
        }
        else
        {
            switchingTimeFactor = Mathf.Clamp01((Time.time - timeStartedWeaponSwitch) / WeaponSwitchDelay);
        }

        if (switchingTimeFactor >= 1f)
        {
            if (SwitchState == WeaponSwitchState.PutDownPrevious)
            {
                WeaponController oldWeapon = GetWeaponAtSlotIndex(ActiveWeaponIndex);
                if (oldWeapon != null)
                {
                    oldWeapon.ShowWeapon(false);
                }

                ActiveWeaponIndex = meaponSwitchNewWeaponIndex;
                switchingTimeFactor = 0f;

                WeaponController newWeapon = GetWeaponAtSlotIndex(ActiveWeaponIndex);
                if (OnSwitchedToWeapon != null)
                {
                    OnSwitchedToWeapon.Invoke(newWeapon);
                }

                if (newWeapon)
                {
                    timeStartedWeaponSwitch = Time.time;
                    SwitchState = WeaponSwitchState.PutUpNew;
                }
                else
                {
                    SwitchState = WeaponSwitchState.Down;
                }
            }
            else if (SwitchState == WeaponSwitchState.PutUpNew)
            {
                SwitchState = WeaponSwitchState.Up;
            }
        }

        if (SwitchState == WeaponSwitchState.PutDownPrevious)
        {
            float curve = weaponSwitchCurve.Evaluate(switchingTimeFactor);
            weaponMainLocalPosition = Vector3.Lerp(DefaultWeaponPosition.localPosition, DownWeaponPosition.localPosition, curve);
            weaponMainLocalRotation = Quaternion.Lerp(DefaultWeaponPosition.localRotation, DownWeaponPosition.localRotation, curve);
        }
        else if (SwitchState == WeaponSwitchState.PutUpNew)
        {
            float curve = weaponSwitchCurve.Evaluate(switchingTimeFactor);
            weaponMainLocalPosition = Vector3.Lerp(DownWeaponPosition.localPosition, DefaultWeaponPosition.localPosition, curve);
            weaponMainLocalRotation = Quaternion.Lerp(DownWeaponPosition.localRotation, DefaultWeaponPosition.localRotation, curve);
        }
    }

    private void UpdateWeaponRecoil()
    {
        if (weaponRecoilLocalPosition.z >= accumulatedRecoil.z * 0.99f)
        {
            weaponRecoilLocalPosition = Vector3.Lerp(weaponRecoilLocalPosition, accumulatedRecoil, RecoilSharpness * Time.deltaTime);
        }
        else
        {
            weaponRecoilLocalPosition = Vector3.Lerp(weaponRecoilLocalPosition, Vector3.zero, RecoilRestitutionSharpness * Time.deltaTime);
            accumulatedRecoil = weaponRecoilLocalPosition;
        }
    }

    private void Move()
    {
        float targetSpeed = Controls.InputActions.Player.Sprint.IsPressed() ? SprintSpeed : MoveSpeed;
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        float speedOffset = 0.1f;
        float inputMagnitude = 1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        Vector2 move = Controls.InputActions.Player.Move.ReadValue<Vector2>();
        Vector3 isometricDirection = Quaternion.AngleAxis(CameraYRotationDeg, Vector3.up) * new Vector3(move.x, 0.0f, move.y).normalized;
        Vector3 relativeVelocity = transform.InverseTransformDirection(_controller.velocity);

        _controller.Move(isometricDirection * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        if (_animator)
        {
            _animator.SetFloat("SpeedZ", relativeVelocity.z);
            _animator.SetFloat("SpeedX", relativeVelocity.x);
            _animator.SetFloat("SpeedHorizontal", currentHorizontalSpeed);
        }
    }



    private void JumpAndGravity()
    {
        if (Grounded)
        {
            _fallTimeoutDelta = FallTimeout;

            // Stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            if (Controls.InputActions.Player.Jump.IsPressed() && _jumpTimeoutDelta <= 0.0f)
            {
                // The square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

            }

            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            _jumpTimeoutDelta = JumpTimeout;

            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }

            //_input.jump = false;
        }

        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying == false)
        {
            return;
        }

        if (aimPoint || muzzleToAimRay)
        {
            Ray ray = Camera.main.ScreenPointToRay(Controls.InputActions.Player.Look.ReadValue<Vector2>());
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            if (groundPlane.Raycast(ray, out float rayDistance))
            {
                Vector3 point = ray.GetPoint(rayDistance);

                if (aimPoint) 
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireSphere(point, aimPointRadius);
                }

                if (cameraRootPoint)
                {
                    Vector3 cameraRootPoint = transform.position + (point - transform.position) / 5;
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireSphere(cameraRootPoint, cameraRootPointRadius);
                }

                if (muzzleToAimRay)
                {
                    Gizmos.color = Color.red;
                    //Gizmos.DrawLine(weaponMuzzle.position, point);
                }
            }
        }
    }

    private void OnDestroy()
    {
        _health.OnHealthDepleted -= DieIGuess;
    }

    public enum WeaponSwitchState
    {
        Up,
        Down,
        PutDownPrevious,
        PutUpNew,
    }
}