using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float Speed;
    [SerializeField] private int maxJumpCount;      
    [SerializeField] private float jumpHeight;
    [SerializeField] private LayerMask groundLayer; // Detecci� de col�lisi� amb ground 
    [SerializeField] TrailRendererController tr;

    private bool canDash = true;
    public bool CanMove;
    private BoxCollider2D boxCollider; 
    private static Rigidbody2D body;
    private Animator animator;
    private int jumpCount;
    private bool canJump;
    public bool IsJumping { get; private set; }

    public bool ApplyingInput { get; private set; }              // Decideix si l'sprite ha de fer flip en eix x o y (si est� mirant dreta o esq)
    private PhysicsMaterial2D defaultMaterial, noFrictionMaterial; /* Material default i material sense
                                                                       fricci� (no es queda enganxat a les parets) */

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        jumpCount = 0;

        boxCollider = GetComponent<BoxCollider2D>();
        defaultMaterial = GetComponent<PhysicsMaterial2D>();
        noFrictionMaterial = new PhysicsMaterial2D();
        noFrictionMaterial.friction = 0;

        CanMove = true;
        canJump = true;
    }
    private void Update()
    {
      
        Movement();
        
        Jump();

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !GetComponent<Attack_Controller>().isAttacking)
        {
            GetComponent<Dash>().TryStartDash(1, 30, true);
        }
    }
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canJump && !GetComponent<Attack_Controller>().isAttacking)
        {
            jumpCount++;
            body.velocity = new Vector2(body.velocity.x, jumpHeight * 2);
        }
        canJump = CanJump();
        animator.SetBool("canJump", IsJumping);
    }
    private bool CanJump()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);

        if (hit.collider != null) // Si NO est� a l'aire
        {
            body.GetComponent<Rigidbody2D>().sharedMaterial = defaultMaterial;
            jumpCount = 0;
            IsJumping = false;

            return true;
        }
        else                      // Si EST� a l'aire
        {
            body.GetComponent<Rigidbody2D>().sharedMaterial = noFrictionMaterial;
            IsJumping = true;

            if (jumpCount < maxJumpCount - 1) { return true; }
            else { return false; }
        }
    }

    public void Movement()
    {
        if (!CanMove) return;  
        if (GetComponent<Dash>().IsDashing) return; 

        float horizontalInput = Input.GetAxis("Horizontal");

        if (!GetComponent<Attack_Controller>().isAttacking)
        {
            body.velocity = new Vector2(horizontalInput * Speed, body.velocity.y);
            if (horizontalInput < 0) { body.GetComponent<SpriteRenderer>().flipX = false; }
            else if (horizontalInput > 0) { body.GetComponent<SpriteRenderer>().flipX = true; }
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) && !GetComponent<Attack_Controller>().isAttacking) 
        { ApplyingInput = true; }
        else
        {
            ApplyingInput = false;
            if (body.velocity.x != 0 && !IsJumping)
            {
                body.velocity = new Vector2(body.velocity.x * 0.1f, body.velocity.y);
            }
        }

        animator.SetBool("run", body.velocity.x != 0 && ApplyingInput);
    }

  
}

