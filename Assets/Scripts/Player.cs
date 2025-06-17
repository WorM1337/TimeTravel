using System.Collections.Generic;
using Unity.Hierarchy;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
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

    [Header("Layer Mask")]
    [SerializeField] private LayerMask _ground;
    [Header("Camera Follow")]

    [SerializeField] GameObject _cameraFollowGO;
    private CameraFollowObject _cameraFollowObject;
    private float _fallSpeedYDampingChangeThreshold;
    
    private bool _isAbleToJump = false;
    public bool IsRight = true;
    private bool _isRunning = false;
    private bool _isJumping = false;

    private float _jumpTimeCounter = 0;
    private float _moveTimeCounter = 0;

    private int _groundedCount = 0;

    // Ссылка на PlayerInput
    private PlayerInput _playerInput;
    private Animator anim;
    private Rigidbody2D _rigidbody;

    void Awake()
    {
        anim = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _cameraFollowObject = _cameraFollowGO.GetComponent<CameraFollowObject>();

        _fallSpeedYDampingChangeThreshold = CameraManager.instance._fallSpeedYDampingChangeThreshold;

        var jumpAction = _playerInput.actions["Jump"];
        jumpAction.performed += OnJumpPerformed;
        jumpAction.canceled += OnJumpCanceled;

        var runAction = _playerInput.actions["Run"];
        runAction.performed += OnRunPerformed;
        runAction.canceled += OnRunCanceled;

        var slowTimeAction = _playerInput.actions["SlowTime"];
        slowTimeAction.performed += OnSlowTimePerformed;
    }


    private void Update()
    {

        _currentMaxJumpTime = _maxJumpTime * TimeManager.instance.SlowFactor;
        _currentTimeOfStartMoving = _timeOfStartMoving * TimeManager.instance.SlowFactor;

        // if we are falling past a certain speed threashold
        if (_rigidbody.linearVelocityY < _fallSpeedYDampingChangeThreshold && !CameraManager.instance.IsLerpingYDamping && !CameraManager.instance.LerpedFromPlayerFalling)
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
        Vector2 movement = _playerInput.actions["Move"].ReadValue<Vector2>();

        if (movement.x == 0f) _moveTimeCounter = 0;
        else _moveTimeCounter += Time.unscaledDeltaTime;
        Move(movement);

        checkVelocity();
        TryToHoldJump();
    }



    public void TryToJump()
    {
        if (_isAbleToJump)
        {
            _isAbleToJump = false;
            _isJumping = true;

            // Логика через velocity

            _jumpTimeCounter = _maxJumpTime;

            _rigidbody.linearVelocityY = _startJumpVelocity / TimeManager.instance.SlowFactor;

            //_rigidbody.AddForce(Vector2.up * _forceOfJump, ForceMode2D.Impulse);
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

    private void Move(Vector2 direction) // Движение игрока
    {
        if (_isRunning)
        {
            _rigidbody.linearVelocityX = direction.x * (_moveTimeCounter > _currentTimeOfStartMoving ?
                _runVelocity : Mathf.Lerp(0, _runVelocity, _moveTimeCounter/_currentTimeOfStartMoving)) / TimeManager.instance.SlowFactor;
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

    private void flip() // меняет направление игрока - с лева на право, и с право на лево
    {
        IsRight = !IsRight;
        var transform = GetComponent<Transform>();
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, (transform.eulerAngles.y + 180f) % 360, 0f);
        _cameraFollowObject.CallTurn();
    }
    private void checkVelocity() // Смотрит, чтобы скорость не превышала допустимую
    {
        if (_rigidbody.linearVelocity.y > _maxVelocityY / TimeManager.instance.SlowFactor)
        {
            _rigidbody.linearVelocityY = _maxVelocityY / TimeManager.instance.SlowFactor;
        }
    }

    // Функции, которые подписаны на события ввода
    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        TryToJump();
    }
    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        if(_isJumping) _rigidbody.linearVelocityY = -_interraptedJumpVelocity;
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

        if(TimeManager.instance.CurrentTimeSpeed != TimeSpeed.Slow)
        {
            TimeManager.instance.EditTimeSpeed(TimeSpeed.Slow);
        }
        else
        {
            TimeManager.instance.EditTimeSpeed(TimeSpeed.Normal);
        }

        _rigidbody.gravityScale = (TimeManager.instance.CurrentTimeSpeed == TimeSpeed.Normal ?
            1f : 1/(TimeManager.instance.SlowFactor * TimeManager.instance.SlowFactor));
        
    }

    private Dictionary<int, Collision2D> _lastGroundedCollisions = new Dictionary<int, Collision2D>();    

    private bool IsGroundedCollision(Collision2D collision)
    {

        var flag = false;


        if ((_ground.value & (1 << collision.gameObject.layer)) == 0) return false;

        foreach(var contact in collision.contacts)
        {
            float angles = Vector2.Angle(Vector2.up, contact.normal);

            if (angles <= _minJumpDegrees)
            {
                flag = true; break;
            }
        }
        return flag;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsGroundedCollision(collision)) // Обработка того, что мы взаимодействуем с землёй
        {
            _groundedCount++;

            var key = collision.gameObject.GetHashCode();

            if (_lastGroundedCollisions.ContainsKey(key))
            {
                _lastGroundedCollisions[key] = collision;
            }
            else
            {
                _lastGroundedCollisions.Add(key, collision);
            }

            Debug.Log("EnterCountGrounds " + _groundedCount + $" {collision.gameObject}");

            if (_groundedCount != 0)
            {
                _isAbleToJump = true;
                anim.SetBool("jumping", false);
            }

            
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (IsGroundedCollision(collision)) // Обработка того, что мы взаимодействуем с землёй
        {

            var key = collision.gameObject.GetHashCode();

            _lastGroundedCollisions[key] = collision;


        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        var key = collision.gameObject.GetHashCode();

        if (_lastGroundedCollisions.ContainsKey(key)) // Обработка того, что мы перестаём взаимодействовать именно с землей
        {
            _groundedCount--;

            _lastGroundedCollisions.Remove(key);

            Debug.Log("ExitCountGrounds " + _groundedCount + $" {collision.gameObject}");

            

            if (_groundedCount == 0)
            {
                _isAbleToJump = false;
                anim.SetBool("jumping", true);
            }
        }
    }
}