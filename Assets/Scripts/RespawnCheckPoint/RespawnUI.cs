using UnityEngine;
[RequireComponent (typeof(Animator))]
public class RespawnUI : MonoBehaviour
{
    private Animator _animator;
    private float _oldTimeScale;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null) Debug.Log("Animator for UI Respawn is null in Awake!");
    }
    public void PlayUI()
    {
        if(Time.timeScale != 0f)
        {
            _oldTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            if (_animator == null) Debug.Log("Animator for UI Respawn is null!");
            _animator.SetBool("HaveToRespawn", true);
        }
    }
    public void OnStartGettingDark()
    {
        _animator.SetBool("HaveToRespawn", false);
    }
    public void OnEndGettingDark()
    {
        Time.timeScale = _oldTimeScale;
        CameraManager.instance.GoToPlayer();
    }
}
