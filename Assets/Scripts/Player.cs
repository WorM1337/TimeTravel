using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public enum ActiveAbility
{
    None,
    Rewind,
    SlowTime
}
public class Player : MonoBehaviour, IRewindable, IPlatforming, IDamageable
{
    [Header("Walking and Running")]
    [SerializeField] private float _walkVelocity = 15.0f;
    [SerializeField] private float _runVelocity = 30.0f;
    [SerializeField] private float _maxVelocityYWhileWalking = 5f;
    [SerializeField] private float _timeOfStartMoving = 0.5f;
    private float _currentTimeOfStartMoving = 2f;

    [Header("Jumping")]
    [SerializeField] private float _maxVelocityY = 50f;
    [SerializeField] private float _maxJumpTime = 2f;
    private float _currentMaxJumpTime = 2f;
    [SerializeField] private float _startJumpVelocity = 15f;
    [SerializeField] private float _interraptedJumpVelocity = 15f;
    [SerializeField] private float _gravityScale = 2f;

    [Header("Ground Detection")]
    private Vector2 _boxCastSize; // Размер BoxCast
    [SerializeField] private float _groundCheckDistance = 0.3f; // Расстояние проверки до земли
    [SerializeField] private float _ceilingCheckDistance = 0.1f; // Расстояние проверки до потолка
    [SerializeField] private LayerMask _groundAndCeiling;
    [SerializeField] private LayerMask _platform;

    [Header("Health")]
    private float _maxHealth = 100f;
    private float _currentHealth;
    [SerializeField] private float _minFallHeight = 3f;
    [SerializeField] private float _damagePerMeter = 5.714f;

    public event Action<float> OnHealthChanged; // Событие для UI
    

    [Header("Camera Follow")]
    [SerializeField] GameObject _cameraFollowGO;
    private CameraFollowObject _cameraFollowObject;
    private float _fallSpeedYDampingChangeThreshold;

    private bool _isAbleToJump = false;
    
    [NonSerialized] public bool IsRight = true;
    private bool _isRunning = false;
    private bool _isJumping = false;
    private bool _isGettingDown = false;
    
    public ActiveAbility currentAbility = ActiveAbility.None;

    private float _jumpTimeCounter = 0;
    private float _moveTimeCounter = 0;

    private HashSet<Platform> activePlatforms = new HashSet<Platform>();
    

    private PlayerInput _playerInput;
    private Animator anim;
    private Rigidbody2D _rigidbody;
    private Collider2D _collider;

    private Vector3 position;
    private Quaternion rotation;
    private Vector2 velocity;
    private float health; // Локальная переменная для сохранения здоровья
    private float moveX;
    private bool jumping;
    private bool running;

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

        var moveAction = _playerInput.actions["Move"];
        moveAction.performed += OnMovePerformed;

