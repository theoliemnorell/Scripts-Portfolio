using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCultistMage_ : EnemyBase_
{
    [SerializeField] protected GameObject projectilePrefab;
    protected float attackRateTimer;
    [SerializeField] float attackRate = 0.6f;
    [SerializeField] Animator cultistAnimator;
    [SerializeField] FMODUnity.EventReference mageAttack;
    [SerializeField] float shootOffset = 2f;
    EnemyType enemyType => EnemyType.mage;
    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    override protected void FixedUpdate()
    {
        base.FixedUpdate();
        cultistAnimator.SetBool("Idle", state == StateMachine.Detection || state == StateMachine.Attack);
        cultistAnimator.SetBool("Walk", state == StateMachine.Patrol || state == StateMachine.Chase);
    }
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
       
    }


    protected override void Death()
    {
        base.Death();
        cultistAnimator.enabled = false;
    }


    override protected void Attack()
    {
        base.Attack();
        attackRateTimer += Time.deltaTime;
        if (attackRateTimer >= attackRate)
        {
            cultistAnimator.SetTrigger("Shoot");
            FMODUnity.RuntimeManager.PlayOneShot(mageAttack, transform.position);
            GameObject newProjectile = Instantiate(projectilePrefab, new Vector3(transform.position.x,transform.position.y,transform.position.z) + transform.forward * shootOffset ,transform.rotation);

            attackRateTimer = 0;
        }

    }
}