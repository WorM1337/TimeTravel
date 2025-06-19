using UnityEngine;

public interface IRewindable
{
    void SaveState();
    object GetState();
    void LoadState(object state);
    void OnStartRewind(); // Добавлено
    void OnStopRewind();  // Добавлено
}