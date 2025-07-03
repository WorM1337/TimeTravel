using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private WayPoint[] _wayPoints;
    [SerializeField] private float _movingSpeed = 10f;

    private Rigidbody2D _rigidbody;

    private List<WayPoint> _currentPoints;

    float _movingCounter = 0f;
    float _waitingCounter = 0f;

    private Coroutine _waitingCoroutine;
    private Coroutine _movingCoroutine;

    private bool _isMoving = false;
    private bool _isWaiting = false;

    private int _currentIndex = 0;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        transform.position = _wayPoints[0].PointTransform.position;

        _currentPoints = new List<WayPoint> { _wayPoints[0], _wayPoints[1] };
    }

    private void Update()
    {
        CheckWay();
    }

    private IEnumerator MovingProcess(float startTime, WayPoint pointStart, WayPoint pointEnd)
    {
        _isMoving = true;
        _movingCounter = startTime;

        var generalTime = Vector3.Distance(pointStart.PointTransform.position, pointEnd.PointTransform.position) / _movingSpeed;

        while (_movingCounter < generalTime)
        {
            var newPosition = Vector3.Lerp(pointStart.PointTransform.position, pointEnd.PointTransform.position, _movingCounter / generalTime);

            //_rigidbody.MovePosition(newPosition);
            transform.position = newPosition;
            _movingCounter += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        //_rigidbody.MovePosition(pointEnd.PointTransform.position);
        transform.position = pointEnd.PointTransform.position;

        _isMoving = false;
        _currentIndex = (_currentIndex + 1) % _wayPoints.Length;
        _currentPoints.RemoveAt(0);
    }
    private IEnumerator WaitingProcess(float startTimeWait, WayPoint currentWaitPoint)
    {
        _isWaiting = true;
        _waitingCounter = startTimeWait;

        while(_waitingCounter < currentWaitPoint.WaitTime)
        {
            _waitingCounter += Time.deltaTime;
            yield return null;
        }
        _waitingCounter = currentWaitPoint.WaitTime;
        _isWaiting = false;
        _currentPoints.Add(_wayPoints[(_currentIndex+1)% _wayPoints.Length]);
    }

    private void CheckWay()
    {
        if(_currentPoints.Count == 1 && !_isWaiting)
        {
            _waitingCoroutine = StartCoroutine(WaitingProcess(0, _currentPoints[0]));
        }
        else if (_currentPoints.Count == 2 && !_isMoving)
        {
            _movingCoroutine = StartCoroutine(MovingProcess(0, _currentPoints[0], _currentPoints[1]));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<IMovingPlatform>() is IMovingPlatform obj)
        {
            obj.SetParent(transform);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<IMovingPlatform>() is IMovingPlatform obj)
        {
            obj.SetParent(obj.FirstParent);
        }
    }
}
[System.Serializable]
public class WayPoint
{
    public Transform PointTransform;
    public float WaitTime;
}