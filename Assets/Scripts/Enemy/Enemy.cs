using System;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
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

    private bool facingRight = true;

    private event Action<float> OnHealthChanged;

    [NonSerialized] public bool IsPlayerInSearchRadius = false;
    [NonSerialized] public bool IsPlayerInAttackRadius = false;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        OnHealthChanged += _healthBarEnemy.SetHealthUI;
        _currentHealth = _maxHealth;
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

        Destroy(gameObject);
    }
}