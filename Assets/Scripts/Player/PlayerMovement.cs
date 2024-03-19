using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public CapsuleCollider playerCollider;
    public GameObject particles1;
    public GameObject particles2;
    public GameObject particles3;

    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float wallrunSpeed;
    public float slideSpeed;
    public float dashSpeed;

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    public float speedIncreaseMultipler;
    public float slopeIncreaseMultipler;
    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    private bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchHeight = 0.721594f;
    public float crouchRadius = 0.360797f;
    private float startHeight;
    private float startRadius;
  

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    public Transform orientation;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;

    private Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        idle,
        walking,
        sprinting,
        wallrunning,
        crouching,
        dashing,
        sliding,
        air
    }

    public bool sliding;
    public bool wallrunning;
    public bool dashing;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (playerCollider == null)
        {
            playerCollider = GetComponentInChildren<CapsuleCollider>();
        }
        rb.freezeRotation = true;
        readyToJump = true;
        startHeight = 1.596363f;
        startRadius = 0.360797f;
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        MyInput();
        SpeedControl();
        StateHandler();

        
        if(sliding)
        {
            animator.SetBool("Slide", true);
            animator.SetBool("Idle", false);
            animator.SetBool("Run", false);
            animator.SetBool("Sprint", false);
            animator.SetBool("Jump", false);
            animator.SetBool("Dash", false);

        }

        if (dashing)
        {
            animator.SetBool("Slide", false);
            animator.SetBool("Idle", false);
            animator.SetBool("Run", false);
            animator.SetBool("Sprint", false);
            animator.SetBool("Jump", false);
            particles3.SetActive(true);
            particles1.SetActive(false);
            particles2.SetActive(false);

        }
        if (grounded)
        {
            readyToJump = true;
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //when to jump
        if (Input.GetKeyDown(jumpKey) && readyToJump && grounded)
        {
            
            readyToJump = true;
           
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // start crouch
        if(Input.GetKeyDown(crouchKey) && grounded)
        {
            animator.SetBool("Crouch", true);
            animator.SetBool("Slide", false);
            animator.SetBool("Jump", false);
            
            playerCollider.height = crouchHeight;
            playerCollider.radius = crouchRadius;

            playerCollider.center = new Vector3(playerCollider.center.x, crouchHeight / 2f, playerCollider.center.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        
        //stop crouch
        if(Input.GetKeyUp(crouchKey))
        {
            animator.SetBool("Crouch", false);
            playerCollider.height = startHeight;
            playerCollider.radius = startRadius;

            playerCollider.center = new Vector3(playerCollider.center.x, startHeight / 2.1f, playerCollider.center.z);
        }
    }

    private void StateHandler()
    {
        //Mode-Idle

        if (grounded && rb.velocity.magnitude <= 0.2f)
        {
            state = MovementState.idle;
            desiredMoveSpeed = walkSpeed;
            animator.SetBool("Run", false);
            animator.SetBool("Sprint", false);
            animator.SetBool("Idle", true);
            animator.SetBool("Jump", false);
            animator.SetBool("Slide", false);
            particles1.SetActive(false);
            particles2.SetActive(false);



        }
        // Mode- Wallrunning
        if (wallrunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallrunSpeed;
            animator.SetBool("Run", true);
            animator.SetBool("Sprint", false);
            animator.SetBool("Idle", false);
            animator.SetBool("Jump", false);
            animator.SetBool("Slide", false);
            particles2.SetActive(false);
            particles1.SetActive(true);
        }
        if (sliding)
        {
            state = MovementState.sliding;

            if (OnSlope() && rb.velocity.y < 0.1f)
                desiredMoveSpeed = slideSpeed;

            else
                desiredMoveSpeed = sprintSpeed;
        }

        // Mode - Crouching
        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }
        // Mode - Sprinting
        if(grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
            animator.SetBool("Sprint", true);
            animator.SetBool("Run", false);
            animator.SetBool("Idle", false);
            animator.SetBool("Jump", false);
            animator.SetBool("Slide", false);
            particles2.SetActive(true);
            particles1.SetActive(false);



        }

        //Mode - Walking
        else if (grounded && rb.velocity.magnitude > 0.1f)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
            animator.SetBool("Run", true);
            animator.SetBool("Sprint", false);
            animator.SetBool("Idle", false);
            animator.SetBool("Jump", false);
            animator.SetBool("Slide", false);
            particles2.SetActive(false);
            particles1.SetActive(true);



        }
        //Mode - Air
        if(grounded == false)
        {   
            state = MovementState.air;
            animator.SetBool("Jump", true);
            animator.SetBool("Run", false);
            animator.SetBool("Sprint", false);
            animator.SetBool("Idle", false);
            animator.SetBool("Slide", false);
            particles1.SetActive(false);
            particles2.SetActive(false);


        }

        if(Mathf.Abs(lastDesiredMoveSpeed - lastDesiredMoveSpeed) > 4f && moveSpeed != 0)
        {
            
            StopAllCoroutines();
            StartCoroutine(SmoothyLerpMoveSpeed());
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;

    }

    private IEnumerator SmoothyLerpMoveSpeed()
    {
        //smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while(time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if(OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultipler * slopeIncreaseMultipler * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * speedIncreaseMultipler;
            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private void MovePlayer()
    {
        //calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);
            rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
        //on ground
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        //in air
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
            
        }
        // turn gravity off while on slope
        if(!wallrunning) rb.useGravity = !OnSlope();

        
    }

    private void SpeedControl()
    {

        // limit velocity if needed
        if (OnSlope() && !exitingSlope)
        {
            if(rb.velocity.magnitude >moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;

        }

        // limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            //limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);

            }
        }
    }

    private void Jump()
    {
       
        exitingSlope = true;
        //Reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    public bool OnSlope()
    {
        if(Physics.Raycast(transform.position,Vector3.down, out slopeHit,playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }
    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {

        return Vector3.ProjectOnPlane(direction,slopeHit.normal).normalized;
    }
}