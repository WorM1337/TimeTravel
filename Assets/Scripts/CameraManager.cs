using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera[] _cinemachineCameras;

    public static CameraManager instance;

    [Header("Controls the learping the Y damping during player jump/fall")]
    [SerializeField] private float _fallPanAmount = 0.25f;
    [SerializeField] private float _fallYPanTime = 0.35f;
    public float _fallSpeedYDampingChangeThreshold = -10f;

    public bool IsLerpingYDamping { get; private set; }
    public bool LerpedFromPlayerFalling { get; set; }

    private Coroutine _lerpYPanCoroutine;

    private CinemachineCamera _currentCamera;

    private CinemachinePositionComposer _cinemachinePositionComposer;


    private float _normYPanAmount;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        for (int i = 0; i < _cinemachineCameras.Length; i++)
        {
            if (_cinemachineCameras[i].enabled)
            {
                //set the current active camera
                _currentCamera = _cinemachineCameras[i];

                // set the position composer
                _cinemachinePositionComposer = _currentCamera.GetComponent<CinemachinePositionComposer>();
            }
        }
        _normYPanAmount = _cinemachinePositionComposer.Damping.y;
    }

    #region Lerp the Y Damping

    public void LerpYDamping(bool isPlayerFalling)
    {
        _lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
    }

    private IEnumerator LerpYAction(bool isPlayerFalling)
    {
        IsLerpingYDamping = true;

        //grab the starting damping amount
        float startDampAmount = _cinemachinePositionComposer.Damping.y;
        float endDampAmount = 0f;

        if (isPlayerFalling)
        {
            endDampAmount = _fallPanAmount;
            LerpedFromPlayerFalling = true;
        }
        else
        {
            endDampAmount = _normYPanAmount;
        }

        float elapsedTime = 0f;
        while (elapsedTime < endDampAmount)
        {
            elapsedTime += Time.deltaTime;

            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, (elapsedTime/_fallYPanTime));
            _cinemachinePositionComposer.Damping.y = lerpedPanAmount;

            yield return null;
        }

        IsLerpingYDamping = false;
    }
    #endregion 
}
