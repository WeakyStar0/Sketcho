using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnumFlagsAttribute : PropertyAttribute { }

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    [SerializeField] public Animator Animator;
    bool isFacingRight = true;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    private float currentMoveSpeed;
    float horizontalMovement;
    bool isRunning = false;

    [Header("Dashing")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.1f;
    bool isDashing;
    bool canDash = true;
    TrailRenderer trailRenderer;

    [Header("Jumping")]
    public float jumpPower = 10f;
    public int maxJumps = 2;
    private int jumpsRemaining;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.49f, 0.03f);
    [EnumFlags] public LayerMask groundLayer;
    bool isGrounded;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallGravityMult = 2f;

    [Header("WallCheck")]
    public Transform wallCheckPos;
    public Vector2 wallCheckSize = new Vector2(0.49f, 0.03f);
    [EnumFlags] public LayerMask wallLayer;

    [Header("WallSlide")]
    public float wallSlideSpeed = 2f;
    bool isWallSliding;

    //wall jumping here
    bool isWallJumping;
    float wallJumpDirection;
    float wallJumpTime = 0.5f;
    float wallJumpTimer;
    public Vector2 wallJumpPower = new Vector2(5f, 10f);

    private void Start()
    {
        currentMoveSpeed = moveSpeed; // Initialize with walk speed
        trailRenderer = GetComponent<TrailRenderer>();
    }

    [System.Obsolete]
    void Update()
    {

        Animator.SetFloat("yVelocity", rb.velocity.y);
        Animator.SetFloat("magnitude", rb.velocity.magnitude);
        Animator.SetBool("isWallSliding", isWallSliding);
        Animator.SetBool("isRunning", isRunning); // Set running animation parameter

        if (isDashing)
        {
            return;
        }

        GroundCheck();
        ProccessGravity();
        ProcessWallSlide();
        ProcessWallJump();

        if (!isWallJumping)
        {
            rb.linearVelocity = new Vector2(horizontalMovement * currentMoveSpeed, rb.linearVelocity.y);
            Flip();
        }


    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }


    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    private IEnumerator DashCoroutine()
    {
        Physics2D.IgnoreLayerCollision(31, 30, true); //ignore collision with player and enemy

        canDash = false;
        isDashing = true;

        trailRenderer.emitting = true;
        float dashDirection = isFacingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, rb.linearVelocity.y); //dash movement
        yield return new WaitForSeconds(dashDuration); //wait for dash duration
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y); //stop dash movement

        isDashing = false;
        trailRenderer.emitting = false;
        Physics2D.IgnoreLayerCollision(31, 30, false); //ignore collision with player and enemy

        yield return new WaitForSeconds(dashCooldown); //wait for dash cooldown
        canDash = true; //reset dash
    }

    // New method for handling run input
    public void Run(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isRunning = true;
            currentMoveSpeed = runSpeed;
        }
        else if (context.canceled)
        {
            isRunning = false;
            currentMoveSpeed = moveSpeed;
        }
    }

    [System.Obsolete]
    public void Jump(InputAction.CallbackContext context)
    {
        if (jumpsRemaining > 0)
        {
            if (context.performed)
            {
                //Hold down jump button = full height
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                jumpsRemaining--;
                Animator.SetTrigger("jump");
            }
            else if (context.canceled && rb.linearVelocity.y > 0)
            {
                //Light tap of jump button = half the height
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
                jumpsRemaining--;
                Animator.SetTrigger("jump");
            }
        }

        //wall jumping here
        if (context.performed && wallJumpTimer > 0f)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y); //saltar da parede
            wallJumpTimer = 0f; //reset timer

            //Virar player
            if (transform.localScale.x != wallJumpDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 theScale = transform.localScale;
                theScale.x *= -1;
                transform.localScale = theScale;
            }

            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f); //cancel wall jump after a set time
        }
    }

    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer)) //checks if set box overlaps with ground
        {
            jumpsRemaining = maxJumps;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);
    }

    private void ProccessGravity()
    {
        //falling gravity
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallGravityMult; //fall faster and faster
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed)); //max fall speed
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    private void ProcessWallSlide()
    {
        if (!isGrounded & WallCheck() & horizontalMovement != 0)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed)); //max fall speed
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void ProcessWallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = -transform.localScale.x; //direção oposta da parede
            wallJumpTimer = wallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        }
        else if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
        }
    }

    private void CancelWallJump()
    {
        isWallJumping = false;
    }

    private void Flip()
    {
        if (isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }

    private void OnDrawGizmosSelected()
    {
        //Ground check visual
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
    }
}