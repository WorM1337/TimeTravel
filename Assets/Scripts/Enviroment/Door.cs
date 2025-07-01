using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private float _usualSize;
    [SerializeField] private bool _isClosed;
    [SerializeField] private float _closeDuration;
    [SerializeField] private float _openDuration;

    [Header("Follow object")]

    [SerializeField] private Transform _followObjTransform;

    private Coroutine _closing;
    private Coroutine _opening;

    private void Update()
    {
        _followObjTransform.localPosition = new Vector3(_followObjTransform.localPosition.x, -1f, _followObjTransform.localPosition.z);
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
    }
}
