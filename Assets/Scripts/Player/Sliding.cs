using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerObj;
    private Rigidbody rb;
    private PlayerMovement pm;
    public CapsuleCollider playerCollider;
    public Animator animator;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    private float slideTimer;
    public float crouchHeight = 0.721594f;
    public float crouchRadius = 0.360797f;

    private float startHeight;
    private float startRadius;

    [Header("Input")]
    public KeyCode slideKey = KeyCode.LeftControl;
    private float horizontalInput;
    private float verticalInput;

    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
        if (playerCollider == null)
        {
            playerCollider = GetComponentInChildren<CapsuleCollider>();
        }

        startHeight = 1.603061f;
        startRadius = 0.360797f;
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(slideKey) && (horizontalInput != 0 || verticalInput != 0))
            StartSlide();
        if(Input.GetKeyUp(slideKey) && pm.sliding)
            StopSlide();
    }

    void FixedUpdate()
    {
        if (pm.sliding)
            SlidingMovement();
    }

    private void StartSlide()
    {
        pm.sliding = true;
        animator.SetBool("Slide", true);
        animator.SetBool("Idle", false);
        animator.SetBool("Run", false);
        animator.SetBool("Sprint", false);
        animator.SetBool("Jump", false);

        playerCollider.height = crouchHeight;
        playerCollider.radius = crouchRadius;
        playerCollider.center = new Vector3(playerCollider.center.x, crouchHeight / 2f, playerCollider.center.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        slideTimer = maxSlideTime;
    }

    private void SlidingMovement()
    {
        animator.SetBool("Slide", true);
        animator.SetBool("Idle", false);
        animator.SetBool("Run", false);
        animator.SetBool("Sprint", false);
        animator.SetBool("Jump", false);

        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (!pm.OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }
        else
        {
            rb.AddForce(pm.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
        }
        
        if (slideTimer <= 0)
            StopSlide();

    }

    private void StopSlide()
    {
        pm.sliding = false;

        playerCollider.height = startHeight;
        playerCollider.radius = startRadius;

        playerCollider.center = new Vector3(playerCollider.center.x, startHeight / 2.1f, playerCollider.center.z);
    }
}
