using UnityEngine;
using UnityEngine.UI;

namespace SpaceShooter.UI
{
    public class ResponsiveUIManager : MonoBehaviour
    {
        [SerializeField] private CanvasScaler _canvasScaler;
        [SerializeField] private RectTransform _landscapeLayoutRoot;
        [SerializeField] private RectTransform _portraitLayoutRoot;

        private bool _isPortrait = false;
        public bool IsPortrait => _isPortrait;

        private void Awake()
        {
            if (_canvasScaler == null) _canvasScaler = GetComponent<CanvasScaler>();
            UpdateLayout();
        }

        private void Update()
        {
            bool currentPortrait = Screen.height > Screen.width;
            if (currentPortrait != _isPortrait)
            {
                _isPortrait = currentPortrait;
                UpdateLayout();
            }
        }

        public void UpdateLayout()
        {
            _isPortrait = Screen.height > Screen.width;

            if (_canvasScaler != null)
            {
                _canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                _canvasScaler.referenceResolution = _isPortrait ? new Vector2(1080, 1920) : new Vector2(1920, 1080);
                _canvasScaler.matchWidthOrHeight = 0.5f;
            }

            if (_landscapeLayoutRoot != null) _landscapeLayoutRoot.gameObject.SetActive(!_isPortrait);
            if (_portraitLayoutRoot != null) _portraitLayoutRoot.gameObject.SetActive(_isPortrait);

            Debug.Log($"[ResponsiveUIManager] Reflowed UI layout to: {(_isPortrait ? "PORTRAIT" : "LANDSCAPE PRIMARY")}");
        }
    }
}
