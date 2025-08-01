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
    public float _fallSpeedYDampingChangeThreshold = -40f;

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
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
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

    public void GoToPlayer()
    {
        Debug.Log(_currentCamera.Target.TrackingTarget.position);
        _cinemachinePositionComposer.ForceCameraPosition(_currentCamera.Target.TrackingTarget.position, Quaternion.identity);
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

            float elapsedTime = 0f;
            while (elapsedTime < endDampAmount)
            {

                float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, (elapsedTime / endDampAmount));
                _cinemachinePositionComposer.Damping.y = lerpedPanAmount;

                elapsedTime += Time.unscaledDeltaTime;

                yield return null;
            }

        }
        else
        {
            endDampAmount = _normYPanAmount;
        }

        

        _cinemachinePositionComposer.Damping.y = endDampAmount;

        IsLerpingYDamping = false;
    }
    #endregion 
}
