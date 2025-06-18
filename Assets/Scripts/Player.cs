using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class Player : MonoBehaviour, IRewindable
{
    [Header("Walking and Running")]
    [SerializeField] private float _walkVelocity = 15.0f;
    [SerializeField] private float _runVelocity = 30.0f;
    [SerializeField] private float _timeOfStartMoving = 0.5f;
    private float _currentTimeOfStartMoving = 2f;

    [Header("Jumping")]
    [SerializeField] private float _maxVelocityY = 50f;
    [SerializeField] private float _maxJumpTime = 2f;
    private float _currentMaxJumpTime = 2f;
    [SerializeField] private float _startJumpVelocity = 15f;
    [SerializeField] private float _interraptedJumpVelocity = 15f;
    [SerializeField] private float _minJumpDegrees = 25f;

    [Header("Ground Detection")]
    private Vector2 _boxCastSize; // Размер BoxCast
    [SerializeField] private float _groundCheckDistance = 0.3f; // Расстояние проверки до земли
    [SerializeField] private LayerMask _ground;

    [Header("Health")]
    private float _maxHealth = 100f;
    private float _currentHealth;
    [SerializeField] private float _minFallHeight = 3f;
    [SerializeField] private float _damagePerMeter = 5.714f;

    public event Action<float> OnHealthChanged; // Событие для UI
    [Header("SlowAbility")]
    [SerializeField] private float _maxSlowTime = 3f; // В секундах 
    [SerializeField] private float _culldownSlowTime = 5f;

    [Header("Camera Follow")]
    [SerializeField] GameObject _cameraFollowGO;
    private CameraFollowObject _cameraFollowObject;
    private float _fallSpeedYDampingChangeThreshold;

    private bool _isAbleToJump = false;
    private bool _isAbleToSlow = true;
    public bool IsRight = true;
    private bool _isRunning = false;
    private bool _isJumping = false;
    private bool _isSlow = false;

    private float _jumpTimeCounter = 0;
    private float _moveTimeCounter = 0;
    private float _slowTimeCounter = 0;
    private float _slowTimeCulldownCounter = 0;

    private PlayerInput _playerInput;
    private Animator anim;
    private Rigidbody2D _rigidbody;
    private Collider2D _collider;

    private Vector3 position;
    private Quaternion rotation;
    private Vector2 velocity;
    private float health; // Локальная переменная для сохранения здоровья

    private Bounds _bounds;
    private float _maxHeight;
    private bool _isFalling;
    private Vector3 _spawnPosition;

    void Awake()
    {
        _collider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _cameraFollowObject = _cameraFollowGO.GetComponent<CameraFollowObject>();
        _fallSpeedYDampingChangeThreshold = CameraManager.instance._fallSpeedYDampingChangeThreshold;

        _bounds = _collider.bounds;
        _boxCastSize = new Vector2(_bounds.size.x * 0.9f, 1f);

        var jumpAction = _playerInput.actions["Jump"];
        jumpAction.performed += OnJumpPerformed;
        jumpAction.canceled += OnJumpCanceled;

        var runAction = _playerInput.actions["Run"];
        runAction.performed += OnRunPerformed;
        runAction.canceled += OnRunCanceled;

        var slowTimeAction = _playerInput.actions["SlowTime"];
        slowTimeAction.performed += OnSlowTimePerformed;

        _currentHealth = _maxHealth;
        OnHealthChanged?.Invoke(_currentHealth);
        _spawnPosition = transform.position;
    }

    void Start()
    {
        TimeRewindManager.Instance.RegisterRewindable(this);
        _currentHealth = _maxHealth;
        OnHealthChanged?.Invoke(_currentHealth);
    }

    void OnDestroy()
    {
        TimeRewindManager.Instance.UnregisterRewindable(this);
    }

    public void SaveState()
    {
        position = transform.position;
        rotation = transform.rotation;
        velocity = _rigidbody.linearVelocity;
        health = _currentHealth; // Сохраняем текущее здоровье
    }

    public object GetState()
    {
        return new PlayerState
        {
            position = position,
            rotation = rotation,
            velocity = velocity,
            health = health // Возвращаем сохранённое здоровье
        };
    }

    public void LoadState(object state)
    {
        var savedState = (PlayerState)state;
        transform.position = savedState.position;
        transform.rotation = savedState.rotation;
        _rigidbody.linearVelocity = savedState.velocity;
        _currentHealth = savedState.health; // Восстанавливаем здоровье
        OnHealthChanged?.Invoke(_currentHealth); // Уведомляем UI об изменении
    }

    public void OnStartRewind()
    {
        _rigidbody.isKinematic = true; // Отключаем физику
    }

    public void OnStopRewind()
    {
        _rigidbody.isKinematic = false; // Включаем физику обратно
    }

    private void Update()
    {
        if (TimeRewindManager.Instance != null && TimeRewindManager.Instance.IsRewinding) return;

        _currentMaxJumpTime = _maxJumpTime * TimeManager.instance.SlowFactor;
        _currentTimeOfStartMoving = _timeOfStartMoving * TimeManager.instance.SlowFactor;

        if (!_isAbleToJump && _rigidbody.linearVelocityY > 0)
        {
            _maxHeight = Mathf.Max(_maxHeight, transform.position.y);
            _isFalling = false;
        }
        else if (!_isAbleToJump && _rigidbody.linearVelocityY < 0)
        {
            _isFalling = true;
        }

        if (_rigidbody.linearVelocityY < _fallSpeedYDampingChangeThreshold && !CameraManager.instance.IsLerpingYDamping && !CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpYDamping(true);
        }

        if (_rigidbody.linearVelocityY >= 0f && !CameraManager.instance.IsLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpedFromPlayerFalling = false;
            CameraManager.instance.LerpYDamping(false);
        }

        CheckTimeSlow();
    }

    private void FixedUpdate()
    {
        if (TimeRewindManager.Instance != null && TimeRewindManager.Instance.IsRewinding) return;

        Vector2 movement = _playerInput.actions["Move"].ReadValue<Vector2>();
        if (movement.x == 0f) _moveTimeCounter = 0;
        else _moveTimeCounter += Time.unscaledDeltaTime;
        Move(movement);

        CheckGroundAnimated();
        checkVelocity();
        TryToHoldJump();
    }

    public void TryToJump()
    {
        if (_isAbleToJump)
        {
            _isAbleToJump = false;
            _isJumping = true;
            _jumpTimeCounter = _maxJumpTime;
            _rigidbody.linearVelocityY = _startJumpVelocity / TimeManager.instance.SlowFactor;
            _maxHeight = transform.position.y;
        }
    }

    private void TryToHoldJump()
    {
        if (_isJumping)
        {
            if (_jumpTimeCounter > 0)
            {
                _rigidbody.linearVelocityY = Mathf.Lerp(_startJumpVelocity, 0, 1 - _jumpTimeCounter / _currentMaxJumpTime);
                _jumpTimeCounter -= Time.unscaledDeltaTime;
            }
            else
            {
                _isJumping = false;
            }
        }
    }

    private void Move(Vector2 direction)
    {
        if (_isRunning)
        {
            _rigidbody.linearVelocityX = direction.x * (_moveTimeCounter > _currentTimeOfStartMoving ?
                _runVelocity : Mathf.Lerp(0, _runVelocity, _moveTimeCounter / _currentTimeOfStartMoving)) / TimeManager.instance.SlowFactor;
        }
        else
        {
            _rigidbody.linearVelocityX = direction.x * (_moveTimeCounter > _currentTimeOfStartMoving ?
                _walkVelocity : Mathf.Lerp(0, _walkVelocity, _moveTimeCounter / _currentTimeOfStartMoving)) / TimeManager.instance.SlowFactor;
        }

        float right = direction.x;
        if (right > 0 && !IsRight)
        {
            flip();
        }
        else if (right < 0 && IsRight)
        {
            flip();
        }
        anim.SetFloat("moveX", Mathf.Abs(direction.x));
    }

    private void flip()
    {
        IsRight = !IsRight;
        var transform = GetComponent<Transform>();
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, (transform.eulerAngles.y + 180f) % 360, 0f);
        _cameraFollowObject.CallTurn();
    }

    private void checkVelocity()
    {
        if (_rigidbody.linearVelocity.y > _maxVelocityY / TimeManager.instance.SlowFactor)
        {
            _rigidbody.linearVelocityY = _maxVelocityY / TimeManager.instance.SlowFactor;
        }
    }

    private void OnJumpPerformed(InputAction.CallbackContext context) => TryToJump();
    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        if (_isJumping) _rigidbody.linearVelocityY = -_interraptedJumpVelocity;
        _isJumping = false;
    }

    private void OnRunPerformed(InputAction.CallbackContext context)
    {
        _isRunning = true;
        anim.SetBool("running", true);
    }

    private void OnRunCanceled(InputAction.CallbackContext context)
    {
        _isRunning = false;
        anim.SetBool("running", false);
    }

    private void OnSlowTimePerformed(InputAction.CallbackContext context)
    {
        if (TimeManager.instance.CurrentTimeSpeed != TimeSpeed.Slow && _isAbleToSlow)
        {
            TimeManager.instance.EditTimeSpeed(TimeSpeed.Slow);
            _isSlow = true;
        }
        else
        {
            TimeManager.instance.EditTimeSpeed(TimeSpeed.Normal);
            _isSlow = false;
        }

        _rigidbody.gravityScale = (TimeManager.instance.CurrentTimeSpeed == TimeSpeed.Normal ?
            1f : 1 / (TimeManager.instance.SlowFactor * TimeManager.instance.SlowFactor));
    }

    private void CheckTimeSlow()
    {
        if (_slowTimeCounter > _maxSlowTime)
        {
            TimeManager.instance.EditTimeSpeed(TimeSpeed.Normal);
            _isSlow = false;
            _slowTimeCounter = 0;
            _isAbleToSlow = false;
            _slowTimeCulldownCounter = 0;
        }
        else if (_isSlow)
        {
            _slowTimeCounter += Time.unscaledDeltaTime;
        }

        if (!_isAbleToSlow)
        {
            if (_slowTimeCulldownCounter > _culldownSlowTime)
            {
                _slowTimeCulldownCounter = 0;
                _isAbleToSlow = true;
            }
            else
            {
                _slowTimeCulldownCounter += Time.unscaledDeltaTime;
            }
        }
    }

    private void CheckGroundAnimated()
    {
        Vector2 origin = (Vector2)transform.position - new Vector2(0, _bounds.size.y / 2 - _boxCastSize.y / 2);

        RaycastHit2D hit = Physics2D.BoxCast(
            origin,
            _boxCastSize,
            0f,
            -Vector2.up,
            _groundCheckDistance,
            _ground
        );

        bool wasGrounded = _isAbleToJump;
        _isAbleToJump = hit.collider != null;

        if (_isAbleToJump && !wasGrounded)
        {
            anim.SetBool("jumping", false);

            if (_isFalling)
            {
                float fallHeight = _maxHeight - transform.position.y;

                if (fallHeight > _minFallHeight)
                {
                    float damage = (fallHeight - _minFallHeight) * _damagePerMeter;
                    TakeDamage(damage);
                }

                _isFalling = false;
            }
        }

        if (!_isAbleToJump && wasGrounded)
        {
            anim.SetBool("jumping", true);
        }
    }

    private Dictionary<int, Collision2D> _lastGroundedCollisions = new Dictionary<int, Collision2D>();

    private void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, _maxHealth);
        OnHealthChanged?.Invoke(_currentHealth);
        Debug.Log($"Player took {damage} damage. Current health: {_currentHealth}");

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float healAmount)
    {
        _currentHealth += healAmount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, _maxHealth);
        OnHealthChanged?.Invoke(_currentHealth);
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
        Debug.Log("Player died");
        Respawn();
    }

    private void Respawn()
    {
        _currentHealth = _maxHealth;
        OnHealthChanged?.Invoke(_currentHealth);

        transform.position = _spawnPosition;
        _rigidbody.linearVelocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;

        _isJumping = false;
        _isRunning = false;
        _isAbleToJump = true;
        _isFalling = false;
        _maxHeight = transform.position.y;
        _jumpTimeCounter = 0;
        _moveTimeCounter = 0;

        anim.SetBool("jumping", false);
        anim.SetBool("running", false);
        anim.SetFloat("moveX", 0f);

        _rigidbody.gravityScale = TimeManager.instance.CurrentTimeSpeed == TimeSpeed.Normal ?
            1f : 1 / (TimeManager.instance.SlowFactor * TimeManager.instance.SlowFactor);
    }
}

// Класс состояния для отката
public class PlayerState
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector2 velocity;
    public float health; // Делаем public для доступа
}