using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour, IRewindable
{
    [SerializeField] private float _usualSize;
    [SerializeField] private bool _isClosed;
    [SerializeField] private float _closeDuration;
    [SerializeField] private float _openDuration;

    [Header("Follow object")]

    [SerializeField] private Transform _followObjTransform;

    private Coroutine _closing;
    private Coroutine _opening;

    private bool isWrapsActive;

    private Vector3 scaleForRewind;
    private bool isClosedForRewind;

    private void Start()
    {
        TimeRewindManager.Instance.RegisterRewindable(this);
        if(_followObjTransform != null)
            isWrapsActive = _followObjTransform.gameObject.activeSelf;
    }

    private void Update()
    {
        if(_followObjTransform != null) _followObjTransform.localPosition = new Vector3(_followObjTransform.localPosition.x, -transform.localScale.y, _followObjTransform.localPosition.z);
    }

    [ContextMenu("Close")]
    public void Close()
    {
        if (!_isClosed)
        {
            if(_opening != null) StopCoroutine(_opening);
            _closing = StartCoroutine(ClosingProcess(transform.localScale.y));
        }
    }
    [ContextMenu("Open")]
    public void Open()
    {
        if(_isClosed)
        {
            if (_closing != null) StopCoroutine(_closing);
            _opening = StartCoroutine(OpeningProcess(transform.localScale.y));
        }
    }
    public void Switch()
    {
        if (_isClosed) Open();
        else Close();
    }
    private IEnumerator ClosingProcess(float startYScale)
    {
        _isClosed = true;
        var counter = 0f;

        var generalDuration = _closeDuration - startYScale / _usualSize * _closeDuration;
        while (counter < generalDuration)
        {
            var height = Mathf.Lerp(startYScale, _usualSize, counter / generalDuration);
            transform.localScale = new Vector3(transform.localScale.x, height, transform.localScale.z);
            counter += Time.deltaTime;
            yield return null;
        }
        transform.localScale = new Vector3(transform.localScale.x, _usualSize, transform.localScale.z);
    }
    public void SetActivityOfWraps()
    {
        if (_followObjTransform != null)
        {
            _followObjTransform.gameObject.SetActive(!isWrapsActive);
            isWrapsActive = !isWrapsActive;
        }
    }
    private IEnumerator OpeningProcess(float startYScale)
    {
        _isClosed = false;
        var counter = 0f;

        var generalDuration = startYScale / _usualSize * _closeDuration;
        while (counter < generalDuration)
        {
            var height = Mathf.Lerp(startYScale, 0, counter / generalDuration);
            transform.localScale = new Vector3(transform.localScale.x, height, transform.localScale.z);
            counter += Time.deltaTime;
            yield return null;
        }
        transform.localScale = new Vector3(transform.localScale.x, 0f, transform.localScale.z);
    }

    public void SaveState()
    {
        scaleForRewind = transform.localScale;
        isClosedForRewind = _isClosed;
    }

    public object GetState()
    {
        return new DoorRewindState
        {
            scale = scaleForRewind,
            isClosed = isClosedForRewind
        };
    }

    public void LoadState(object state)
    {
        StopAllCoroutines();
        var savedState = (DoorRewindState)state;

        transform.localScale = savedState.scale;
        _isClosed = savedState.isClosed;

        if (_isClosed && transform.localScale.y != _usualSize)
        {
            _isClosed = !_isClosed;
            Close();
        }
        if ((!_isClosed) && transform.localScale.y != 0)
        {
            _isClosed = !_isClosed;
            Open();
        }
    }

    public void OnStartRewind()
    {
        var collider = GetComponent<Collider>();
        if(collider != null) collider.isTrigger = true;
    }

    public void OnStopRewind()
    {
        var collider = GetComponent<Collider>();
        if (collider != null) collider.isTrigger = false;
    }
}

public class DoorRewindState
{
    public Vector3 scale;
    public bool isClosed;
} 
