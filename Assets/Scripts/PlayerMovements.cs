using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
    public Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private BoxCollider2D Bcollider;

    public GameObject parentObjectHuman;

    private Respawn respawn;
    private Spawner spawner;

    private float dirX;

    [SerializeField] private float slapForce = 10f;
    [SerializeField] private float moveSpeed = 18f;
    [SerializeField] private float jumpForce = 15f;
    private int airJump = 1;
    [SerializeField] private int airJumpCount = 1;
    [SerializeField] private float dashForce = 50f;
    [SerializeField] private float dashDuration = 0.15f;
    private bool isDashing = false;
    [SerializeField] private LayerMask jumbeleGround;

    //Throwables
    public GameObject heldObject;
    public Transform heldObjectPosition;
    public float throwingForce = 10f;

    public bool isAlive;
    private enum MovementState { idle, run, jump, falling, slap, 
                                death, dash, dance1 }
    MovementState state;

    //------------------------------------------------------------------------------
    //Get components
    //------------------------------------------------------------------------------
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        Bcollider = GetComponent<BoxCollider2D>();

        isAlive = true;
    }

    //------------------------------------------------------------------------------
    // Input + Movement
    //------------------------------------------------------------------------------
    void Update()
    {
        dirX = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y); //Move Player

        if (isGrounded())
        {
            airJump = airJumpCount;
        }

        if (Input.GetButtonDown("Jump") && isGrounded()) //Jump
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        if (Input.GetButtonDown("Jump") && !isGrounded() && airJump > 0) // Air Jump
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            airJump--;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && Mathf.Abs(dirX) > 0) // Dash
        {
            Dash();
        }

        if (Input.GetKeyDown(KeyCode.F)) //Slap
        {
            slapPush();
        }

        // Other player controls and logic here

        if (Input.GetKeyDown(KeyCode.T))
        {
            ThrowHeldObject();
        }

        updateAimtaionUpdate();
    }

    //------------------------------------------------------------------------------
    // Animation
    //------------------------------------------------------------------------------
    private void updateAimtaionUpdate()
    {

        //Move anim
        if (dirX > 0f)
        {
            state = MovementState.run;
            FlipCharacter(false); // face right
        }
        else if (dirX < 0f)
        {
            state = MovementState.run;
            FlipCharacter(true); // face left
        }
        else
        {
            state = MovementState.idle; // idle
        }

        // Jump anim
        if (rb.velocity.y > .1f)
        {
            state = MovementState.jump; // jump
        }
        else if (rb.velocity.y < -.1f)
        {
            state = MovementState.falling; // fall
        }

        // Slap anime
        if (Input.GetKey(KeyCode.F))
        {
            state = MovementState.slap; // slap
        }

        // death
        if (isAlive == false)
        {
            state = MovementState.death; // death
        }

        //Dance
        if (Input.GetKeyDown(KeyCode.Alpha1) && isGrounded())
        {
            state = MovementState.dance1; // silly dance
        }

        anim.SetInteger("state", (int)state);
    }

    //------------------------------------------------------------------------------
    // Is Grounded
    //------------------------------------------------------------------------------
    private bool isGrounded()
    {
        bool isGrounded = Physics2D.BoxCast(Bcollider.bounds.center, Bcollider.bounds.size, 0f, Vector2.down, .1f, jumbeleGround);
        return isGrounded;
    }

    //------------------------------------------------------------------------------
    // Dash
    //------------------------------------------------------------------------------
    private void Dash()
    {
        if (!isDashing)
        {
            isDashing = true;
            rb.velocity = new Vector2(dashForce * Mathf.Sign(dirX), rb.velocity.y);
            StartCoroutine(DashCooldown());
        }
    }
    //------------------------------------------------------------------------------
    // Dash Cooldown 
    //------------------------------------------------------------------------------
    private IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
    }

    //------------------------------------------------------------------------------
    // Parent Flipper
    //------------------------------------------------------------------------------
    void FlipCharacter(bool flipX)
    {
        Vector3 localScale = parentObjectHuman.transform.localScale;
        localScale.x = flipX ? -Mathf.Abs(localScale.x) : Mathf.Abs(localScale.x);
        parentObjectHuman.transform.localScale = localScale;
    }
    bool IsFacingLeft()
    {
        return parentObjectHuman.transform.localScale.x < 0;
    }

    //------------------------------------------------------------------------------
    // Push
    //------------------------------------------------------------------------------
    void slapPush()
    {
        float pushDistance = 2f;
        Vector2 pushDirection = IsFacingLeft() ? Vector2.left : Vector2.right;
        Vector2 overlapCircleOrigin = (Vector2)transform.position + pushDirection * pushDistance * 0.5f;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(overlapCircleOrigin, pushDistance * 0.5f);

        foreach (Collider2D collider in colliders)
        {
            Rigidbody2D hitRigidbody = collider.GetComponent<Rigidbody2D>();

            if (hitRigidbody != null)
            {
                hitRigidbody.AddForce(pushDirection * slapForce, ForceMode2D.Impulse);
            }
        }
    }

    //------------------------------------------------------------------------------
    // Collecting objects:
    //------------------------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        CollectibleObject collectible = collision.GetComponent<CollectibleObject>();

        if (collectible != null && heldObject == null)
        {
            heldObject = collectible.gameObject;
            heldObject.transform.SetParent(heldObjectPosition);
            heldObject.transform.localPosition = Vector3.zero;
            heldObject.GetComponent<Rigidbody2D>().isKinematic = true;
            heldObject.GetComponent<Collider2D>().enabled = false;
        }
    }

    //------------------------------------------------------------------------------
    // Throwing the held object
    //------------------------------------------------------------------------------
    private void ThrowHeldObject()
    {
        if (heldObject != null)
        {
            heldObject.transform.SetParent(null);
            Rigidbody2D rb = heldObject.GetComponent<Rigidbody2D>();
            rb.isKinematic = false;
            rb.AddForce(transform.right * throwingForce, ForceMode2D.Impulse);

            Collider2D collider = heldObject.GetComponent<Collider2D>();
            collider.enabled = true;

            heldObject = null;
        }
    }

    //------------------------------------------------------------------------------
    // Death & Respawn
    //------------------------------------------------------------------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If the player is already dead, don't execute the code again
        if (!isAlive) return;

        if (collision.gameObject.CompareTag("EnemyTag"))
        {
            // Handle player death and respawn
            Debug.Log("EnemyTag Collided");

            isAlive = false;
            rb.bodyType = RigidbodyType2D.Static;
            spawner.DestroyEnemy();
            updateAimtaionUpdate();
        }
    }
}
