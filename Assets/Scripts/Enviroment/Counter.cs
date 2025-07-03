using UnityEngine;
using UnityEngine.Events;

public class Counter : MonoBehaviour
{
    [SerializeField] private UnityEvent _eventAfterRequiredCount;
    [SerializeField] private UnityEvent _eventAfterCanceled;

    [SerializeField] private int _requiredCount = 2;

    [SerializeField] private int _currentCount = 0;

    [SerializeField] private bool _isInvoked = false;
    public void PlusCount()
    {
        _currentCount++;
        CheckRequired();
    }
    public void MinusCount()
    {
        _currentCount--;
        CheckRequired();
    }

    public void CheckRequired()
    {
        if( _isInvoked && _currentCount != _requiredCount)
        {
            _isInvoked = false;

            _eventAfterCanceled?.Invoke();
        }
        else if(_currentCount == _requiredCount)
        {
            _isInvoked = true;
            _eventAfterRequiredCount?.Invoke();
        }
    }
}
