using UnityEngine;

namespace SpaceShooter.Cameras
{
    /// <summary>
    /// Legacy wrapper that bridges to GameplayCameraController.
    /// </summary>
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class SpaceCamera : GameplayCameraController
    {
    }
}
