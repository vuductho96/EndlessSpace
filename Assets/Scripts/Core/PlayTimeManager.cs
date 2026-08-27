using System;
using UnityEngine;

namespace SpaceShooter.Core
{
    /// <summary>
    /// Accurately tracks active in-game combat gameplay time.
    /// Only accumulates seconds during GameState.Playing (strictly excludes menus, hangar, pause, and loading).
    /// </summary>
    public class PlayTimeManager : MonoBehaviour
    {
        public static PlayTimeManager Instance { get; private set; }

        public event Action<float> OnPlayTimeUpdated;

        private float _sessionAccumulator = 0f;
        private float _saveIntervalTimer = 0f;
        private const float SAVE_INTERVAL = 10f; // Flush to disk every 10s of active combat

        public float TotalActivePlayTimeSeconds
        {
            get
            {
                if (SaveManager.Instance != null && SaveManager.Instance.CurrentSave != null)
                {
                    return SaveManager.Instance.CurrentSave.TotalActivePlayTimeSeconds + _sessionAccumulator;
                }
                return _sessionAccumulator;
            }
        }

        public float TotalActivePlayTimeMinutes => TotalActivePlayTimeSeconds / 60f;

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
            // Only count active gameplay
            bool isPlaying = GameStateManager.Instance != null &&
                             GameStateManager.Instance.CurrentState == GameState.Playing &&
                             Time.timeScale > 0.01f;

            if (isPlaying)
            {
                float delta = Time.unscaledDeltaTime;
                _sessionAccumulator += delta;
                _saveIntervalTimer += delta;

                if (_saveIntervalTimer >= SAVE_INTERVAL)
                {
                    FlushSessionTime();
                }

                OnPlayTimeUpdated?.Invoke(TotalActivePlayTimeSeconds);
            }
        }

        public void FlushSessionTime()
        {
            if (_sessionAccumulator > 0f && SaveManager.Instance != null && SaveManager.Instance.CurrentSave != null)
            {
                SaveManager.Instance.AddActivePlayTime(_sessionAccumulator);
                _sessionAccumulator = 0f;
                _saveIntervalTimer = 0f;
            }
        }

        public float GetRemainingPlayTimeMinutes(float requiredMinutes)
        {
            float requiredSeconds = requiredMinutes * 60f;
            float remaining = Mathf.Max(0f, requiredSeconds - TotalActivePlayTimeSeconds);
            return remaining / 60f;
        }

        public float GetRemainingPlayTimeSeconds(float requiredMinutes)
        {
            float requiredSeconds = requiredMinutes * 60f;
            return Mathf.Max(0f, requiredSeconds - TotalActivePlayTimeSeconds);
        }

        public string GetRemainingPlayTimeString(float requiredMinutes)
        {
            float rem = GetRemainingPlayTimeSeconds(requiredMinutes);
            int minutes = Mathf.FloorToInt(rem / 60f);
            int seconds = Mathf.FloorToInt(rem % 60f);
            return $"{minutes:00}:{seconds:00}";
        }

        public bool IsPlayTimeMet(float requiredMinutes)
        {
            return TotalActivePlayTimeSeconds >= (requiredMinutes * 60f);
        }

        /// <summary>
        /// Developer simulation method to add active play time for testing.
        /// </summary>
        public void SimulateAddPlayTime(float minutes)
        {
            float seconds = minutes * 60f;
            if (SaveManager.Instance != null && SaveManager.Instance.CurrentSave != null)
            {
                SaveManager.Instance.AddActivePlayTime(seconds);
                SaveManager.Instance.SaveGame();
            }
            else
            {
                _sessionAccumulator += seconds;
            }
            OnPlayTimeUpdated?.Invoke(TotalActivePlayTimeSeconds);
            Debug.Log($"<color=#00FFCC><b>[PlayTimeManager]</b></color> Simulated +{minutes:F1} mins. Total Active: {TotalActivePlayTimeMinutes:F2} mins ({TotalActivePlayTimeSeconds:F0}s)");
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                FlushSessionTime();
            }
        }

        private void OnApplicationQuit()
        {
            FlushSessionTime();
        }
    }
}
