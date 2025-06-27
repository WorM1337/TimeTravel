using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(PlayerInput))]
public class PauseSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;
    private PlayerInput _playerInput;
    private float _oldTimescale;
    private bool _isPaused;
    private void Awake()
    {
        _oldTimescale = Time.timeScale;
        _playerInput = GetComponent<PlayerInput>();

        var pause = _playerInput.actions["Pause"];
        pause.performed += OnPausePerformed;
    }

    private void OnPausePerformed(InputAction.CallbackContext ctx)
    {
        _isPaused = !_isPaused;
        _pauseMenu.gameObject.SetActive(_isPaused);

        SwitchTimeScale();
    }
    private void SwitchTimeScale()
    {
        if(_oldTimescale!= Time.timeScale)
        {
            Time.timeScale = _oldTimescale;
        }
        else
        {
            _oldTimescale = Time.timeScale;
            Time.timeScale = 0;
        }
    }
}
