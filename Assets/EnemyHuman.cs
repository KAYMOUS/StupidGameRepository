using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyHuman : MonoBehaviour
{
    public GameObject player;
    private bool shouldMove;

    private Rigidbody2D rb;
    private Animator anim;
    private BoxCollider2D Bcollider;
    private CircleCollider2D cc;

    public GameObject parentObjectHuman;

    private float dirX;
    [SerializeField] private float slapForce = 10f;
    [SerializeField] private float moveSpeed = 18f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private LayerMask jumbeleGround;
    private enum MovementState { idle, run, jump, falling, slap, dash }
    MovementState state;


    //Get components
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        Bcollider = GetComponent<BoxCollider2D>();
    }

    // Input + Movement
    void Update()
    {
        FollowPlayer();
        updateAimtaionUpdate();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Human")
        {
            state = MovementState.slap;
            anim.SetInteger("state", (int)state);
            updateAimtaionUpdate();
        }
    }

    //Animation
    private void updateAimtaionUpdate()
    {
        //Move anim
        if (rb.velocity.x > 0f)
        {
            state = MovementState.run;
            FlipCharacter(false); // face right
        }
        else if (rb.velocity.x < 0f)

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
        anim.SetInteger("state", (int)state);
    }

    //AI
    private void FollowPlayer()
    {
        Vector2 currentPosition = rb.transform.position;
        Vector2 targetPosition = player.transform.position;

        // Move only on the X-axis
        float xPosition = Mathf.MoveTowards(currentPosition.x, targetPosition.x, moveSpeed * Time.deltaTime);
        Vector2 newPosition = new Vector2(xPosition, currentPosition.y);

        // Perform ground detection
        shouldMove = IsGrounded();

        if (shouldMove)
        {
            rb.MovePosition(newPosition);
        }
    }

    //Is Grounded
    private bool IsGrounded()
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

