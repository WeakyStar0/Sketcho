using System.Collections;
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
    float speedMultiplier = 1f;
    bool isRunning = false;

    [Header("Forced Movement")]
    public bool inWalkBound = false;
    public float forcedMoveSpeed = 10f;
    private bool originalFacingRight;

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

    // Wall jumping
    bool isWallJumping;
    float wallJumpDirection;
    float wallJumpTime = 0.5f;
    float wallJumpTimer;
    public Vector2 wallJumpPower = new Vector2(5f, 10f);

    private void Start()
    {
        SpeedItem.OnSpeedItemCollected += StartSpeedBoost;
        currentMoveSpeed = moveSpeed;
        trailRenderer = GetComponent<TrailRenderer>();
    }

    void StartSpeedBoost(float multiplier)
    {
        StartCoroutine(SpeedBoostCoroutine(multiplier));
    }
    
    private IEnumerator SpeedBoostCoroutine(float multiplier)
    {
        speedMultiplier = multiplier;
        yield return new WaitForSeconds(2f);
        speedMultiplier = 1f;
    }


    [System.Obsolete]
    void Update()
    {
        Animator.SetFloat("yVelocity", rb.velocity.y);
        Animator.SetFloat("magnitude", rb.velocity.magnitude);
        Animator.SetBool("isWallSliding", isWallSliding);
        Animator.SetBool("isRunning", isRunning);

        if (isDashing) return;

        GroundCheck();
        ProccessGravity();
        ProcessWallSlide();
        ProcessWallJump();

        if (!isWallJumping)
        {
            // Forced right movement in walkBound
            if (inWalkBound)
            {
                horizontalMovement = 1f; // Always move right
            }
            
            rb.linearVelocity = new Vector2(horizontalMovement * currentMoveSpeed * speedMultiplier, rb.linearVelocity.y);
            Flip();
        }
    }

    public void EnterWalkBound()
    {
        inWalkBound = true;
        originalFacingRight = isFacingRight;
        
        // Force facing right
        if (!isFacingRight)
        {
            Flip();
        }
        
        currentMoveSpeed = forcedMoveSpeed;
        isRunning = true;
    }

    public void ExitWalkBound()
    {
        inWalkBound = false;
        
        // Restore original facing if needed
        if (originalFacingRight != isFacingRight)
        {
            Flip();
        }
        
        currentMoveSpeed = moveSpeed;
        isRunning = false;
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (!inWalkBound) // Only process input when not in walkBound
        {
            horizontalMovement = context.ReadValue<Vector2>().x;
        }
    }

    public void Run(InputAction.CallbackContext context)
    {
        if (!inWalkBound) // Only process input when not in walkBound
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
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash && !inWalkBound) // Can't dash in walkBound
        {
            StartCoroutine(DashCoroutine());
        }
    }

    private IEnumerator DashCoroutine()
    {
        Physics2D.IgnoreLayerCollision(31, 30, true);

        canDash = false;
        isDashing = true;

        trailRenderer.emitting = true;
        float dashDirection = isFacingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, rb.linearVelocity.y);
        yield return new WaitForSeconds(dashDuration);
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        isDashing = false;
        trailRenderer.emitting = false;
        Physics2D.IgnoreLayerCollision(31, 30, false);

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    [System.Obsolete]
    public void Jump(InputAction.CallbackContext context)
    {
        if (jumpsRemaining > 0)
        {
            if (context.performed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                jumpsRemaining--;
                Animator.SetTrigger("jump");
            }
            else if (context.canceled && rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
                jumpsRemaining--;
                Animator.SetTrigger("jump");
            }
        }

        if (context.performed && wallJumpTimer > 0f)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            wallJumpTimer = 0f;

            if (transform.localScale.x != wallJumpDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 theScale = transform.localScale;
                theScale.x *= -1;
                transform.localScale = theScale;
            }

            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f);
        }
    }

    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))
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
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallGravityMult;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
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
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed));
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
            wallJumpDirection = -transform.localScale.x;
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
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
    }
}