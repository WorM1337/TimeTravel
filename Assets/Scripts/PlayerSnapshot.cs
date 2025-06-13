using UnityEngine;

[System.Serializable]
public class PlayerSnapshot
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector2 velocity;

    public PlayerSnapshot(Vector3 pos, Quaternion rot, Vector2 vel)
    {
        position = pos;
        rotation = rot;
        velocity = vel;
    }
}
