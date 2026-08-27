using UnityEngine;

namespace SpaceShooter.Performance
{
    public class PerformanceManager : MonoBehaviour
    {
        public static PerformanceManager Instance { get; private set; }

        public float CurrentFPS { get; private set; } = 60f;
        public float FrameTimeMs { get; private set; } = 16.6f;

        private float _fpsAccumulator = 0f;
        private int _fpsFrames = 0;
        private float _fpsTimeLeft = 0.5f;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            _fpsTimeLeft -= Time.unscaledDeltaTime;
            _fpsAccumulator += Time.unscaledDeltaTime;
            _fpsFrames++;

            if (_fpsTimeLeft <= 0f)
            {
                CurrentFPS = _fpsFrames / _fpsAccumulator;
                FrameTimeMs = (_fpsAccumulator / _fpsFrames) * 1000f;
                _fpsTimeLeft = 0.5f;
                _fpsAccumulator = 0f;
                _fpsFrames = 0;
            }
        }
    }
}
