using UnityEngine;
using SpaceShooter.Core;

namespace SpaceShooter.Input
{
    public static class HapticFeedback
    {
        public static void TriggerLight()
        {
            if (!IsHapticsEnabled()) return;
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                Handheld.Vibrate();
            }
            catch {}
#endif
        }

        public static void TriggerMedium()
        {
            if (!IsHapticsEnabled()) return;
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                Handheld.Vibrate();
            }
            catch {}
#endif
        }

        public static void TriggerHeavy()
        {
            if (!IsHapticsEnabled()) return;
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                Handheld.Vibrate();
            }
            catch {}
#endif
        }

        private static bool IsHapticsEnabled()
        {
            if (SaveManager.Instance != null && SaveManager.Instance.CurrentSave != null)
            {
                return SaveManager.Instance.CurrentSave.HapticsEnabled;
            }
            return true;
        }
    }
}
