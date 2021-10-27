using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement_ : MonoBehaviour
{
    [SerializeField] public GameObject lookDirection;
    private float currentMoveSpeed;
    [SerializeField] float walkSpeed = 2f, sprintSpeed = 4f;
    [SerializeField] [Range(0.01f, 0.1f)] float rotationSpeed;
    [SerializeField] LayerMask rayHitLayer;
    [SerializeField] KeyCode sprintButton = KeyCode.LeftShift;
    [SerializeField] public Animator playerAnimator;
    FMODUnity.StudioEventEmitter hover;
    PlayerHealth_ playerHealth_;
    bool isWalking, isIdle, isSprint, isMoving;
    Vector3 forward, right;
    bool canMove = true;
    Vector3 lookAtPosition;

    [SerializeField] float fireRate = 1f;
    float nextFire;
    private void Start()
    {
        playerHealth_ = GetComponent<PlayerHealth_>();
        hover = GetComponent<FMODUnity.StudioEventEmitter>();
    }
    void Update()
    {
        
        if (playerHealth_.IsAlive())
        {

            canMove = !playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Shoot");
            if (canMove)
            {
                WalkOrSprint();
                PlayerMove();
            }
            else
            {
                currentMoveSpeed = 0;
                transform.LookAt(lookAtPosition);
            }

            playerAnimator.SetBool("Walk", isWalking);
            playerAnimator.SetBool("Sprint", isSprint);
            playerAnimator.SetBool("Idle", isIdle);
            //WalkOrSprint();
            PLayerLook();
        }
    }

    private void PlayerMove()
    {
        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;

        Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        if (direction.magnitude > 0.1f)
        {
            Vector3 rightMovement = right * currentMoveSpeed * Time.deltaTime * Input.GetAxisRaw("Horizontal");
            Vector3 upMovement = forward * currentMoveSpeed * Time.deltaTime * Input.GetAxisRaw("Vertical");

            Vector3 heading = Vector3.Normalize(rightMovement + upMovement);
            transform.forward = Vector3.Lerp(transform.forward, heading, rotationSpeed);
            transform.position += heading * currentMoveSpeed * Time.deltaTime;
        }
    }

    public void SetSpeed(float Speed)
    {
        walkSpeed = Speed;
        sprintSpeed = Speed * 1.75f;
    }
    private void WalkOrSprint()
    {


        isMoving = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;


        if (isMoving && !Input.GetKey(sprintButton))
        {
            hover.SetParameter("onOFF", 0f);
            isWalking = true;

            isSprint = false;
            isIdle = false;
            currentMoveSpeed = walkSpeed;
        }
        else if (isMoving && Input.GetKey(sprintButton))
        {
            hover.SetParameter("onOFF", 1f);
            isSprint = true;

            isWalking = false;
            isIdle = false;
            currentMoveSpeed = sprintSpeed;
        }
        else
        {
            isIdle = true;

            isWalking = false;
            isSprint = false;
        }
    }
    private void PLayerLook()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit cameraRayHit;
        if (Physics.Raycast(cameraRay, out cameraRayHit, rayHitLayer))
        {

            Vector3 targetPosition = new Vector3(cameraRayHit.point.x, transform.position.y, cameraRayHit.point.z);
            lookDirection.transform.LookAt(targetPosition);
            // lookDirection.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(targetPosition.x, targetPosition.y, targetPosition.z)), Time.deltaTime *1);
            if (Input.GetMouseButtonDown(0) && Time.time > nextFire)
            {
                lookAtPosition = targetPosition;
                transform.LookAt(targetPosition);
                GetComponent<TestShoot_>().Fire(this.transform);
                nextFire = Time.time + fireRate;
            }
        }
    }

    public bool IsSprint()
    {
        return isSprint;
    }
}
