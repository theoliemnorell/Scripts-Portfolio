using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BigEnemy : EnemyMatryohska
{
    [SerializeField] private float waitTime, angularDragPatrol = 7, angularDragChase = 4;
    private float waitTimeCurrent;
    private bool isWaiting, hasReachedPositionPoint;

    protected override void Attack(float deltaTime)
    {
        chaseTimer = 0;
        weapon.Rotate((Vector2)transform.position + weaponPositionOffset, target.position, weaponDistance, deltaTime * aimSpeed);
        weapon.Shoot();
    }

    protected override bool PatrolIsGrounded()
    {
        return matryoshkaScript.IsGrounded;
    }

    protected override void Movement(float deltaTime, float direction)
    {
        rigidbody2D.angularDrag = 0.05f;
        MovementType movement = movementTypes.First(movementType => movementType.targetState == state);


        if (movementTypes.Any(movementType => movementType.targetState == state))
        {
            //To make enemy wait x time before switching direction when reaching position point. 

            if (rigidbody2D.position.x <= patrolLeftEdge || rigidbody2D.position.x >= patrolRightEdge)
            {
                hasReachedPositionPoint = true;
            }



            if (hasReachedPositionPoint)
            {

                waitTimeCurrent += deltaTime;
                if (waitTimeCurrent >= waitTime)
                {

                    isWaiting = false;
                    hasReachedPositionPoint = false;
                }
                else
                {
                    isWaiting = true;
                }
            }
            else
            {
                waitTimeCurrent = 0;
            }



            if (!isWaiting)
            {
                if (state == StateMachine.Patrol||state == StateMachine.PatrolSlopes)
                {
                    rigidbody2D.angularDrag = angularDragPatrol;
                }
                else if (state == StateMachine.Chase)
                {
                    rigidbody2D.angularDrag = angularDragChase;
                }
               


                currentTime += deltaTime;
                if (currentTime >= movement.timeBetweenJump)
                {
                    currentTime = 0;
                    rigidbody2D.AddForce(new Vector2(movement.jumpForceHorizontal * direction, movement.jumpForceUp) * mass);
                    rigidbody2D.AddTorque(movement.torqueForce * direction, ForceMode2D.Impulse);
                }
            }
            else
            {
                rigidbody2D.angularDrag = 0.05f;
            }

        }
    }
}
