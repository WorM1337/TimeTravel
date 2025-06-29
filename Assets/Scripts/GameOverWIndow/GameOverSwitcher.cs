using UnityEngine;

public class GameOverSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject _gameoverMenu;
    private bool _isActive = false;
    private float _oldTimescale;
    public void SwitchMenu()
    {
        _isActive = !_isActive;

        _gameoverMenu.SetActive(_isActive);

        SwitchTimeScale();
    }
    private void SwitchTimeScale()
    {
        if (_oldTimescale != Time.timeScale)
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
