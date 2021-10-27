using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[RequireComponent(typeof(Matryoshka))]
public abstract class EnemyMatryohska : Entity, IHealth
{
    [SerializeField] GameObject[] innerEnemiesPrefabs;
    [Range(0, 100)]
    [SerializeField] int doNotSpawnInnerEnemyPercentage;
    protected Matryoshka matryoshkaScript;
    [SerializeField] private float baseHealth;
    private float currentHealth = -1;
    [SerializeField] public GameObject enemyPrefab, weaponPrefab;
    [SerializeField] string prefabName;
    protected Weapon weapon;
    [SerializeField] protected float weaponDistance;
    [SerializeField] protected float aimSpeed;
    [SerializeField] private SpriteRenderer crackSpriteRenderer;
    private Material crackMaterial;
    protected Animator enemyAnimator;
    protected new Rigidbody2D rigidbody2D;
    bool hasStarted = false;
    public Enemy Enemy
    {
        get
        {
            Enemy enemy = new Enemy();
            enemy.gameObject = gameObject;
            enemy.spawnPos = transform.position;
            enemy.prefab = enemyPrefab;
            return enemy;
        }
    }

    public GameObject WeaponGameObject
    {
        get
        {
            if (weapon != null)
            {

                return weapon.gameObject;
            }
            else
            {
                return null;
            }
        }
    }


    protected override void Start()
    {
        base.Start();
        hasStarted = true;
        audioController = GetComponent<MatryoshkaAudioController>();
        enemyAnimator = GetComponent<Animator>();
        matryoshkaScript = GetComponent<Matryoshka>();
        target = FindObjectOfType<PlayerController>().transform;
        rigidbody2D = GetComponent<Rigidbody2D>();
        hatController = GetComponent<HatController>();

        if (currentHealth <= 0)
        {
            currentHealth = baseHealth;
        }

        crackMaterial = crackSpriteRenderer.material;
        mass = rigidbody2D.mass;
        onGround = matryoshkaScript.IsGrounded;
        worldStartXPos = rigidbody2D.position.x;
        UpdateVisuals();
        if (weaponPrefab != null)
        {
            weapon = Instantiate(weaponPrefab, transform.position, transform.rotation).gameObject.GetComponent<Weapon>();
            weapon.HolderTransform = transform;
        }

    }

    //States
    HatController hatController;
    bool onGround;
    MatryoshkaAudioController audioController;
    bool hatIsOff = false;

    //"small", "medium" or "large"
    [SerializeField] string enemySize;
    [SerializeField] bool enableAudio;
    [SerializeField] protected float direction = 1;
    [SerializeField] protected bool hasDetected = false;
    [SerializeField] protected float currentDetectionTime = 0, detectionTime = 2, currentExclamationMarks = 0, maxExclamationMarks = 1, detectionRange = 6;
    [SerializeField] protected StateMachine state = StateMachine.Patrol;
    [SerializeField] protected LayerMask rayCastObstacleLayerMask;
    [SerializeField] protected ContactFilter2D raycastTargetContactFilter2D;
    [SerializeField] protected GameObject exclamationMark;
    [SerializeField] protected Vector2 weaponPositionOffset;
    [SerializeField] protected MovementType[] movementTypes;
    [SerializeField] protected LayerMask slopeLayerMask;
    [SerializeField] protected float currentTime = 0;
    [SerializeField] protected float patrolLeftEdge, patrolRightEdge;
    [SerializeField] protected float chaseTimer = 0, chaseTimerMax = 10f, maxDistanceUntilForget = 15f, maxDistanceToWeapon = 2f;
    protected Transform target;
    protected float mass, worldStartXPos, lowerBoundrary = -20;

    public float LowerBoundrary
    {
        set
        {
            lowerBoundrary = value;
        }
    }

    public enum StateMachine
    {
        Patrol, PatrolSlopes, Detection, Attack, Chase
    }

    public void ResetState()
    {
        state = StateMachine.Patrol;
        hasDetected = false;
    }

    public void Maintenance()
    {
        if (transform.position.y < lowerBoundrary)
        {
            Destroy(weapon.gameObject);
            Destroy(gameObject);
        }
        else if (weapon != null && Vector2.Distance(transform.position, weapon.transform.position) > maxDistanceToWeapon)
        {
            weapon.transform.position = transform.position;
        }
    }

