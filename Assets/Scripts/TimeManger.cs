using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public enum TimeSpeed
{
    VerySlow,
    Slow,
    Normal,
    Fast,
    VeryFast
}

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;

    public TimeSpeed CurrentTimeSpeed = TimeSpeed.Normal;
    public float SlowFactor = 1f;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EditTimeSpeed(TimeSpeed timeSpeed)
    {

        switch (timeSpeed)
        {
            case TimeSpeed.VerySlow:
                Time.timeScale = SlowFactor = 0.2f;
                break;
            case TimeSpeed.Slow:
                Time.timeScale = SlowFactor = 0.5f;
                break;
            case TimeSpeed.Normal:
                Time.timeScale = SlowFactor = 1f;
                break;
            case TimeSpeed.Fast:
                Time.timeScale = SlowFactor = 1.5f;
                break;
            case TimeSpeed.VeryFast:
                Time.timeScale = SlowFactor = 1.8f;
                break;
        }
        CurrentTimeSpeed = timeSpeed;
    }
}
