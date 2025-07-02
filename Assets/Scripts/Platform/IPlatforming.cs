using UnityEngine;

public interface IPlatforming
{
    void CheckPlatformDown();
    void GetDown(Platform platform);
    void ClearActivePlatforms(Platform current);
    void ForbidCollisionForAllPlatforms();
}
