using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace SpaceShooter.Editor
{
    [InitializeOnLoad]
    public static class AutoScreenshotServer
    {
        private static string TriggerPath => Path.Combine(Application.dataPath, "Screenshots", ".trigger_capture");
        private static string DonePath => Path.Combine(Application.dataPath, "Screenshots", ".capture_done");
        private static string LatestPath => Path.Combine(Application.dataPath, "Screenshots", "auto_screenshot_latest.png");

        static AutoScreenshotServer()
        {
            EditorApplication.update += CheckForCaptureTrigger;
        }

        private static void CheckForCaptureTrigger()
        {
            if (!File.Exists(TriggerPath)) return;

            try
            {
                File.Delete(TriggerPath);
                PerformCapture();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AutoScreenshotServer] Error during auto capture: {ex.Message}");
            }
        }

        public static void PerformCapture()
        {
            string screenshotsDir = Path.Combine(Application.dataPath, "Screenshots");
            if (!Directory.Exists(screenshotsDir))
            {
                Directory.CreateDirectory(screenshotsDir);
            }

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string timestampedPath = Path.Combine(screenshotsDir, $"Screenshot_{timestamp}.png");

            if (Application.isPlaying)
            {
                ScreenCapture.CaptureScreenshot(LatestPath, 1);
                ScreenCapture.CaptureScreenshot(timestampedPath, 1);
                Debug.Log($"<color=#00FFCC><b>[AutoScreenshotServer]</b></color> Captured Play-Mode frame to: <b>{LatestPath}</b>");
            }
            else
            {
                // Ensure objects exist before capturing in Edit mode
                EditorSceneBuilder.BakeCurrentActiveScene();

                UnityEngine.Camera cam = UnityEngine.Camera.main;
                if (cam != null)
                {
                    int w = 2048;
                    int h = 2732;
                    RenderTexture rt = new RenderTexture(w, h, 24);
                    RenderTexture prevRt = cam.targetTexture;
                    RenderTexture prevActive = RenderTexture.active;

                    cam.targetTexture = rt;
                    cam.Render();

                    RenderTexture.active = rt;
                    Texture2D screenShot = new Texture2D(w, h, TextureFormat.RGB24, false);
                    screenShot.ReadPixels(new Rect(0, 0, w, h), 0, 0);
                    screenShot.Apply();

                    cam.targetTexture = prevRt;
                    RenderTexture.active = prevActive;
                    rt.Release();

                    byte[] bytes = screenShot.EncodeToPNG();
                    File.WriteAllBytes(LatestPath, bytes);
                    File.WriteAllBytes(timestampedPath, bytes);
                    Debug.Log($"<color=#00FFCC><b>[AutoScreenshotServer]</b></color> Rendered Camera frame to: <b>{LatestPath}</b>");
                }
            }

            File.WriteAllText(DonePath, timestampedPath);
            AssetDatabase.Refresh();
        }
    }
}
