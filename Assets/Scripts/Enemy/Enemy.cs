using System;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Searching and attack")]
    public float SearchingRadius = 5f;
    public float AttackRadiusDelta = 1.5f;
    public float chaseRange = 10f;
    public float attackRange = 2f;
    public float attackDamage = 5f;

    private IEnemyState currentState;


    [Header("Moving")]
    public float moveSpeed = 3f;
    public float patrolTime = 2f;
    [Header("Idle")]
    public float idleTime = 2f;
    
    private bool facingRight = true;

    [NonSerialized] public bool IsPlayerInSearchRadius = false;
    [NonSerialized] public bool IsPlayerInAttackRadius = false;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        SwitchState(new PatrolState());
    }

    void Update()
    {
        currentState.Update();

        
        if(rb.linearVelocityX > 0f && !facingRight || rb.linearVelocityX < 0f && facingRight)
        {
            Flip();
        }

        
    }

    public void SwitchState(IEnemyState newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;
        currentState.Enter(this);
    }

    public void MoveTowards(float direction)
    {
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    public void StopMoving()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    private void Flip()
    {
        facingRight = !facingRight;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, (transform.eulerAngles.y + 180f) % 360, 0f);
    }
}