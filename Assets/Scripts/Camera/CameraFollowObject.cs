using System.Collections;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _playerTransform;

    [Header("Flip Rotation Stats")]
    [SerializeField] private float _flipYRotationTime = 0f;

    private Coroutine _turnCoroutine;

    private Player _player;

    private bool _isFacingRight;

    private void Awake()
    {
        _player = _playerTransform.gameObject.GetComponent<Player>();

        _isFacingRight = _player.IsRight;
    }
    private void Update()
    {
        //make the cameraFollow follow the player's position
        transform.position = _playerTransform.position;    
    }

    public void CheckToTurn()
    {
        if (_isFacingRight != _player.IsRight)
        {
            CallTurn();
        }
    }
    public void CallTurn()
    {
        //_turnCoroutine = StartCoroutine(FlipYLerp());

        LeanTween.rotateY(gameObject, DetermineEndRotation(), _flipYRotationTime * TimeManager.instance.SlowFactor).setEaseInOutSine();
    }

    private IEnumerator FlipYLerp()
    {
        float startRotation = transform.localEulerAngles.y;
        float endRotation = DetermineEndRotation();
        float yRotation = 0f;

        float elapsedTime = 0f;

        while (elapsedTime < _flipYRotationTime)
        {
            elapsedTime += Time.deltaTime;

            //lerp the y rotation
            yRotation = Mathf.Lerp(startRotation, endRotation, (elapsedTime / _flipYRotationTime));
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);

            yield return null;
        }
    }

    private float DetermineEndRotation()
    {
        _isFacingRight = !_isFacingRight;

        if (_isFacingRight)
        {
            return 0f;
        }
        else
        {
            return 180f;
        }
    }

}
