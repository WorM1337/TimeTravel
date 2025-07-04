using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class MovingPlatform : MonoBehaviour, IRewindable
{
    [SerializeField] private WayPoint[] _wayPoints;
    [SerializeField] private float _movingSpeed = 10f;

    private Rigidbody2D _rigidbody;

    [SerializeField] private List<WayPoint> _currentPoints;

    float _movingCounter = 0f;
    float _waitingCounter = 0f;

    private Coroutine _waitingCoroutine;
    private Coroutine _movingCoroutine;

    [SerializeField] private bool _isMoving = false;
    [SerializeField] private bool _isWaiting = false;

    [SerializeField] private int _currentIndex = 0;

    [SerializeField] private bool _isActive; 

    [SerializeField] private bool _isRewind = false;

    [SerializeField] private int countOfMovingCoroutines = 0; // Debug
    [SerializeField] private int countOfWaitingCoroutines = 0; // Debug
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        transform.position = _wayPoints[0].PointTransform.position;

        _currentPoints = new List<WayPoint> { _wayPoints[0], _wayPoints[1] };
    }
    private void Start()
    {
        TimeRewindManager.Instance.RegisterRewindable(this);
    }
    private void Update()
    {
        if(!_isRewind && _isActive) CheckWay();
    }

    private IEnumerator MovingProcess(float startTime, WayPoint pointStart, WayPoint pointEnd)
    {
        countOfMovingCoroutines++;
        _isMoving = true;
        _movingCounter = startTime;

        var generalTime = Vector3.Distance(pointStart.PointTransform.position, pointEnd.PointTransform.position) / _movingSpeed;

        while (_movingCounter < generalTime)
        {
            var newPosition = Vector3.Lerp(pointStart.PointTransform.position, pointEnd.PointTransform.position, _movingCounter / generalTime);

            //_rigidbody.MovePosition(newPosition);
            transform.position = newPosition;
            _movingCounter += Time.deltaTime;
            yield return new WaitUntil(() => _isActive);
        }

        //_rigidbody.MovePosition(pointEnd.PointTransform.position);
        transform.position = pointEnd.PointTransform.position;

        _isMoving = false;
        _currentIndex = (_currentIndex + 1) % _wayPoints.Length;
        _currentPoints.RemoveAt(0);
        countOfMovingCoroutines--;
    }
    private IEnumerator WaitingProcess(float startTimeWait, WayPoint currentWaitPoint)
    {
        countOfWaitingCoroutines++;
        _isWaiting = true;
        _waitingCounter = startTimeWait;

        while(_waitingCounter < currentWaitPoint.WaitTime)
        {
            _waitingCounter += Time.deltaTime;
            yield return new WaitUntil(() => _isActive);
        }
        _waitingCounter = currentWaitPoint.WaitTime;
        _isWaiting = false;
        _currentPoints.Add(_wayPoints[(_currentIndex+1)% _wayPoints.Length]);
        countOfWaitingCoroutines--;
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

    public void SwitchActivation()
    {
        _isActive = !_isActive;
    }

    public void SaveState()
    {
        
    }

    public object GetState()
    {
        return new MovingPlatformRewindState
        {
            position = transform.position,
            currentPoints = _currentPoints.ToList(),
            movingCounter = _movingCounter,
            waitingCounter = _waitingCounter,
            isMoving = _isMoving,
            isWaiting = _isWaiting,
            currentIndex = _currentIndex,
            isActive = _isActive,
        };
    }

    public void LoadState(object state)
    {
        var savedState = (MovingPlatformRewindState)state;

        transform.position = savedState.position;

        _currentPoints = savedState.currentPoints.ToList();

        _movingCounter = savedState.movingCounter;
        _waitingCounter = savedState.waitingCounter;

        _isMoving = savedState.isMoving;    
        _isWaiting = savedState.isWaiting;
        _currentIndex = savedState.currentIndex;
        _isActive = savedState.isActive;
    }

    public void OnStartRewind()
    {
        _isRewind = true;

        if (_movingCoroutine != null) { StopCoroutine(_movingCoroutine); countOfMovingCoroutines--; }
        else Debug.Log("Moving coroutine is null!");
        if (_waitingCoroutine != null) StopCoroutine(_waitingCoroutine);
        else Debug.Log("Waiting coroutine is null!");

        GetComponent<Collider2D>().enabled = false;
    }

    public void OnStopRewind()
    {
        Debug.Log("Вызвался конец rewind");
        GetComponent<Collider2D>().enabled = true;
        if (_currentPoints.Count == 1 && _isWaiting)
        {
            _waitingCoroutine = StartCoroutine(WaitingProcess(_waitingCounter, _currentPoints[0]));
        }
        else if (_currentPoints.Count == 2 && _isMoving)
        {
            _movingCoroutine = StartCoroutine(MovingProcess(_movingCounter, _currentPoints[0], _currentPoints[1]));
        }
        _isRewind = false;
    }
}
[System.Serializable]
public class WayPoint
{
    public Transform PointTransform;
    public float WaitTime;
}

public class MovingPlatformRewindState
{
    public Vector3 position;
    public List<WayPoint> currentPoints;
    public float movingCounter;
    public float waitingCounter;
    public bool isMoving;
    public bool isWaiting;
    public int currentIndex;
    public bool isActive;
}