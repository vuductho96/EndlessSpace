using UnityEngine;
using UnityEngine.InputSystem;
using SpaceShooter.Environment;
using System;
using System.IO;

namespace SpaceShooter.UI
{
    public class DeveloperTestControls : MonoBehaviour
    {
        [SerializeField] private DebugPanel _debugPanel;
        [SerializeField] private InfiniteBackground _parallax;

        private void Update()
        {
            if (Keyboard.current == null) return;

            // F11: Toggle Debug Telemetry Panel
            if (Keyboard.current.f11Key.wasPressedThisFrame)
            {
                if (_debugPanel != null) _debugPanel.TogglePanel();
            }

            // F12: Capture In-Engine Screenshot directly to Assets/Screenshots/
            if (Keyboard.current.f12Key.wasPressedThisFrame)
            {
                CaptureInGameScreenshot();
            }
        }

        public static void CaptureInGameScreenshot()
        {
            string screenshotsDir = Path.Combine(Application.dataPath, "Screenshots");
            if (!Directory.Exists(screenshotsDir))
            {
                Directory.CreateDirectory(screenshotsDir);
            }

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string filename = Path.Combine(screenshotsDir, $"Screenshot_{timestamp}.png");

            ScreenCapture.CaptureScreenshot(filename, 1);
            Debug.Log($"<color=#00FFCC><b>[SpaceShooter]</b></color> In-Game Screenshot saved to: <b>{filename}</b>");
        }
    }
}