    public override void Process(float deltaTime)
    {
        switch (state)
        {
            case StateMachine.Patrol:
                PlayLandAudio();
                Patrol(deltaTime);
                DetectionCircle();
                break;

            case StateMachine.PatrolSlopes:
                PlayLandAudio();
                Patrol(deltaTime);
                DetectionCircle();
                break;
            case StateMachine.Detection:
                PlayLandAudio();
                DetectionState(deltaTime);
                break;

            case StateMachine.Attack:
                PlayLandAudio();
                Attack(deltaTime);
                DetectionCircle();

                break;

            case StateMachine.Chase:
                PlayLandAudio();
                Chase(deltaTime);
                DetectionCircle();
                break;

            default:
                break;
        }
    }
    //abstract behaviours
    protected abstract void Attack(float deltaTime);

    protected virtual void Patrol(float deltaTime)
    {
        if (PatrolIsGrounded())
        {
            Movement(deltaTime, direction);
        }

        if (rigidbody2D.position.x >= worldStartXPos + patrolRightEdge)
        {
            direction = -1;
        }
        else if (rigidbody2D.position.x <= worldStartXPos + patrolLeftEdge)
        {
            direction = 1;
        }

        if (weapon != null) weapon.Rotate((Vector2)transform.position + weaponPositionOffset, 90 * -direction, weaponDistance);

        //switches between slope patrol and normal patrol when overlap circle detects slope layer
        bool slopeOverlapCircleResult = Physics2D.OverlapCircle(transform.position, 1.5f, slopeLayerMask);

        if (slopeOverlapCircleResult && state == StateMachine.Patrol)
        {
            state = StateMachine.PatrolSlopes;
        }
        else if (!slopeOverlapCircleResult && state == StateMachine.PatrolSlopes)
        {
            state = StateMachine.Patrol;

        }
    }

    protected abstract bool PatrolIsGrounded();

    protected virtual void Chase(float deltaTime)
    {
        if (weaponPrefab != null) weapon.Rotate((Vector2)transform.position + weaponPositionOffset, 90 * -direction, weaponDistance);
        chaseTimer += deltaTime;
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

        if (chaseTimer >= chaseTimerMax || Vector2.Distance(transform.position, target.position) >= maxDistanceUntilForget)
        {
            hasDetected = false;
            state = StateMachine.Patrol;
            chaseTimer = 0;
        }

    }
    protected virtual void Movement(float deltaTime, float direction)
    {

        MovementType movement = movementTypes.First(movementType => movementType.targetState == state);

        if (movementTypes.Any(movementType => movementType.targetState == state))
        {
            currentTime += deltaTime;
            if (currentTime >= movement.timeBetweenJump)
            {
                currentTime = 0;
                rigidbody2D.AddForce(new Vector2(movement.jumpForceHorizontal * direction, movement.jumpForceUp) * mass);
                rigidbody2D.AddTorque(movement.torqueForce * direction, ForceMode2D.Impulse);
                // audioController.Move(rigidbody2D.velocity.y);
            }

        }

    }


    protected void DetectionCircle()
    {

        Color color = Color.red;
        RaycastHit2D[] raycastHits = new RaycastHit2D[1];
        //Physics2D.Raycast(transform.position, target.position - transform.position, detectionRange, rayCastObstacleLayerMask);
        Physics2D.Raycast(transform.position, target.position - transform.position, raycastTargetContactFilter2D, raycastHits, detectionRange);

        bool hitPlayer = false;
        if (raycastHits[0].collider != null)
        {
            hitPlayer = raycastHits[0].collider.tag == "Player";
        }
        if (Vector2.Distance(transform.position, target.position) <= detectionRange && hitPlayer)
        {
            if (!hasDetected)
            {
                hasDetected = true;
                state = StateMachine.Detection;
                audioController.Grunt(enemySize);
            }
            else
            {
                state = StateMachine.Attack;
            }
            color = Color.green;

        }
        else if (hasDetected)
        {
            state = StateMachine.Chase;
        }


        Debug.DrawLine(transform.position, target.position, color);



    }

