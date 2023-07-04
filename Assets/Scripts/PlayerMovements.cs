using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private BoxCollider2D Bcollider;

    public GameObject parentObjectHuman;

    private float dirX;
    [SerializeField] private float slapForce = 10f;
    [SerializeField] private float moveSpeed = 18f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private LayerMask jumbeleGround;
    private enum MovementState { idle, run, jump, falling, slap, dash }

    //Get components
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        Bcollider = GetComponent<BoxCollider2D>();
    }

    // Input + Movement
    void Update()
    {
        dirX = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y); //Move Player

        if (Input.GetButtonDown("Jump") && isGrounded()) //Jump
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if (Input.GetKeyDown(KeyCode.F)) //Slap
        {
            slapPush();
        }

        updateAimtaionUpdate();
    }

    //Animation
    private void updateAimtaionUpdate()
    {
        MovementState state;

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
            state = MovementState.idle;
        }

        //Jump anim
        if (rb.velocity.y > .1f)
        {
            state = MovementState.jump;
        }
        else if (rb.velocity.y < -.1f)
        {
            state = MovementState.falling;
        }

        //Slap anime
        if (Input.GetKey(KeyCode.F)) //Slap
        {
            state = MovementState.slap;
        }

        anim.SetInteger("state", (int)state);
    }

    //Is Grounded
    private bool isGrounded()
    {
        bool isGrounded = Physics2D.BoxCast(Bcollider.bounds.center, Bcollider.bounds.size, 0f, Vector2.down, .1f, jumbeleGround);
        return isGrounded;
    }

    //Parent Flipper
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

    //Push
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

}
