using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponController : WeaponController
{
    //[SerializeField]  // Debug trick, good to remember
    private Weapon enemyWeapon;
    //[SerializeField]
    private EnemyController enemyController;

    private void Start()
    {
        enemyWeapon = GetComponent<Weapon>();
        enemyController = enemyWeapon.GetOwner().GetComponent<EnemyController>();

        enemyController.OnAttack += isAttacking => CheckAttack(isAttacking);
        GameManager.Instance.OnPause += () => { base.OnShootingForbidden?.Invoke(); };
        GameManager.Instance.OnUnpause += () => { base.OnShootingAllowed?.Invoke(); };
    }

    private void CheckAttack(bool isAttacking)
    {
        if (isAttacking)
        {
            base.OnTriggerPressed?.Invoke();
        }
        else
        {
            base.OnTriggerReleased?.Invoke();
        }
    }
}
