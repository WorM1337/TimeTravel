using UnityEngine;
using UnityEngine.Rendering.Universal;
[RequireComponent (typeof(Light2D))]
public class LightWithChanger : MonoBehaviour, IRewindable
{
    private Light2D _light;

    private void Awake()
    {
        _light = GetComponent<Light2D>();
    }
    private void Start()
    {
        TimeRewindManager.Instance.RegisterRewindable(this);
    }
    public void ChangeLight(float intensity)
    {
        _light.intensity = intensity;
    }

    public void SaveState()
    {
        
    }

    public object GetState()
    {
        return new LightWithChangerRewindState
        {
            intensity = _light.intensity
        };
    }

    public void LoadState(object state)
    {
        var savedState = (LightWithChangerRewindState)state;
        _light.intensity = savedState.intensity;
    }

    public void OnStartRewind()
    {
        
    }

    public void OnStopRewind()
    {
        
    }
}
public class LightWithChangerRewindState
{
    public float intensity;
}
