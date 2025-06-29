using UnityEngine;

public class RespawnUI : MonoBehaviour
{
    private Animator _animator;
    private float _oldTimeScale;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    public void PlayUI(float oldTimeScale)
    {
        _oldTimeScale = oldTimeScale;
        _animator.SetBool("HaveToRespawn", true);
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
