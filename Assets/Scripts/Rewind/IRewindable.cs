using UnityEngine;

public interface IRewindable
{
    void SaveState();
    object GetState();
    void LoadState(object state);
    void OnStartRewind(); // ���������
    void OnStopRewind();  // ���������
}