    private void DetectionState(float deltaTime)
    {
        if (weaponPrefab != null) weapon.Rotate((Vector2)transform.position + weaponPositionOffset, 90 * -direction, weaponDistance);
        currentDetectionTime += deltaTime;
        if (currentDetectionTime <= detectionTime)
        {

            if (currentExclamationMarks <= maxExclamationMarks)
            {

                GameObject newExclamationMark = Instantiate(exclamationMark, transform.position + new Vector3(Random.Range(-10, 10), Random.Range(2, 8), 0), exclamationMark.transform.rotation);
                newExclamationMark.GetComponent<FollowTarget>().Target = transform;
                Destroy(newExclamationMark, detectionTime);
                currentExclamationMarks++;
            }
        }
        else
        {
            state = StateMachine.Chase;
            currentDetectionTime = 0;
            currentExclamationMarks = 0;
        }
    }

    private void PlayLandAudio()
    {
        if (enableAudio)
        {

            if (matryoshkaScript.IsGrounded && !onGround)
            {
                Debug.Log("LAND");
                audioController.Land(-rigidbody2D.velocity.y);
                onGround = true;
            }
            if (!matryoshkaScript.IsGrounded && onGround)
            {
                Debug.Log("LAND");
                audioController.Move(rigidbody2D.velocity.y);
                onGround = false;
            }
        }
    }

    protected virtual void OnDrawGizmos()
    {
        Color colorDetectionCircle = Color.red;
        colorDetectionCircle.a = .1f;
        Gizmos.color = colorDetectionCircle;
        Gizmos.DrawSphere(transform.position, detectionRange);

        if (!hasStarted)
        {

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(new Vector3(transform.position.x + patrolLeftEdge, transform.position.y), .25f);

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(new Vector3(transform.position.x + patrolRightEdge, transform.position.y), .25f);
        }
        if (hasStarted)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(new Vector3(worldStartXPos + patrolLeftEdge, transform.position.y), .25f);

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(new Vector3(worldStartXPos + patrolRightEdge, transform.position.y), .25f);
        }


        Color colorSlopeCircle = Color.cyan;
        colorSlopeCircle.a = .5f;
        Gizmos.color = colorSlopeCircle;
        Gizmos.DrawSphere(transform.position, 1.5f);


    }

    protected void Death()
    {
        if (innerEnemiesPrefabs.Length != 0 && Random.Range(1, 101) > doNotSpawnInnerEnemyPercentage)
        {
            Instantiate(innerEnemiesPrefabs[Random.Range(0, innerEnemiesPrefabs.Length)], transform.position, transform.rotation).gameObject.GetComponent<EnemyMatryohska>().LowerBoundrary = lowerBoundrary;
        }
        if (weaponPrefab != null) Destroy(weapon.gameObject);
        Destroy(gameObject);

    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetBaseHealth()
    {
        return baseHealth;
    }



    public void ResetHealth()
    {
        currentHealth = baseHealth;
        UpdateVisuals();
    }

    public void SetHealth(float value)
    {

        try
        {
            UpdateVisuals();
        }
        catch
        {
            Debug.Log(name + "was unable to update visual shit");
        }
        currentHealth = value;
        if (currentHealth <= baseHealth / 2 && !hatIsOff)
        {
            hatController.DestroyHat();
            hatIsOff = true;
        }
    }

    public void ChangeHealth(float value, Vector3 pos)
    {
        enemyAnimator.SetTrigger("Hit");
        currentHealth += value;
        if (currentHealth <= 0)
        {
            Death();
        }
        else UpdateVisuals();

        if (!hasDetected)
        {
            state = StateMachine.Detection;
        }

    }

    void UpdateVisuals()
    {
        Debug.Log(name + " is updating visuals with health: " + currentHealth);
        crackMaterial.SetFloat("_HealthRatio", currentHealth / baseHealth);
        if (currentHealth <= baseHealth / 2 && !hatIsOff)
        {
            hatIsOff = true;
            hatController.HatOff();
        }
    }

    [System.Serializable]
    public struct MovementType
    {
        public EnemyMatryohska.StateMachine targetState;
        public float timeBetweenJump, jumpForceUp, jumpForceHorizontal, torqueForce;
    }

}
