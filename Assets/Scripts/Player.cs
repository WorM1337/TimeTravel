using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float _force = 15.0f;
    [SerializeField] private float _forceOfJump = 1.0f;
    [SerializeField] private float _maxVelocity = 20f;

    private bool _isAbleToJump = false;
    private bool _isRight = true;

    [SerializeField] private LayerMask _ground;

    // —сылка на PlayerInput
    private PlayerInput _playerInput;

    public Animator anim;

    private Rigidbody2D _rigidbody;
    void Awake()
    {
        anim = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
        _rigidbody = GetComponent<Rigidbody2D>();
        var jumpAction = _playerInput.actions["Jump"];
        jumpAction.performed += OnJumpPerformed;


    }
    void Start()
    {
        
        

        
    }

    private void FixedUpdate()
    {
        // „итаем текущее значение осей в каждом фрейме
        Vector2 movement = _playerInput.actions["Move"].ReadValue<Vector2>();
        Move(movement);

        if (_isAbleToJump)
        {
            anim.SetBool("jumping", false);
        }
        else {
            anim.SetBool("jumping", true);   
        }

        if (_rigidbody.linearVelocity.magnitude > _maxVelocity)
        {
            _rigidbody.linearVelocity = _rigidbody.linearVelocity.normalized * _maxVelocity;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((_ground.value & (1 << collision.gameObject.layer)) != 0)
        {
            Debug.Log("grounded");
            _isAbleToJump = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if ((_ground.value & (1 << collision.gameObject.layer)) != 0)
        {
            Debug.Log("ungrounded");
            _isAbleToJump = false;
        }
    }

    public void TryToJump()
    {
        if (_isAbleToJump)
        {
            _isAbleToJump = false;
            _rigidbody.AddForce(Vector2.up * _forceOfJump, ForceMode2D.Impulse);
        }
    }

    private void Move(Vector2 direction)
    {
        _rigidbody.AddForce(direction * _force);

        float right = direction.x;

        if (right > 0 && !_isRight)
        {
            Flip();
        }
        else if (right < 0 && _isRight)
        {
            Flip();
        }
        anim.SetFloat("moveX", Mathf.Abs(direction.x));
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        float isJump = context.ReadValue<float>();
        if (isJump == 1.0) TryToJump();
    }

    private void Flip()
    {
        _isRight = !_isRight;
        var transform = GetComponent<Transform>();
        transform.localScale = new Vector3(transform.localScale.x * (-1), transform.localScale.y, 0f);

    }
    
}