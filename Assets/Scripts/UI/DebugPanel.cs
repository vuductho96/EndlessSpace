using UnityEngine;
using UnityEngine.UI;
using SpaceShooter.Cameras;
using SpaceShooter.Player;
using SpaceShooter.Combat;

namespace SpaceShooter.UI
{
    public class DebugPanel : MonoBehaviour
    {
        [Header("UI Reference")]
        [SerializeField] private GameObject _panelRoot;
        [SerializeField] private Text _debugText;

        private float _fpsAccumulator;
        private int _fpsFrames;
        private float _currentFps;
        private float _fpsNextPeriod;

        private void Start()
        {
            _fpsNextPeriod = Time.realtimeSinceStartup + 0.5f;
        }

        private void Update()
        {
            _fpsAccumulator += Time.timeScale / Time.deltaTime;
            _fpsFrames++;

            if (Time.realtimeSinceStartup > _fpsNextPeriod)
            {
                _currentFps = _fpsAccumulator / _fpsFrames;
                _fpsAccumulator = 0f;
                _fpsFrames = 0;
                _fpsNextPeriod = Time.realtimeSinceStartup + 0.5f;
            }

            UpdateTelemetry();
        }

        private void UpdateTelemetry()
        {
            if (_debugText == null) return;

            float aspect = (float)Screen.width / Screen.height;
            string orientation = Screen.height > Screen.width ? "Portrait" : "Landscape";

            var camCtrl = GameplayCameraController.Instance ?? FindAnyObjectByType<GameplayCameraController>();
            float orthoSize = camCtrl != null ? camCtrl.OrthographicSize : (UnityEngine.Camera.main != null ? UnityEngine.Camera.main.orthographicSize : 0f);
            float camAspect = camCtrl != null ? camCtrl.CameraAspect : aspect;
            float worldW = camCtrl != null ? camCtrl.WorldVisibleWidth : orthoSize * 2f * aspect;
            float worldH = camCtrl != null ? camCtrl.WorldVisibleHeight : orthoSize * 2f;

            var player = GameObject.FindGameObjectWithTag("Player");
            float playerWorldScale = player != null ? player.transform.lossyScale.x : 0.3f;

            // Calculate Player Screen Size in pixels and % of screen
            float playerScreenPixels = 0f;
            float playerScreenPercent = 0f;
            if (player != null && UnityEngine.Camera.main != null && worldH > 0f)
            {
                // In world units, ship height is ~4.0 * lossyScale.x
                float shipWorldHeight = 4.0f * playerWorldScale;
                float pixelsPerWorldUnit = Screen.height / worldH;
                playerScreenPixels = shipWorldHeight * pixelsPerWorldUnit;
                playerScreenPercent = (playerScreenPixels / Screen.height) * 100f;
            }

            var enemy = FindAnyObjectByType<TrainingTarget>();
            float enemyWorldScale = enemy != null ? enemy.transform.lossyScale.x : 0.35f;

            string text =
                $"=== ANDROID CAMERA & WORLD TELEMETRY ===\n" +
                $"Screen: {Screen.width} x {Screen.height}\n" +
                $"Aspect: {aspect:F3}\n" +
                $"Orientation: {orientation}\n" +
                $"Camera Orthographic Size: {orthoSize:F2}\n" +
                $"Camera Aspect: {camAspect:F3}\n" +
                $"World Visible Width: {worldW:F2}\n" +
                $"World Visible Height: {worldH:F2}\n" +
                $"Player World Scale: {playerWorldScale:F3}\n" +
                $"Player Screen Size: {playerScreenPixels:F0} px ({playerScreenPercent:F1}% screen height)\n" +
                $"Enemy World Scale: {enemyWorldScale:F3}\n" +
                $"FPS: {_currentFps:F0}";

            _debugText.text = text;
        }

        public void TogglePanel()
        {
            if (_panelRoot != null)
            {
                _panelRoot.SetActive(!_panelRoot.activeSelf);
            }
        }
    }
}
