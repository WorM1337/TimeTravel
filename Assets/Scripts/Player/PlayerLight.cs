using UnityEngine;

public class PlayerLight : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    void Update()
    {
        transform.position = _playerTransform.position;
    }
}
