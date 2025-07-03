using UnityEngine;

public interface IMovingPlatform
{
    Transform FirstParent { get; set; }
    void SetParent(Transform newParent);
}
