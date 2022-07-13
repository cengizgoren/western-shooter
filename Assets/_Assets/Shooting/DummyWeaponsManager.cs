using System;
using UnityEngine;

public class DummyWeaponsManager : MonoBehaviour
{
    public WeaponController currentWeapon;
    public Transform WeaponParentSocket;
    public Transform DefaultWeaponPosition;

    public float MaxRecoilDistance = 0.5f;
    public float RecoilSharpness = 50f;
    public float RecoilRestitutionSharpness = 10f;
    
    private WeaponController activeWeapon;
    private DummyInput _input;
    private InputActions _inputActions;

    private Vector3 m_WeaponMainLocalPosition;
    private Vector3 m_AccumulatedRecoil;
    private Vector3 m_WeaponRecoilLocalPosition;

    private void Awake()
    {
        _inputActions = new InputActions();
    }

    private void OnEnable()
    {
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }

    private void Start()
    {
        _input = GetComponent<DummyInput>();
        activeWeapon = Instantiate(currentWeapon, WeaponParentSocket);
        activeWeapon.OnShootRecoil += ApplyRecoil;
        m_WeaponMainLocalPosition = DefaultWeaponPosition.localPosition;
    }

    void Update() { }

    void LateUpdate()
    {
        UpdateWeaponRecoil();
        WeaponParentSocket.localPosition = m_WeaponMainLocalPosition + m_WeaponRecoilLocalPosition;
    }

    private void ApplyRecoil(object sender, EventArgs e)
    {
        m_AccumulatedRecoil += Vector3.back * activeWeapon.RecoilForce;
        m_AccumulatedRecoil = Vector3.ClampMagnitude(m_AccumulatedRecoil, MaxRecoilDistance);
    }

    void UpdateWeaponRecoil()
    {
        // if the accumulated recoil is further away from the current position, make the current position move towards the recoil target
        if (m_WeaponRecoilLocalPosition.z >= m_AccumulatedRecoil.z * 0.99f)
        {
            m_WeaponRecoilLocalPosition = Vector3.Lerp(m_WeaponRecoilLocalPosition, m_AccumulatedRecoil, RecoilSharpness * Time.deltaTime);
        }
        // otherwise, move recoil position to make it recover towards its resting pose
        else
        {
            m_WeaponRecoilLocalPosition = Vector3.Lerp(m_WeaponRecoilLocalPosition, Vector3.zero, RecoilRestitutionSharpness * Time.deltaTime);
            m_AccumulatedRecoil = m_WeaponRecoilLocalPosition;
        }
    }
}
