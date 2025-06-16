using UnityEngine;

public class CameraTransformer : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;

    [SerializeField] private float _movingSpeed;

    private void Update()
    {
        if(_playerTransform != null)
        {
            Vector3 target = new Vector3()
            {
                x = _playerTransform.position.x,
                y = _playerTransform.position.y,
                z = _playerTransform.position.z - 10
            };
            Vector3 pos = Vector3.Lerp(this.transform.position, target, _movingSpeed*Time.unscaledDeltaTime);

            this.transform.position = pos;
        }
    }

}
