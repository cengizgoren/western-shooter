using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyWeaponController : WeaponController
{
    private Weapon enemyWeapon;
    private EnemyController enemyController;

    private void Start()
    {
        enemyWeapon = GetComponent<Weapon>();
        enemyController = enemyWeapon.GetOwner().GetComponent<EnemyController>();

        enemyController.OnAttack += CheckAttack;
        GameManager.Instance.OnPause += Forbid;
        GameManager.Instance.OnWon += Forbid;
        GameManager.Instance.OnLost += Forbid;
        GameManager.Instance.OnUnpause += Allow;
    }

    private void OnDestroy()
    {
        enemyController.OnAttack -= CheckAttack;
        GameManager.Instance.OnPause -= Forbid;
        GameManager.Instance.OnWon -= Forbid;
        GameManager.Instance.OnLost -= Forbid;
        GameManager.Instance.OnUnpause -= Allow;
    }

    private void Forbid() => OnShootingForbidden?.Invoke();
    private void Allow() => OnShootingAllowed?.Invoke();

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
