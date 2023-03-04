using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fox : MonoBehaviour
{
    // references
    Rigidbody2D rb;
    Animator anim;

    // ground check
    bool isGrounded = false; 
    [SerializeField] Transform groundCheckCollider;
    [SerializeField] float groundCheckRadius = 0.1f;
    [SerializeField] LayerMask groundLayer;

    // jumping
    float jumpPower = 30.0f;
    bool jumpStart = false;
    
    // crouching
    [SerializeField] Collider2D standingCollider;
    [SerializeField] Transform overheadCheckCollider;
    [SerializeField] float overheadCheckRadius = 0.1f;
    [SerializeField] bool isCrouching = false;
    float crouchSpeedMultiplier = 0.5f;

    // moving
    float horizontalInput = 0;
    int scaleConstant = 4;

    float speed = 1f;
    int speedConstant = 50;
    float speedMultiplier = 1.75f;
    bool isRunning = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        isRunning = Input.GetKey(KeyCode.RightShift);

        // jump
        if (Input.GetButtonDown("Jump")) {
            jumpStart = true;
        } else if (Input.GetButtonUp("Jump")) {
            jumpStart = false;
        }

        // crouch
        if (Input.GetButtonDown("Crouch")) {
            isCrouching = true;
        } else if (Input.GetButtonUp("Crouch")) {
            isCrouching = false;
        }
    }

    void FixedUpdate()
    {
        GroundCheck();
        Move(horizontalInput, jumpStart, isCrouching);
    }

    void GroundCheck()
    {
        isGrounded = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            groundCheckCollider.position,
            groundCheckRadius,
            groundLayer
        );

        if (colliders.Length > 0) {
            isGrounded = true;
        }
    }

    void Move(float direction, bool JUMP, bool CROUCHING) {

        #region Jump & Crouch

        // check overhead before stand up
        if (!isCrouching) {
            if (Physics2D.OverlapCircle(overheadCheckCollider.position, overheadCheckRadius, groundLayer)) {
                CROUCHING = true;
            }
        }

        if (isGrounded) {

            standingCollider.enabled = !CROUCHING;
            
            if (JUMP) {
                // clean up variables of interest
                isGrounded = false;
                jumpStart = false;

                rb.AddForce(new Vector2(0f, jumpPower), ForceMode2D.Impulse);
            }
        }

        anim.SetBool("isCrouching", CROUCHING);
        #endregion

        #region Move and Run
        float xVal = direction * speed * speedConstant * Time.fixedDeltaTime;

        // generates final multiplier based on crouching/running state
        float crouchingMultiplier = CROUCHING ? crouchSpeedMultiplier : 1;
        float runningMultiplier = isRunning && !CROUCHING ? speedMultiplier : 1;
        float movementMultiplier = crouchingMultiplier * runningMultiplier;

        xVal *= movementMultiplier;

        rb.AddForce(new Vector2(xVal, 0f), ForceMode2D.Impulse);

        // flip the sprite with transform.localScale
        if (direction != 0) {
            int scaleValue = (direction > 0) ? 1 : -1;
            transform.localScale = new Vector3(scaleValue, 1, 1) * scaleConstant;
        }
        
        anim.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
        #endregion
    }

    
}
