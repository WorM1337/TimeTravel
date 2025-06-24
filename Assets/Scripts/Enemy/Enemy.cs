using System;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable, IRewindable
{
    [Header("Player")]
    public Transform player;

    [Header("Attack")]
    public float attackDamage = 5f;

    private IEnemyState currentState;


    [Header("Moving")]
    public float moveSpeed = 3f;
    public float patrolTime = 2f;

    [Header("Idle")]
    public float idleTime = 2f;
    [Header("UI")]
    [SerializeField] private HealthBarEnemy _healthBarEnemy;

    private bool _healthBarIsActive = false;

    private float _maxHealth = 100f;
    private float _currentHealth;
    [NonSerialized] public float patrolCounter;

    [NonSerialized] public bool facingRight = true;
    private bool isAlive = true;

    private event Action<float> OnHealthChanged;

    [NonSerialized] public bool IsPlayerInSearchRadius = false;
    [NonSerialized] public bool IsPlayerInAttackRadius = false;

    private Rigidbody2D rb;

    /*
     public Vector3 position;
    public Quaternion rotation;
    public Vector2 velocity;
    public float health;
    public bool isRight;
    public IEnemyState enemyState;
     */
    private Vector3 positionForRewind;
    private Quaternion rotationForRewind;
    private Vector2 velocityForRewind;
    private float healthForRewind;
    private bool isRightForRewind;
    private IEnemyState enemyStateForRewind;
    private bool isAliveForRewind;
    private bool healthBarIsActiveForRewind;
    private float patrolCounterForRewind;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        OnHealthChanged += _healthBarEnemy.SetHealthUI;
        _currentHealth = _maxHealth;
        patrolCounter = patrolTime;
    }
    void Start()
    {
        SwitchState(new PatrolState());
        TimeRewindManager.Instance.RegisterRewindable(this);
    }
    

    void Update()
    {
        currentState.Update();
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

    public void Flip()
    {
        facingRight = !facingRight;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, (transform.eulerAngles.y + 180f) % 360, 0f);

        _healthBarEnemy.Flip();
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, _maxHealth);

        if (!_healthBarIsActive)
        {
            _healthBarIsActive = true;
            _healthBarEnemy.gameObject.SetActive(true);
        }

        OnHealthChanged?.Invoke(_currentHealth / _maxHealth);
        // Обновление прогрессбара

        Debug.Log($"Enemy took {damage} damage. Current health: {_currentHealth}");

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float healAmount)
    {
        _currentHealth += healAmount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, _maxHealth);

        // Обновление интерфейса
        OnHealthChanged?.Invoke(_currentHealth/_maxHealth);

    }

    public float GetCurrentHealth()
    {
        return _currentHealth;
    }

    public float GetMaxHealth()
    {
        return _maxHealth;
    }

    private void Die()
    {
        // Анимации

        gameObject.SetActive(false);
    }

    #region Rewind Logic
    public void SaveState()
    {
        positionForRewind = transform.position;
        rotationForRewind = transform.rotation;
        velocityForRewind = rb.linearVelocity;
        healthForRewind = _currentHealth;
        isRightForRewind = facingRight;
        enemyStateForRewind = currentState;
        isAliveForRewind = isAlive;
        healthBarIsActiveForRewind = _healthBarIsActive;
        patrolCounterForRewind = patrolCounter;
    }

    public object GetState()
    {
        return new EnemyRewindState
        {
            position = positionForRewind,
            rotation = rotationForRewind,
            velocity = velocityForRewind,
            health = healthForRewind,
            isRight = isRightForRewind,
            enemyState = enemyStateForRewind,
            isAlive = isAliveForRewind,
            healthBarIsActive = healthBarIsActiveForRewind,
            patrolCounter = patrolCounterForRewind
        };
    }

    public void LoadState(object state)
    {
        var savedState = (EnemyRewindState)state;

        isAlive = savedState.isAlive;
        gameObject.SetActive(isAlive);

        transform.position = savedState.position;
        transform.rotation = savedState.rotation;
        rb.linearVelocity = savedState.velocity;

        _currentHealth = savedState.health;
        OnHealthChanged(_currentHealth/_maxHealth);

        if (facingRight != savedState.isRight)
        {
            Flip();
        }

        SwitchState(savedState.enemyState);

        _healthBarIsActive = savedState.healthBarIsActive;
        _healthBarEnemy.gameObject.SetActive(savedState.healthBarIsActive);

        patrolCounter = savedState.patrolCounter;
    }

    public void OnStartRewind()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public void OnStopRewind()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
    }
    #endregion
}

public class EnemyRewindState
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector2 velocity;
    public float health;
    public bool isRight;
    public IEnemyState enemyState;
    public bool isAlive;
    public bool healthBarIsActive;
    public float patrolCounter;
}