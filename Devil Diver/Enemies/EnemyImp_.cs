using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyImp_ : EnemyBase_
{
    protected float attackRateTimer;
    [SerializeField] float meleeAttackRange = 1, attackRate = 0.6f, meleeDamage = 20;
    [SerializeField] Animator impAnimator;
    bool isAttacking = false;
    [SerializeField] FMODUnity.EventReference impAttack;
    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
    }

    EnemyType enemyType => EnemyType.imp;
    
    // Update is called once per frame
    override protected void FixedUpdate()
    {
        base.FixedUpdate();

        impAnimator.SetBool("Stab", isAttacking&&state == StateMachine.Attack);
        impAnimator.SetBool("Idle", state == StateMachine.Detection);
        impAnimator.SetBool("Run", state == StateMachine.Patrol || state == StateMachine.Chase);

    }
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = new Color(1, 0, 0, 0.1f);

        Gizmos.DrawSphere(transform.position, meleeAttackRange);


    }

    protected override void Death()
    {
        base.Death();
        impAnimator.enabled = false;
    }
    override protected void Attack()
    {
        base.Attack();
        MoveTowardsTarget();
        if (Vector3.Distance(transform.position, target.position) <= meleeAttackRange)
        {
            attackRateTimer += Time.deltaTime;
            if (attackRateTimer >= attackRate)
            {
                if (Vector3.Distance(transform.position, target.position) <= meleeAttackRange)
                {
                    FMODUnity.RuntimeManager.PlayOneShot(impAttack, transform.position);
                        isAttacking = true;
                    IHealth_ hitHealth = target.transform.GetComponentInParent<IHealth_>();
                    if (hitHealth != null)
                    {
                        Debug.Log("Hit Damage!" + target.name);
                        hitHealth.ChangeHealth(-meleeDamage);

                    }
                    
                    Debug.Log(this + " stab");
                }
                else isAttacking = false;
                attackRateTimer = 0;
            }
        }

    }
}
