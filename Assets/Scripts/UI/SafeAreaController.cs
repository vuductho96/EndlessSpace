using UnityEngine;

namespace SpaceShooter.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaController : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private Rect _lastSafeArea = Rect.zero;
        private Vector2Int _lastScreenSize = Vector2Int.zero;
        private ScreenOrientation _lastOrientation = ScreenOrientation.AutoRotation;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            // Do not apply to root Canvas
            if (GetComponent<Canvas>() != null) return;
            ApplySafeArea();
        }

        private void Update()
        {
            if (GetComponent<Canvas>() != null) return;

            if (_lastSafeArea != Screen.safeArea || 
                _lastScreenSize.x != Screen.width || 
                _lastScreenSize.y != Screen.height || 
                _lastOrientation != Screen.orientation)
            {
                ApplySafeArea();
            }
        }

        public void ApplySafeArea()
        {
            if (GetComponent<Canvas>() != null) return;
            if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();

            Rect safeArea = Screen.safeArea;
            _lastSafeArea = safeArea;
            _lastScreenSize = new Vector2Int(Screen.width, Screen.height);
            _lastOrientation = Screen.orientation;

            if (Screen.width <= 0 || Screen.height <= 0) return;

            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            _rectTransform.anchorMin = anchorMin;
            _rectTransform.anchorMax = anchorMax;
        }
    }
}
