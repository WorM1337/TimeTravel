using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeRewindManager : MonoBehaviour
{
    public static TimeRewindManager Instance { get; private set; }
    public bool IsRewinding { get; private set; }
    private List<IRewindable> rewindables = new List<IRewindable>();
    private List<object[]> states = new List<object[]>();
    private float recordInterval = 0.02f;
    [SerializeField] public float RewindDuration = 3f;
    private float recordTimer = 0f;
    private int maxStates;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
            // я решил убрать, потому что между сценами у нас измен€ютс€ объекты, а в states у нас
            // есть инфа только по предыдущим объектам
        }
        else
        {
            Destroy(gameObject);
        }
        maxStates = Mathf.CeilToInt(RewindDuration / recordInterval);
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
    //ќбнаружил, что сама логика unregister не совсем верна - если, мы отписываемс€ от этого объекта,
    // то мы получим ошибки, при попытке rewind в промежуток времени, меньший, чем наибольшоее врем€,
    // которое можно делать rewind - у нас в states останутьс€ состо€ни€ наших отписавшихс€
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
        states.Add(currentStates);

        while (states.Count > maxStates)
        {
            states.RemoveAt(0);
        }
    }

    public void Rewind()
    {
        Debug.Log($"Rewind. Count of states: {states.Count}");
        if (states.Count > 0)
        {
            object[] previousStates = states[states.Count - 1];
            states.RemoveAt(states.Count - 1);
            for (int i = 0; i < rewindables.Count; i++)
            {
                rewindables[i].LoadState(previousStates[i]);
            }
        }
        else
        {
            StopRewind();
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
    public void ClearStates()
    {
        states.Clear();
        Debug.Log(states.Count);
    }
}