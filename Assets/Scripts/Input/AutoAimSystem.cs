using UnityEngine;
using SpaceShooter.Core;
using SpaceShooter.Combat;

namespace SpaceShooter.Input
{
    public enum AimAssistMode
    {
        Off = 0,
        Low = 1,
        Medium = 2,
        High = 3
    }

    public class AutoAimSystem : MonoBehaviour
    {
        public static AutoAimSystem Instance { get; private set; }

        [Header("Aim Assist Settings")]
        [SerializeField] private float _detectionRadius = 18f;
        [SerializeField] private float _maxAssistAngle = 45f;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public Vector2 AdjustAim(Vector2 playerPos, Vector2 intendedAimDir)
        {
            int assistLevel = 2;
            if (SaveManager.Instance != null && SaveManager.Instance.CurrentSave != null)
            {
                assistLevel = SaveManager.Instance.CurrentSave.AimAssistLevel;
            }

            if (assistLevel == 0 || intendedAimDir.sqrMagnitude < 0.01f)
            {
                return intendedAimDir;
            }

            // Find closest training target / enemy in front of player
            Collider2D[] hits = Physics2D.OverlapCircleAll(playerPos, _detectionRadius);
            Transform bestTarget = null;
            float bestDot = -1f;

            foreach (var col in hits)
            {
                if (col.CompareTag("Player")) continue;
                if (col.GetComponent<IDamageable>() == null) continue;

                Vector2 toTarget = ((Vector2)col.transform.position - playerPos).normalized;
                float dot = Vector2.Dot(intendedAimDir.normalized, toTarget);
                float angle = Vector2.Angle(intendedAimDir, toTarget);

                float allowedAngle = assistLevel switch
                {
                    1 => _maxAssistAngle * 0.35f,
                    2 => _maxAssistAngle * 0.70f,
                    3 => _maxAssistAngle * 1.15f,
                    _ => _maxAssistAngle * 0.70f
                };

                if (angle <= allowedAngle && dot > bestDot)
                {
                    bestDot = dot;
                    bestTarget = col.transform;
                }
            }

            if (bestTarget != null)
            {
                Vector2 targetDir = ((Vector2)bestTarget.position - playerPos).normalized;
                float blendFactor = assistLevel switch
                {
                    1 => 0.35f,
                    2 => 0.65f,
                    3 => 0.90f,
                    _ => 0.65f
                };
                return Vector2.Lerp(intendedAimDir, targetDir, blendFactor).normalized;
            }

            return intendedAimDir;
        }
    }
}