        _currentHealth = _maxHealth;
        OnHealthChanged?.Invoke(_currentHealth);
        _spawnPosition = transform.position;
    }

    void Start()
    {
        TimeRewindManager.Instance.RegisterRewindable(this);
        _currentHealth = _maxHealth;
        OnHealthChanged?.Invoke(_currentHealth);

        ForbidCollisionForAllPlatforms();
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
        health = _currentHealth;
        moveX = anim.GetFloat("moveX"); // Берем актуальные значения из аниматора
        jumping = anim.GetBool("jumping");
        running = anim.GetBool("running");
        IsRight = IsRight; // Уже сохранено как поле класса
    }

    public object GetState()
    {
        return new PlayerRewindState
        {
            position = position,
            rotation = rotation,
            velocity = velocity,
            health = health,
            moveX = moveX,
            jumping = jumping,
            running = running,
            isRight = IsRight
        };
    }

    public void LoadState(object state)
    {
        var savedState = (PlayerRewindState)state;
        transform.position = savedState.position;
        transform.rotation = savedState.rotation;
        _rigidbody.linearVelocity = savedState.velocity;
        _currentHealth = savedState.health;
        OnHealthChanged?.Invoke(_currentHealth);
        anim.SetFloat("moveX", savedState.moveX);
        anim.SetBool("jumping", savedState.jumping);
        anim.SetBool("running", savedState.running);
        IsRight = savedState.isRight;
        if (IsRight != (transform.eulerAngles.y == 0))
        {
            flip();
        }
    }

    public void OnStartRewind()
    {
        _rigidbody.isKinematic = true;
    }

    public void OnStopRewind()
    {
        _rigidbody.isKinematic = false;
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

        if (_rigidbody.linearVelocityY < _fallSpeedYDampingChangeThreshold / TimeManager.instance.SlowFactor && !CameraManager.instance.IsLerpingYDamping && !CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpYDamping(true);
        }

        if (_rigidbody.linearVelocityY >= 0f && !CameraManager.instance.IsLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpedFromPlayerFalling = false;
            CameraManager.instance.LerpYDamping(false);
        }


    }

    private void FixedUpdate()
    {
        if (TimeRewindManager.Instance != null && TimeRewindManager.Instance.IsRewinding) return;

        Vector2 movement = _playerInput.actions["Move"].ReadValue<Vector2>();
        if (movement.x == 0f) _moveTimeCounter = 0;
        else _moveTimeCounter += Time.unscaledDeltaTime;
        Move(movement);
        TryGetDown(movement.y);

        if(!_isGettingDown)
        {
            CheckPlatformDown();
        }
        
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
            if (!CheckCeiling())
            {
                if (_jumpTimeCounter > 0)
                {
                    _rigidbody.linearVelocityY = Mathf.Lerp(_startJumpVelocity, 0, 1 - _jumpTimeCounter / _currentMaxJumpTime);
                    _jumpTimeCounter -= Time.unscaledDeltaTime;
                }
                else
                {
                    _isJumping = false;
                    _jumpTimeCounter = 0;
                }
            }
            else
            {
                _isJumping = false;
                _rigidbody.linearVelocityY = -_interraptedJumpVelocity;
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
        anim.SetFloat("moveX", Mathf.Abs(direction.x)); // Устанавливаем напрямую
        running = _isRunning; // Синхронизируем running с состоянием
    }

    private void UpdateAnimationState()
    {
        jumping = anim.GetBool("jumping");
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
        if (_isAbleToJump)
        {
            _rigidbody.linearVelocityY = Math.Min(_maxVelocityYWhileWalking, _rigidbody.linearVelocityY);
        }
        else if (Math.Abs(_rigidbody.linearVelocity.y) > _maxVelocityY / TimeManager.instance.SlowFactor)
        {
            _rigidbody.linearVelocityY = _maxVelocityY / TimeManager.instance.SlowFactor *
                (_rigidbody.linearVelocityY >= 0 ? 1f: -1f);
        }
        _rigidbody.gravityScale = (TimeManager.instance.CurrentTimeSpeed == TimeSpeed.Normal ?
            1f : 1 / (TimeManager.instance.SlowFactor * TimeManager.instance.SlowFactor)) * _gravityScale;
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

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        if(context.ReadValue<Vector2>().x != 0)_cameraFollowObject.CheckToTurn();
    }

    private void TryGetDown(float movementY)
    {
        if (movementY == -1)
        {
            foreach (var platform in activePlatforms)
            {
                _isGettingDown = true;
                GetDown(platform);
            }
            activePlatforms.Clear();
            if(_isGettingDown) _rigidbody.linearVelocityY = -_interraptedJumpVelocity / TimeManager.instance.SlowFactor;
        }
        else
        {
            _isGettingDown = false;
        }
    }

    public void ForbidCollisionForAllPlatforms()
    {
        var listOfPlatforms = GameObject.FindGameObjectsWithTag("Platform");

        foreach(var platformObj in listOfPlatforms)
        {
            Platform platform = platformObj.GetComponent<Platform>();

            if(platform == null)
            {
                Debug.Log($"Платформа {platformObj.name} не имеет скрипта!");
                continue;
            }
            else
            {
                platform.ForbidCollision(_collider);
            }
        }
    }

    public void GetDown(Platform platform)
    {
        platform.ForbidCollision(_collider);
    }

    public void CheckPlatformDown()
    {
        Vector2 origin = (Vector2)transform.position - new Vector2(0, _bounds.size.y / 2 - _boxCastSize.y / 2);

        RaycastHit2D hit = Physics2D.BoxCast(
            origin,
            _boxCastSize,
            0f,
            -Vector2.up,
            _groundCheckDistance,
            _platform
        );

        if (hit.collider != null)
        {
            var platform = hit.collider.GetComponent<Platform>();

            if (platform != null)
            {
                platform.AllowCollision(_collider);
                activePlatforms.Add(platform);
            }
            else
            {
                Debug.Log("Ошибка!!! На Платформе отсутствует скрипт!");
            }

            ClearActivePlatforms(platform);
        }
        else
        {
            ClearActivePlatforms(null);
        }
    }
    public void ClearActivePlatforms(Platform? current)
    {

        foreach(var platform in activePlatforms)
        {
            if (current != platform) platform.ForbidCollision(_collider);
        }

        if(activePlatforms.Contains(current))
        {
            activePlatforms = new HashSet<Platform> { current };
        }
        else
        {
            activePlatforms = new HashSet<Platform>();
        }
    }

    private bool CheckCeiling()
    {
        Vector2 origin = (Vector2)transform.position + new Vector2(0, _bounds.size.y / 2 - _boxCastSize.y / 2);

        RaycastHit2D hitCeiling = Physics2D.BoxCast(
            origin,
            _boxCastSize,
            0f,
            Vector2.up,
            _ceilingCheckDistance,
            _groundAndCeiling
        );

        return hitCeiling.collider != null;
    }
    private void CheckGroundAnimated()
    {
        Vector2 origin = (Vector2)transform.position - new Vector2(0, _bounds.size.y / 2 - _boxCastSize.y / 2);

        RaycastHit2D hitGround = Physics2D.BoxCast(
            origin,
            _boxCastSize,
            0f,
            -Vector2.up,
            _groundCheckDistance,
            _groundAndCeiling
        );
        RaycastHit2D hitPlatform = Physics2D.BoxCast(
            origin,
            _boxCastSize,
            0f,
            -Vector2.up,
            _groundCheckDistance,
            _platform
        );

        bool wasGrounded = _isAbleToJump;
        _isAbleToJump = hitGround.collider != null || hitPlatform.collider != null;

        if (_isAbleToJump && !wasGrounded)
        {
            anim.SetBool("jumping", false);

            
        }

        if (!_isAbleToJump && wasGrounded)
        {
            anim.SetBool("jumping", true);
        }
        UpdateAnimationState(); // Обновляем состояние анимации
    }

    public void TakeDamage(float damage)
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
            1f : 1 / (TimeManager.instance.SlowFactor * TimeManager.instance.SlowFactor) * _gravityScale;
    }

    
}

public class PlayerRewindState
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector2 velocity;
    public float health;
    public float moveX;
    public bool jumping;
    public bool running;
    public bool isRight;
}