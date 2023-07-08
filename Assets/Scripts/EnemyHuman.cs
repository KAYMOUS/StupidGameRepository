using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyHuman : MonoBehaviour
{
    public GameObject player;

    private Rigidbody2D rb;
    private BoxCollider2D bc;

    [SerializeField] private float moveSpeed = 18f;

    //Get components
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Input + Movement
    void Update()
    {
        FollowPlayer();
    }

    //AI
    private void FollowPlayer()
    {
        Vector2 currentPosition = rb.position;
        Vector2 targetPosition = player.transform.position;
        Vector2 newPosition = Vector2.MoveTowards(currentPosition, targetPosition, moveSpeed * Time.deltaTime);
        rb.MovePosition(newPosition);
    }

}

