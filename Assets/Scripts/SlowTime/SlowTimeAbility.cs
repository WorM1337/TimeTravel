using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlowTimeAbility : MonoBehaviour
{

    [SerializeField] private float _maxSlowTime = 3f; // В секундах 
    [SerializeField] private float _culldownSlowTime = 5f;
    [SerializeField] private SlowTimeUI _slowTimeUI;

    private bool _isAbleToSlow = true;
    private bool _isSlow = false;
    private bool _isCulldown = false;

    private PlayerInput _playerInput;


    private Coroutine _slowTimeProcess;
    private Coroutine _slowTimeCulldown;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        var slowTimeAction = _playerInput.actions["SlowTime"];
        slowTimeAction.performed += OnSlowTimePerformed;

        _slowTimeUI.ChangeHourGlassColor(Color.green);
        _slowTimeUI.ChangeAngleOfFilling(0.0f);
    }
    private IEnumerator SlowTimeProcess()
    {
        _isSlow = true;
        _isAbleToSlow = false;

        _slowTimeUI.ChangeHourGlassColor(Color.yellow);


        var counter = 0.0;
        while (counter < _maxSlowTime)
        {


            counter += Time.unscaledDeltaTime;
            yield return null;
        }

        _isSlow = false;


        TimeManager.instance.EditTimeSpeed(TimeSpeed.Normal);

        _slowTimeUI.ChangeHourGlassColor(Color.red);

        _slowTimeCulldown = StartCoroutine(SlowTimeCulldown());
    }

    private IEnumerator SlowTimeCulldown()
    {
        _isAbleToSlow = false;
        var counter = 0.0;
        while (counter < _culldownSlowTime)
        {
            _slowTimeUI.ChangeAngleOfFilling(1f - (float)counter / _culldownSlowTime);
            
            counter += Time.unscaledDeltaTime;
            yield return null;
        }
        _isAbleToSlow = true;
        _slowTimeUI.ChangeHourGlassColor(Color.green);
    }

    private void OnSlowTimePerformed(InputAction.CallbackContext context)
    {
        if (TimeManager.instance.CurrentTimeSpeed != TimeSpeed.Slow && _isAbleToSlow)
        {
            TimeManager.instance.EditTimeSpeed(TimeSpeed.Slow);
            
            _slowTimeProcess = StartCoroutine(SlowTimeProcess());
        }
        else
        {
            if (_isSlow)
            {
                TimeManager.instance.EditTimeSpeed(TimeSpeed.Normal);

                StopCoroutine(_slowTimeProcess);
                _isSlow = false;

                _slowTimeUI.ChangeHourGlassColor(Color.red);

                _slowTimeCulldown = StartCoroutine(SlowTimeCulldown());
            }
        }

        
    }
}
