using UnityEngine;
using SpaceShooter.Performance;

namespace SpaceShooter.Core
{
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance { get; private set; }

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

        private void Start()
        {
            ApplySettingsFromSave();
        }

        public void ApplySettingsFromSave()
        {
            if (SaveManager.Instance == null || SaveManager.Instance.CurrentSave == null) return;
            var save = SaveManager.Instance.CurrentSave;

            // 1. Frame Rate
            Application.targetFrameRate = save.TargetFps;

            // 2. Quality Profile
            if (QualityManager.Instance != null)
            {
                QualityManager.Instance.SetQualityProfile((QualityProfileLevel)save.QualityLevel);
            }

            // 3. Audio
            AudioListener.volume = save.MasterVolume;

            Debug.Log($"[SettingsManager] Applied settings: FPS={save.TargetFps}, Quality={save.QualityLevel}, Haptics={save.HapticsEnabled}");
        }

        public void SetTargetFps(int fps)
        {
            fps = (fps == 30) ? 30 : 60;
            Application.targetFrameRate = fps;
            if (SaveManager.Instance != null && SaveManager.Instance.CurrentSave != null)
            {
                SaveManager.Instance.CurrentSave.TargetFps = fps;
                SaveManager.Instance.SaveGame();
            }
        }

        public void SetHapticsEnabled(bool enabled)
        {
            if (SaveManager.Instance != null && SaveManager.Instance.CurrentSave != null)
            {
                SaveManager.Instance.CurrentSave.HapticsEnabled = enabled;
                SaveManager.Instance.SaveGame();
            }
        }
    }
}
