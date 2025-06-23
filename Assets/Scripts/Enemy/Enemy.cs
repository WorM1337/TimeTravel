using UnityEngine;

public class Enemy : MonoBehaviour
{
    
    public Transform player;
    [Header("Searching and attack")]
    public float SearchingRadius = 5f;
    public float AttackRadiusDelta = 1.5f;
    public float chaseRange = 10f;
    public float attackRange = 2f;
    public float attackDamage = 5f;

    private IEnemyState currentState;


    
    public float moveSpeed = 3f;
    private Rigidbody2D rb;
    private bool facingRight = true;

    public bool IsPlayerInSearchRadius = false;
    public bool IsPlayerInAttackRadius = false;

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

        
        if ((player.position.x > transform.position.x && !facingRight) ||
            (player.position.x < transform.position.x && facingRight))
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
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}