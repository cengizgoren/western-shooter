using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DummyWeaponsManager : MonoBehaviour
{
    public enum WeaponSwitchState
    {
        Up,
        Down,
        PutDownPrevious,
        PutUpNew,
    }

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

    private readonly WeaponController[] weaponSlots = new WeaponController[9];

    private float timeStartedWeaponSwitch;
    private int meaponSwitchNewWeaponIndex;
    private WeaponController activeWeapon;

    private Vector3 weaponMainLocalPosition;
    private Quaternion weaponMainLocalRotation;
    private Vector3 accumulatedRecoil;
    private Vector3 weaponRecoilLocalPosition;

    // Components
    private InputActions inputActions;

    // Events
    public UnityAction<WeaponController> OnSwitchedToWeapon;
    private UnityAction<WeaponController, int> OnAddedWeapon;

    // Properties
    public int ActiveWeaponIndex { get; private set; }
    public WeaponSwitchState SwitchState { get; private set; }

    private void Awake()
    {
        inputActions = new InputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Start()
    {
        ActiveWeaponIndex = -1;
        SwitchState = WeaponSwitchState.Down;

        weaponMainLocalPosition = DefaultWeaponPosition.localPosition;
        weaponMainLocalRotation = DefaultWeaponPosition.localRotation;

        OnSwitchedToWeapon += OnWeaponSwitched;

        inputActions.Player.Weapon.performed += ctx =>
        {
            SwitchWeaponByNumber(ctx.ReadValue<float>());
        };

        foreach (var weapon in StartingWeapons)
        {
            AddWeapon(weapon);
        }

        SwitchWeaponAscending(true);
    }

    private void Update() { }

    private void LateUpdate()
    {
        UpdateWeaponSwitching();
        UpdateWeaponRecoil();

        WeaponParentSocket.localPosition = weaponMainLocalPosition + weaponRecoilLocalPosition;
        WeaponParentSocket.localRotation = weaponMainLocalRotation;
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
                weaponInstance.WeaponManager = this;
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
            newWeapon.OnShootRecoil += ApplyRecoil;
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
                    // oldWeapon.OnShootRecoil -= ApplyRecoil;  // Probably no need to unsubscribe if not destroying anything
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

    private void ApplyRecoil(float recoilForce)
    {
        accumulatedRecoil += Vector3.back * recoilForce;
        accumulatedRecoil = Vector3.ClampMagnitude(accumulatedRecoil, MaxRecoilDistance);
    }
}
