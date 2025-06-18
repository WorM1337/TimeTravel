using System.Collections.Generic;
using UnityEngine;

public class TimeRewindManager : MonoBehaviour
{
    public static TimeRewindManager Instance { get; private set; }
    public bool IsRewinding { get; private set; }
    private List<IRewindable> rewindables = new List<IRewindable>();
    private Stack<object[]> states = new Stack<object[]>();
    private float recordInterval = 0.02f;
    [SerializeField] public float RewindDuration = 3f; // ѕубличное свойство дл€ инспектора
    private float recordTimer = 0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        if (!IsRewinding)
        {
            recordTimer += Time.fixedDeltaTime;
            if (recordTimer >= recordInterval)
            {
                recordTimer = 0f;
                SaveStates();
            }
        }
    }

    public void RegisterRewindable(IRewindable rewindable)
    {
        rewindables.Add(rewindable);
    }

    public void UnregisterRewindable(IRewindable rewindable)
    {
        rewindables.Remove(rewindable);
    }

    private void SaveStates()
    {
        object[] currentStates = new object[rewindables.Count];
        for (int i = 0; i < rewindables.Count; i++)
        {
            rewindables[i].SaveState();
            currentStates[i] = rewindables[i].GetState();
        }
        states.Push(currentStates);

        // ”дал€ем старые состо€ни€, чтобы ограничить длительность
        while (states.Count > Mathf.CeilToInt(RewindDuration / recordInterval))
        {
            states.TryPop(out _);
        }
    }

    public void Rewind()
    {
        if (states.Count > 0)
        {
            object[] previousStates = states.Pop();
            for (int i = 0; i < rewindables.Count; i++)
            {
                rewindables[i].LoadState(previousStates[i]);
            }
        }
        else
        {
            StopRewind(); // ѕринудительно останавливаем, если состо€ни€ закончились
        }
    }

    public void StartRewind()
    {
        IsRewinding = true;
        foreach (var rewindable in rewindables)
        {
            rewindable.OnStartRewind();
        }
    }

    public void StopRewind()
    {
        IsRewinding = false;
        foreach (var rewindable in rewindables)
        {
            rewindable.OnStopRewind();
        }
    }
}