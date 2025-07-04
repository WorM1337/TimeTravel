using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class CameraScaler : MonoBehaviour
{
    [SerializeField] private float _increasedSizeOfCamera;
    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private float _scaleDuration;
    [SerializeField] private AnimationCurve _scaleCurve;
    private float _oldSize;

    private Coroutine _increasingProcess;
    private Coroutine _decreasingProcess;

    private bool _isIncreasing = false;
    private bool _isDecreasing = false;

    private void Start()
    {
        _oldSize = _camera.Lens.OrthographicSize;
        
    }

    private IEnumerator IncreasingProcess()
    {
        _isIncreasing = true;
        var counter = _camera.Lens.OrthographicSize/_increasedSizeOfCamera * _scaleDuration;

        while (counter < _scaleDuration)
        {
            _camera.Lens.OrthographicSize = Mathf.Lerp(_oldSize, _increasedSizeOfCamera, _scaleCurve.Evaluate(counter / _scaleDuration));
            _camera.GetComponent<CinemachineConfiner2D>().InvalidateBoundingShapeCache();
            counter += Time.unscaledDeltaTime;
            yield return null;
        }
        _camera.Lens.OrthographicSize = _increasedSizeOfCamera;
        _camera.GetComponent<CinemachineConfiner2D>().InvalidateBoundingShapeCache();
        _isIncreasing = false;
    }
    private IEnumerator DecreasingProcess()
    {
        _isDecreasing = true;
        var counter = _scaleDuration - _camera.Lens.OrthographicSize / _increasedSizeOfCamera * _scaleDuration;

        while (counter < _scaleDuration)
        {
            _camera.Lens.OrthographicSize = Mathf.Lerp(_increasedSizeOfCamera, _oldSize, _scaleCurve.Evaluate(counter / _scaleDuration));
            _camera.GetComponent<CinemachineConfiner2D>().InvalidateBoundingShapeCache();
            counter += Time.unscaledDeltaTime;
            yield return null;
        }
        _camera.Lens.OrthographicSize = _oldSize;
        _camera.GetComponent<CinemachineConfiner2D>().InvalidateBoundingShapeCache();
        _isDecreasing = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Player>())
        {

            if (_isDecreasing)
            {
                StopCoroutine(_decreasingProcess);
                _isDecreasing = false;
            }
            _increasingProcess = StartCoroutine(IncreasingProcess());
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            if (_isIncreasing)
            {
                StopCoroutine(_decreasingProcess);
                _isIncreasing = false;
            }
            _decreasingProcess = StartCoroutine(DecreasingProcess());
        }
    }
}
