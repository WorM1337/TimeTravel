using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
[RequireComponent (typeof(Light2D))]
public class LightWithChanger : MonoBehaviour, IRewindable, IInteractable
{
    private Light2D _light;
    private Collider2D _collider;

    [SerializeField] private float _intensity = 2f;
    [SerializeField] private Color _color = Color.white;
    [SerializeField] private bool _isOff = false;
    [SerializeField] private UnityEvent _onLightChange;
    private void Awake()
    {
        _light = GetComponent<Light2D>();
        _collider = GetComponent<Collider2D>();

        UpdateProperties();
    }
    private void Start()
    {
        TimeRewindManager.Instance.RegisterRewindable(this);

        if(_collider) _collider.isTrigger = true;
    }
    public Light2D GetLight2D()
    {
        return _light;
    }
    public bool IsOff()
    {
        return _isOff;
    }
    public void ChangeLightIntensity(float intensity)
    {
        _intensity = intensity;

        UpdateProperties();

        _onLightChange?.Invoke();
    }
    public void ChangeLightColorRed()
    {
        Debug.Log("ChangeRed");
        if(!_isOff) _color = Color.red;

        UpdateProperties();

        _onLightChange?.Invoke();
    }
    public void ChangeLightColorGreen()
    {
        Debug.Log("ChangeGreen");
        if (!_isOff) _color =Color.green;

        UpdateProperties();

        _onLightChange?.Invoke();
    }

    private void UpdateProperties()
    {
        if(!_isOff)
        {
            _light.intensity = _intensity;
            _light.color = _color;
        }
    } 

    public void SaveState()
    {
        
    }

    public object GetState()
    {
        return new LightWithChangerRewindState
        {
            intensity = _intensity,
            color = _color,
            isOff = _isOff,
        };
    }

    public void LoadState(object state)
    {
        var savedState = (LightWithChangerRewindState)state;
        _intensity = savedState.intensity;
        _color = savedState.color;
        _isOff = savedState.isOff;

        UpdateProperties();
    }

    public void OnStartRewind()
    {
        
    }

    public void OnStopRewind()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() is Player player)
        {
            player.CurrentInteractable = this;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() is Player player)
        {
            player.CurrentInteractable = null;
        }
    }
    public void Press()
    {
        _isOff = !_isOff;
        
        UpdateProperties();

        if(_isOff) _light.intensity = 0f;

        _onLightChange?.Invoke();
    }
}
public class LightWithChangerRewindState
{
    public float intensity;
    public Color color;
    public bool isOff;
}
