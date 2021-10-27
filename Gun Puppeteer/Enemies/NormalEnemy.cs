using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NormalEnemy : EnemyMatryohska
{



    protected override void Start()
    {
        base.Start();

    }


    protected override void Attack(float deltaTime)
    {
        chaseTimer = 0;
        if (weaponPrefab != null)
        {
            weapon.Rotate((Vector2)transform.position + weaponPositionOffset, target.position, weaponDistance, deltaTime * aimSpeed);
            weapon.Shoot();
        }
    }
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

    }

    protected override bool PatrolIsGrounded()
    {
        return state == StateMachine.Patrol ? matryoshkaScript.IsUprightOnGround : matryoshkaScript.IsGrounded;
    }
}
