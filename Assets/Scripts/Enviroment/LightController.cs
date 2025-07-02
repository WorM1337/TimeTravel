using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class LightController : MonoBehaviour
{
    [SerializeField] private LightWithChanger[] _lights;

    [Header("ConditionsOfProblem")]
    [SerializeField] private Color[] _colorsForLights; // Количество строго совпадает с _lights!!!
    [SerializeField] private UnityEvent _eventAfterComplete;
    [SerializeField] private UnityEvent _eventAfterUncomplete;

    private bool _isCompleted = false;
    [ContextMenu("CheckComplete")]
    public void CheckComplete()
    {
        var ans = true;
        for(int i = 0; i < _lights.Length;i++)
        {
            if (_lights[i].GetLight2D().color != _colorsForLights[i] || _lights[i].IsOff()) ans = false;

            if (!ans) break;
        }
        if(ans)
        {
            _isCompleted = true;
            _eventAfterComplete?.Invoke();
        }
        else
        {
            if (_isCompleted)
            {
                _eventAfterUncomplete?.Invoke();
            }
            _isCompleted = false;
        }
    }

}
