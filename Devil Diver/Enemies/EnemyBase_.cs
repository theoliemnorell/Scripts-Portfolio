using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase_ : MonoBehaviour, IHealth_
{
    protected bool isAlive = true;
    protected float currentHealth;
    [SerializeField] protected float baseHealth;

    protected Vector3 startPosition;
    [SerializeField] protected Vector3[] pathPoint;

    protected int currentTarget = 0;
    protected int previousTarget = 0;
    [SerializeField] protected StateMachine state = StateMachine.Patrol;
    protected Rigidbody rb;
    [SerializeField] protected float movementSpeed, rotationSpeed = 10f;

    protected bool hasStarted = false;

    protected EnemyType enemyType = EnemyType.imp;

    protected float currentDetectionTime;
    [SerializeField] protected float detectionTime = 2, exclamationMarkOffset = 6;
    [SerializeField] GameObject exclamationMark;
    protected int currentExclamationMarks, maxExclamationMarks = 1;

    [SerializeField] LayerMask detectionLayers;
    [SerializeField] protected Transform target => GameObject.Find("Player").transform;
    [SerializeField] protected float detectionRange, attackRange;
    protected bool hasDetected = false;

    protected float chaseTimer = 0;
    [SerializeField] protected float  chaseTime = 10f, chaseDistance = 15f;

    KillCounter_ killCounter;

    [SerializeField] FMODUnity.EventReference impDamaged;
    [SerializeField] FMODUnity.EventReference mageDamaged;
    [SerializeField] FMODUnity.EventReference death;
    [SerializeField] FMODUnity.EventReference alert;
    [SerializeField] FMODUnity.EventReference combatDialogue;

    [SerializeField] GameObject ragdollArmature;
    public enum StateMachine
    {
        Patrol, Detection, Attack, Chase
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        ragdollArmature.SetActive(false);
        killCounter = FindObjectOfType<KillCounter_>();
        ResetHealth();
        hasStarted = true;
        startPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }
    public StateMachine GetState()
    {
        return state;
    }
    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        if (GetCurrentHealth() <= 0)
        {

            Death();
        }
        if (isAlive)
            switch (state)
            {
                case StateMachine.Patrol:
                    Rotate(startPosition + pathPoint[currentTarget]);
                    PatrolMovement();
                    DetectingTarget();
                    break;
                case StateMachine.Detection:
                    Rotate(target.position);
                    Detection();
                    //DetectingTarget();
                    break;
                case StateMachine.Attack:
                    Rotate(target.position);
                    Attack();
                    DetectingTarget();

                    break;
                case StateMachine.Chase:
                    Rotate(target.position);
                    DetectingTarget();
                    Chase();
                    FMODUnity.RuntimeManager.PlayOneShot(combatDialogue, transform.position);
                    break;

            }
    }

    public bool CheckDetection()
    {
        return hasDetected;
    }

    protected virtual void MoveTowardsTarget()
    {
        Vector3 pos = Vector3.MoveTowards(transform.position, target.position/*new Vector3(target.position.x, target.position.y, target.position.z)*/, movementSpeed * Time.deltaTime);
        rb.MovePosition(pos);
    }
    protected virtual void Attack()
    {
        chaseTimer = 0;
    }

    protected void Rotate(Vector3 target)
    {
        Vector3 currentDirection = (target - transform.position).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(currentDirection.x, 0, currentDirection.z)), Time.deltaTime * rotationSpeed);
    }
    protected void Chase()
    {
        MoveTowardsTarget();
        chaseTimer += Time.deltaTime;
        if (chaseTimer >= chaseTime || Vector3.Distance(transform.position, target.position) >= chaseDistance)
        {
            chaseTimer = 0;
            state = StateMachine.Patrol;
            hasDetected = false;
        }

    }
    protected void PatrolMovement()
    {




        // Debug.Log(transform.position + "_" + startPosition + new Vector3(pathPoint[currentTarget].x, transform.position.y, pathPoint[currentTarget].z));

        if (Vector3.Distance(transform.position, startPosition + pathPoint[currentTarget]) >= 1.5f)
        {
            Vector3 pos = Vector3.MoveTowards(transform.position, startPosition + new Vector3(pathPoint[currentTarget].x, transform.position.y, pathPoint[currentTarget].z), movementSpeed * Time.deltaTime);
            rb.MovePosition(pos);
        }
        else
        {
            currentTarget = Random.Range(0, pathPoint.Length);
        }
    }


    protected void DetectingTarget()
    {
        Color color = Color.red;
        RaycastHit raycastHit;

        Physics.Raycast(transform.position, target.position - transform.position, out raycastHit, detectionRange, detectionLayers);

        bool hitPlayer = false;
        if (raycastHit.collider != null)
        {
            hitPlayer = raycastHit.collider.tag == "Player";


            if (Vector3.Distance(transform.position, target.position) <= detectionRange && hitPlayer && !hasDetected)
            {

                state = StateMachine.Detection;
                FMODUnity.RuntimeManager.PlayOneShot(alert, transform.position);
                color = Color.blue;
            }
            else if (Vector3.Distance(transform.position, target.position) <= attackRange && hitPlayer && hasDetected)
            {
                state = StateMachine.Attack;
                color = Color.green;
            }
            else if (hasDetected)
            {
                state = StateMachine.Chase;
            }

        }
        Debug.DrawLine(transform.position, target.position, color);
    }



    protected void Detection()
    {
        currentDetectionTime += Time.deltaTime;
        if (currentDetectionTime <= detectionTime)
        {
            if (currentExclamationMarks <= maxExclamationMarks)
            {
                GameObject newExclamationMark = Instantiate(exclamationMark, new Vector3(transform.position.x, exclamationMarkOffset, transform.position.z), exclamationMark.transform.rotation);
                Destroy(newExclamationMark, detectionTime);
                currentExclamationMarks++;
            }


        }
        else
        {
            currentExclamationMarks = 0;
            hasDetected = true;
            state = StateMachine.Chase;
            currentDetectionTime = 0;
        }
    }








    protected virtual void OnDrawGizmos()
    {

        Gizmos.color = Color.red;

        if (hasStarted)
        {
            for (int i = 0; i < pathPoint.Length; i++)
            {
                Gizmos.DrawSphere(startPosition + pathPoint[i], 0.5f);

            }
        }
        else
        {
            for (int i = 0; i < pathPoint.Length; i++)
            {
                Gizmos.DrawSphere(transform.position + pathPoint[i], 0.5f);

            }
        }

        Gizmos.color = new Color(0, 0, 1, 0.1f);

        Gizmos.DrawSphere(transform.position, detectionRange);

        Gizmos.color = new Color(0, 1, 0, 0.1f);

        Gizmos.DrawSphere(transform.position, attackRange);


    }

    protected virtual void Death()
    {
        
        
            isAlive = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            GetComponent<CapsuleCollider>().enabled = false;
            ragdollArmature.SetActive(true);
        
    }

    public bool CheckIfAlive()
    {
        return isAlive;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetBaseHealth()
    {
        return baseHealth;
    }

    public void ChangeHealth(float value)
    {
        currentHealth += value;

        if (value<0 && currentHealth > 0)
        {
           switch (enemyType)
            {
                case EnemyType.imp:
                    FMODUnity.RuntimeManager.PlayOneShot(impDamaged, transform.position);
                    break;
                case EnemyType.mage:
                    FMODUnity.RuntimeManager.PlayOneShot(mageDamaged, transform.position);
                    break;
            }
        }
        if (currentHealth  <= 0 && isAlive)
        {
            killCounter.AddToKillCount();
            FMODUnity.RuntimeManager.PlayOneShot(death, transform.position);
        }
    }

    public void ResetHealth()
    {
        currentHealth = baseHealth;
    }

    public void SetHealth(float value)
    {
        currentHealth = value;
    }

    public EnemyType GetEnemyType()
    {
        return enemyType;
    }

    public enum EnemyType
    {
        imp,
        mage
    }
}
