using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace SpaceShooter.Editor
{
    public static class GameViewCapture
    {
        [MenuItem("SpaceShooter/Capture High-Res Screenshot (F12) _F12")]
        public static void CaptureScreenshot()
        {
            string screenshotsDir = Path.Combine(Application.dataPath, "Screenshots");
            if (!Directory.Exists(screenshotsDir))
            {
                Directory.CreateDirectory(screenshotsDir);
            }

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string filename = Path.Combine(screenshotsDir, $"Screenshot_{timestamp}.png");

            ScreenCapture.CaptureScreenshot(filename, 1);
            Debug.Log($"<color=#00FFCC><b>[SpaceShooter]</b></color> Screenshot captured to: <b>{filename}</b>");

            AssetDatabase.Refresh();
        }
    }
}
