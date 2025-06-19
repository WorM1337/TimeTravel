using UnityEngine;
using UnityEngine.InputSystem;

public class SlowTimeAbility : MonoBehaviour
{

    [SerializeField] private float _maxSlowTime = 3f; // В секундах 
    [SerializeField] private float _culldownSlowTime = 5f;

    private bool _isAbleToSlow = true;
    private bool _isSlow = false;

    private float _slowTimeCounter = 0;
    private float _slowTimeCulldownCounter = 0;

    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        var slowTimeAction = _playerInput.actions["SlowTime"];
        slowTimeAction.performed += OnSlowTimePerformed;
    }
    private void Update()
    {
        CheckTimeSlow();
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

        
    }
}
