using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SpaceShooter.Input
{
    /// <summary>
    /// Robust, resolution-independent virtual joystick and touch controls for Android.
    /// Supports both fixed joystick touch and dynamic left-screen touch zone.
    /// </summary>
    public class TouchInputController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [Header("Joystick References")]
        [SerializeField] private RectTransform _joystickBackground;
        [SerializeField] private RectTransform _joystickHandle;
        [SerializeField] private float _handleRadius = 75f;
        [SerializeField] private float _deadZone = 8f;

        private Vector2 _defaultBgPosition;
        private Canvas _canvas;
        private Camera _uiCamera;
        private bool _isDragging;
        private int _activePointerId = -999;

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
            if (_canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                _uiCamera = _canvas.worldCamera;
            }
        }

        private void Start()
        {
            if (_joystickBackground != null)
            {
                _defaultBgPosition = _joystickBackground.anchoredPosition;
            }
            ApplyCustomization();
        }

        public void SetupReferences(RectTransform bg, RectTransform handle, float radius = 75f)
        {
            _joystickBackground = bg;
            _joystickHandle = handle;
            _handleRadius = radius;
            if (_joystickBackground != null)
            {
                _defaultBgPosition = _joystickBackground.anchoredPosition;
            }
        }

        public void ApplyCustomization()
        {
            float sizeMult = 1.0f;
            float opacity = 0.85f;

            if (Core.SaveManager.Instance != null && Core.SaveManager.Instance.CurrentSave != null)
            {
                sizeMult = Core.SaveManager.Instance.CurrentSave.JoystickSize;
                opacity = Core.SaveManager.Instance.CurrentSave.JoystickOpacity;
            }

            if (_joystickBackground != null)
            {
                _joystickBackground.localScale = Vector3.one * sizeMult;
                var imgs = _joystickBackground.GetComponentsInChildren<Image>();
                foreach (var img in imgs)
                {
                    Color c = img.color;
                    c.a = opacity;
                    img.color = c;
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // Only process left-side touches for movement joystick
            if (_isDragging) return;

            _isDragging = true;
            _activePointerId = eventData.pointerId;

            if (_joystickBackground != null)
            {
                // Convert screen point to local point within parent (TouchOverlay)
                RectTransform parentRt = _joystickBackground.parent as RectTransform;
                if (parentRt != null && RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRt, eventData.position, _uiCamera, out Vector2 localPoint))
                {
                    // Check if touch is near default position or in left zone
                    if (eventData.position.x < Screen.width * 0.55f)
                    {
                        _joystickBackground.anchoredPosition = localPoint;
                    }
                }
            }

            ProcessJoystickMovement(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging || eventData.pointerId != _activePointerId) return;
            ProcessJoystickMovement(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId != _activePointerId) return;

            _isDragging = false;
            _activePointerId = -999;

            if (_joystickHandle != null)
            {
                _joystickHandle.anchoredPosition = Vector2.zero;
            }

            if (_joystickBackground != null)
            {
                _joystickBackground.anchoredPosition = _defaultBgPosition;
            }

            if (InputManager.Instance != null)
            {
                InputManager.Instance.TouchMoveVector = Vector2.zero;
            }
        }

        private void ProcessJoystickMovement(PointerEventData eventData)
        {
            if (_joystickBackground == null) return;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_joystickBackground, eventData.position, _uiCamera, out Vector2 localPoint))
            {
                float dist = localPoint.magnitude;
                if (dist < _deadZone)
                {
                    if (_joystickHandle != null) _joystickHandle.anchoredPosition = Vector2.zero;
                    if (InputManager.Instance != null) InputManager.Instance.TouchMoveVector = Vector2.zero;
                    return;
                }

                Vector2 clamped = Vector2.ClampMagnitude(localPoint, _handleRadius);
                if (_joystickHandle != null)
                {
                    _joystickHandle.anchoredPosition = clamped;
                }

                Vector2 normalizedVector = clamped / _handleRadius;
                if (InputManager.Instance != null)
                {
                    InputManager.Instance.TouchMoveVector = normalizedVector;
                }
            }
        }

        // Discrete Action Buttons
        public void SetFirePressed(bool pressed)
        {
            if (InputManager.Instance != null) InputManager.Instance.TouchFirePressed = pressed;
        }

        public void SetBoostPressed(bool pressed)
        {
            if (InputManager.Instance != null) InputManager.Instance.TouchBoostPressed = pressed;
        }

        public void SetAbilityPressed(bool pressed)
        {
            if (InputManager.Instance != null) InputManager.Instance.TouchAbilityPressed = pressed;
        }
    }
}
