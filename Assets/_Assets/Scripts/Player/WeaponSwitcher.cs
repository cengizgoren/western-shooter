using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(WeaponArsenal))]
public class WeaponSwitcher : MonoBehaviour
{
    public UnityAction<Weapon> OnSwitchedToWeapon;
    public UnityAction<bool> OnWeaponReady;
   

    public Transform WeaponParentSocket;
    public Transform DefaultWeaponPosition;
    public Transform DownWeaponPosition;
    [Space(10)]
    public WeaponSwitchState switchState;
    public int ActiveWeaponIndex;
    public float WeaponSwitchDelay = 1f;
    public AnimationCurve weaponSwitchCurve;

    private WeaponArsenal playerWeaponArsenal;

    private float timeStartedWeaponSwitch;
    private int meaponSwitchNewWeaponIndex;
    private Weapon activeWeapon;
    private Vector3 weaponMainLocalPosition;
    private Quaternion weaponMainLocalRotation;

    private void Start()
    {
        ActiveWeaponIndex = -1;
        playerWeaponArsenal = GetComponent<WeaponArsenal>();

        UpdateState(WeaponSwitchState.Down);
        weaponMainLocalPosition = DefaultWeaponPosition.localPosition;
        weaponMainLocalRotation = DefaultWeaponPosition.localRotation;
        OnSwitchedToWeapon += OnWeaponSwitched;

        Controls.InputActions.Player.Weapon.performed += SwitchWeaponByNumber;

        SwitchWeaponAscending(true);
    }

    private void OnDestroy()
    {
        Controls.InputActions.Player.Weapon.performed -= SwitchWeaponByNumber;
    }

    private void SwitchWeaponByNumber(InputAction.CallbackContext context)
    {
        float weaponNumber = context.ReadValue<float>();
        if (switchState == WeaponSwitchState.Up || switchState == WeaponSwitchState.Down)
        {
            int switchWeaponInput = (int)weaponNumber;
            if (switchWeaponInput != 0)
            {
                if (playerWeaponArsenal.GetWeaponAtSlotIndex(switchWeaponInput - 1) != null)
                {
                    SwitchToWeaponIndex(switchWeaponInput - 1);
                }
            }
        }
    }

    public Weapon GetActiveWeapon()
    {
        return playerWeaponArsenal.GetWeaponAtSlotIndex(ActiveWeaponIndex);
    }

    // Temporary workaround to decouple other calss
    private void UpdateState(WeaponSwitchState newState)
    {
        switchState = newState;

        switch (switchState)
        {
            case WeaponSwitchState.Up:
                OnWeaponReady?.Invoke(true);
                break;
            case WeaponSwitchState.Down:
                OnWeaponReady?.Invoke(false);
                break;
            case WeaponSwitchState.PutDownPrevious:
                OnWeaponReady?.Invoke(false);
                break;
            case WeaponSwitchState.PutUpNew:
                OnWeaponReady?.Invoke(false);
                break;
        }
    }

    private void Update()
    {
        UpdateWeaponSwitching();

        WeaponParentSocket.localPosition = weaponMainLocalPosition;
        WeaponParentSocket.localRotation = weaponMainLocalRotation;
    }

    private void OnWeaponSwitched(Weapon newWeapon)
    {
        if (newWeapon != null)
        {
            newWeapon.gameObject.SetActive(true);
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
            if (switchState == WeaponSwitchState.PutDownPrevious)
            {
                Weapon oldWeapon = playerWeaponArsenal.GetWeaponAtSlotIndex(ActiveWeaponIndex);
                if (oldWeapon != null)
                {
                    oldWeapon.gameObject.SetActive(false);
                }

                ActiveWeaponIndex = meaponSwitchNewWeaponIndex;
                switchingTimeFactor = 0f;

                Weapon newWeapon = playerWeaponArsenal.GetWeaponAtSlotIndex(ActiveWeaponIndex);
                if (OnSwitchedToWeapon != null)
                {
                    OnSwitchedToWeapon.Invoke(newWeapon);
                }

                if (newWeapon)
                {
                    timeStartedWeaponSwitch = Time.time;
                    UpdateState(WeaponSwitchState.PutUpNew);
                }
                else
                {
                    UpdateState(WeaponSwitchState.Down);
                }
            }
            else if (switchState == WeaponSwitchState.PutUpNew)
            {
                UpdateState(WeaponSwitchState.Up);
            }
        }

        if (switchState == WeaponSwitchState.PutDownPrevious)
        {
            float curve = weaponSwitchCurve.Evaluate(switchingTimeFactor);
            weaponMainLocalPosition = Vector3.Lerp(DefaultWeaponPosition.localPosition, DownWeaponPosition.localPosition, curve);
            weaponMainLocalRotation = Quaternion.Lerp(DefaultWeaponPosition.localRotation, DownWeaponPosition.localRotation, curve);
        }
        else if (switchState == WeaponSwitchState.PutUpNew)
        {
            float curve = weaponSwitchCurve.Evaluate(switchingTimeFactor);
            weaponMainLocalPosition = Vector3.Lerp(DownWeaponPosition.localPosition, DefaultWeaponPosition.localPosition, curve);
            weaponMainLocalRotation = Quaternion.Lerp(DownWeaponPosition.localRotation, DefaultWeaponPosition.localRotation, curve);
        }
    }

    public void SwitchWeaponAscending(bool ascendingOrder)
    {
        int newWeaponIndex = -1;
        int closestSlotDistance = WeaponArsenal.WeaponSlotsNumber;
        for (int i = 0; i < WeaponArsenal.WeaponSlotsNumber; i++)
        {
            if (i != ActiveWeaponIndex && playerWeaponArsenal.GetWeaponAtSlotIndex(i) != null)
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
                UpdateState(WeaponSwitchState.PutUpNew);
                ActiveWeaponIndex = meaponSwitchNewWeaponIndex;

                Weapon newWeapon = playerWeaponArsenal.GetWeaponAtSlotIndex(meaponSwitchNewWeaponIndex);

                if (OnSwitchedToWeapon != null)
                {
                    OnSwitchedToWeapon.Invoke(newWeapon);
                }
            }
            else
            {
                UpdateState(WeaponSwitchState.PutDownPrevious);
            }
        }
    }

    private int GetDistanceBetweenWeaponSlots(int fromSlotIndex, int toSlotIndex, bool ascendingOrder)
    {
        int distanceBetweenSlots = 0;

        if (ascendingOrder)
            distanceBetweenSlots = toSlotIndex - fromSlotIndex;
        else
            distanceBetweenSlots = -1 * (toSlotIndex - fromSlotIndex);

        if (distanceBetweenSlots < 0)
            distanceBetweenSlots = WeaponArsenal.WeaponSlotsNumber + distanceBetweenSlots;

        return distanceBetweenSlots;
    }

    public enum WeaponSwitchState
    {
        Up,
        Down,
        PutDownPrevious,
        PutUpNew,
    }
}
