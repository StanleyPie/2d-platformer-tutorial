using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.VirtualTexturing;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    bool isFacingRIght = true;
    public Animator animator;
    public ParticleSystem smokeFX;
    public BoxCollider2D playerCollider;

    [Header("Movement")]
    public float moveSpeed= 5f;
    float horizontalMovement;
    [Range(-1, 1)] public float heightvalue;

    [Header("Dashing")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.1f;
    public float dashCooldown = 0.1f;
    bool isDashing;
    bool canDash = true;
    TrailRenderer trailRenderer;

    [Header("Jump")]
    public float jumpPower = 10f;
    public int jumpMax = 2;
    public int jumpRemain;

    [Header("Wall Sliding")]
    public float wallSlideSpeed = 2f;
    public bool isWallSliding;

    [Header("Wall Jumping")]
    public bool isWallJumping;
    float wallJumpDirection;
    public float wallJumpTime = 0.5f;
    public float wallJumpTimer;
    public Vector2 wallJumpPower = new Vector2(5f, 10f);

    [Header("Ground Check")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;
    public bool isGrounded;
    bool isOnPlatform;

    [Header("Wall Check")]
    public Transform wallCheckPos;
    public Vector2 wallCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask wallLayer;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMutiplier = 2f;


    private void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }
    private void Update()
    {
        animator.SetFloat("yVelocity", rb.velocity.y);
        animator.SetFloat("magnitude", rb.velocity.magnitude);
        animator.SetBool("isWallSliding", isWallSliding);

        if (isDashing) return;
        GroundCheck();
        Gravity();
        ProcessWallSliding();
        ProcessWallJumping();

        if (!isWallJumping)
        {
            rb.velocity = new Vector2(horizontalMovement * moveSpeed, rb.velocity.y);
            Flip();
        }
    }

    private void Gravity()
    {
        if(rb.velocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMutiplier;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
            
        }
        else
        {
            rb.gravityScale = baseGravity;
        }

    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Drop(InputAction.CallbackContext context)
    {
        if(context.performed && isGrounded && isOnPlatform && playerCollider.enabled)
        {
            StartCoroutine(this.DisablePlayerCollider(0.25f));
        }
    }

    IEnumerator DisablePlayerCollider(float disableTime)
    {
        playerCollider.enabled = false;
        yield return new WaitForSeconds(disableTime);
        playerCollider.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            isOnPlatform = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            isOnPlatform = false;
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    IEnumerator DashCoroutine()
    {
        Physics2D.IgnoreLayerCollision(8, 9, true);

        canDash = false;
        isDashing = true;

        trailRenderer.emitting = true;
        float direction = isFacingRIght ? 1 : -1;
        rb.velocity = new Vector2(direction * dashSpeed, rb.velocity.y);

        yield return new WaitForSeconds(dashDuration);

        rb.velocity = new Vector2(0f, rb.velocity.y);

        isDashing = false;
        trailRenderer.emitting = false;
        Physics2D.IgnoreLayerCollision(8, 9, false);


        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        //Debug.Log(context.phase);
        if (jumpRemain > 0)
        {
            if (context.performed)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpPower);
                jumpRemain--;
                JumpFX();
            }

            else if (context.canceled && rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * this.heightvalue);
                jumpRemain--;
                JumpFX();
            }
        }

        if (context.performed && wallJumpTimer > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            wallJumpTimer = 0f;
            jumpRemain += 2;
            JumpFX();



            if (transform.localScale.x != wallJumpDirection)
            {
                isFacingRIght = !isFacingRIght;
                Vector3 ls = transform.localScale;
                ls.x *= -1f;
                transform.localScale = ls;
            }

            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f);
        }
    }

    private void JumpFX()
    {
        animator.SetTrigger("jump");
        smokeFX.Play();
    }

    private void Flip()
    {
        if (isFacingRIght && horizontalMovement < 0 || !isFacingRIght && horizontalMovement > 0)
        {
            isFacingRIght = !isFacingRIght;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;

            if (rb.velocity.y == 0f)
            {
                smokeFX.Play();
            }
        }
    }

    private void ProcessWallSliding()
    {
        if (!isGrounded & WallCheck() & horizontalMovement != 0)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -wallSlideSpeed));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void ProcessWallJumping()
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

    protected void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPos. position, groundCheckSize,0, groundLayer))
        {
            jumpRemain = jumpMax;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    protected bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
    }
}
