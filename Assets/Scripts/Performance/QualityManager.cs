using UnityEngine;

namespace SpaceShooter.Performance
{
    public enum QualityProfileLevel
    {
        Low = 0,
        Medium = 1,
        High = 2
    }

    public class QualityManager : MonoBehaviour
    {
        public static QualityManager Instance { get; private set; }

        [SerializeField] private QualityProfileLevel _currentProfile = QualityProfileLevel.High;
        public QualityProfileLevel CurrentProfile => _currentProfile;

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

        public void SetQualityProfile(QualityProfileLevel level)
        {
            _currentProfile = level;
            switch (level)
            {
                case QualityProfileLevel.Low:
                    QualitySettings.particleRaycastBudget = 64;
                    QualitySettings.vSyncCount = 0;
                    QualitySettings.maxQueuedFrames = 1;
                    break;
                case QualityProfileLevel.Medium:
                    QualitySettings.particleRaycastBudget = 256;
                    QualitySettings.vSyncCount = 0;
                    QualitySettings.maxQueuedFrames = 2;
                    break;
                case QualityProfileLevel.High:
                    QualitySettings.particleRaycastBudget = 1024;
                    QualitySettings.vSyncCount = 1;
                    QualitySettings.maxQueuedFrames = 2;
                    break;
            }
            Debug.Log($"[QualityManager] Set profile: {level}");
        }
    }
}
