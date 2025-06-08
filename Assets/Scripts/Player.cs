using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float _walkForce = 15.0f;
    [SerializeField] private float _runForce = 30.0f;
    [SerializeField] private float _forceOfJump = 1.0f;
    [SerializeField] private float _maxWalkingVelocity = 20f;
    [SerializeField] private float _maxRunVelocity = 35f;

    [SerializeField] private LayerMask _ground;

    private bool _isAbleToJump = false;
    private bool _isRight = true;
    private bool _isRunning = false;

    

    // Ссылка на PlayerInput
    private PlayerInput _playerInput;
    private Animator anim;
    private Rigidbody2D _rigidbody;

    void Awake()
    {
        anim = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
        _rigidbody = GetComponent<Rigidbody2D>();
        var jumpAction = _playerInput.actions["Jump"];
        jumpAction.performed += OnJumpPerformed;

        var runAction = _playerInput.actions["Run"];
        runAction.performed += OnRunPerformed;
        runAction.canceled += OnRunCanceled;
    }

    private void FixedUpdate()
    {
        Vector2 movement = _playerInput.actions["Move"].ReadValue<Vector2>();
        Move(movement);

        checkVelocity();
    }

    

    public void TryToJump()
    {
        if (_isAbleToJump)
        {
            _isAbleToJump = false;
            _rigidbody.AddForce(Vector2.up * _forceOfJump, ForceMode2D.Impulse);
        }
    }

    private void Move(Vector2 direction) // Движение игрока
    {
        if (_isRunning)
        {
            _rigidbody.AddForce(direction * _runForce);
        }
        else
        {
            _rigidbody.AddForce(direction * _walkForce);
        }
            

        float right = direction.x;

        if (right > 0 && !_isRight)
        {
            flip();
        }
        else if (right < 0 && _isRight)
        {
            flip();
        }
        anim.SetFloat("moveX", Mathf.Abs(direction.x));
    }

    private void flip() // меняет направление игрока - с лева на право, и с право на лево
    {
        _isRight = !_isRight;
        var transform = GetComponent<Transform>();
        transform.localScale = new Vector3(transform.localScale.x * (-1), transform.localScale.y, 0f);
    }
    private void checkVelocity() // Смотрит, чтобы скорость не превышала допустимую
    {
        float currentMaxVelocity = (_isRunning ? _maxRunVelocity : _maxWalkingVelocity);

        if (_rigidbody.linearVelocity.magnitude > currentMaxVelocity)
        {
            _rigidbody.linearVelocity = _rigidbody.linearVelocity.normalized * currentMaxVelocity;
        }
    }

    // Функции, которые подписаны на события ввода
    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        TryToJump();
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


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((_ground.value & (1 << collision.gameObject.layer)) != 0) // Обработка того, что мы взаимодействуем с землёй
        {
            Debug.Log("grounded");
            _isAbleToJump = true;
            anim.SetBool("jumping", false);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if ((_ground.value & (1 << collision.gameObject.layer)) != 0) // Обработка того, что мы взаимодействуем с землёй
        {
            Debug.Log("ungrounded");
            _isAbleToJump = false;
            anim.SetBool("jumping", true);
        }
    }
}