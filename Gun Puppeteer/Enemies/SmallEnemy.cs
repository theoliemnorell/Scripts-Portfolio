using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class SmallEnemy : EnemyMatryohska
{
    [SerializeField] float attackTime = 1;
    float attackTimeCurrent = 0;
    protected override void Attack(float deltaTime)
    {

        chaseTimer = 0;


        if (weaponPrefab != null)
        {
            
            weapon.Rotate((Vector2)transform.position + weaponPositionOffset, target.position, weaponDistance, deltaTime * aimSpeed);
           
                attackTimeCurrent = 0;
                weapon.Shoot();
           
           
        }






        if (target.position.x < transform.position.x)
        {
            direction = -1;
        }
        else
        {
            direction = 1;
        }

        if (matryoshkaScript.IsGrounded)
        {

            Movement(deltaTime, direction);
        }

    }

    protected override bool PatrolIsGrounded()
    {
        return state == StateMachine.Patrol ? matryoshkaScript.IsUprightOnGround : matryoshkaScript.IsGrounded;
    }

}
