using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fox : MonoBehaviour
{
    // references
    Rigidbody2D rb;
    Animator anim;

    // ground check
    [SerializeField] bool isGrounded = false; 
    [SerializeField] Transform groundCheckCollider;
    [SerializeField] LayerMask groundLayer;

    // moving variables
    float horizontalValue = 0;
    int scaleConstant = 4;

    [SerializeField] float speed = 3f;
    int speedConstant = 50;
    float speedMultiplier = 2.0f;
    bool isRunning = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalValue = Input.GetAxisRaw("Horizontal");
        isRunning = Input.GetKey(KeyCode.LeftShift);
    }

    void FixedUpdate()
    {
        GroundCheck();
        Move(horizontalValue);
    }

    void GroundCheck()
    {
        isGrounded = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckCollider.position, 0.2f, groundLayer);

        if (colliders.Length > 0) {
            isGrounded = true;
        }
    }

    void Move(float direction) {
        float xVal = direction * speed * speedConstant * Time.fixedDeltaTime;
        xVal *= isRunning ? speedMultiplier : 1;

        rb.velocity = new Vector2(xVal, rb.velocity.y);

        // flip the sprite with transform.localScale
        if (direction != 0) {
            int scaleValue = (direction > 0) ? 1 : -1;
            transform.localScale = new Vector3(scaleValue, 1, 1) * scaleConstant;
        }
        
        anim.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
    }
}
