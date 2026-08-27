using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace SpaceShooter.Input
{
    /// <summary>
    /// Bulletproof Virtual Joystick for Unity 6 (New Input System & Mobile).
    /// Supports BOTH EventSystem Pointer Events and Direct Hardware Input Polling (Mouse & Touch).
    /// Works 100% reliably in Unity Editor, Device Simulator, and on Android devices.
    /// </summary>
    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [Header("Joystick Visual Components")]
        [SerializeField] private RectTransform _background;
        [SerializeField] private RectTransform _handle;
        [SerializeField] private float _baseRadius = 95f;

        private Canvas _canvas;
        private Camera _uiCamera;
        private bool _isDragging = false;
        private int _activeTouchId = -1;

        private void Awake()
        {
            if (_background == null) _background = GetComponent<RectTransform>();
            CacheCanvas();
        }

        private void Start()
        {
            CacheCanvas();
        }

        private void CacheCanvas()
        {
            if (_canvas == null) _canvas = GetComponentInParent<Canvas>();
            if (_canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                _uiCamera = _canvas.worldCamera;
            }
            else
            {
                _uiCamera = null;
            }
        }

        public void Setup(RectTransform bg, RectTransform handle, float radius = 95f)
        {
            _background = bg;
            _handle = handle;
            _baseRadius = radius;
            if (_background == null) _background = GetComponent<RectTransform>();
            CacheCanvas();
        }

        // ============================================================
        // 1. EVENT SYSTEM INTERFACE HANDLERS (uGUI)
        // ============================================================
        public void OnPointerDown(PointerEventData eventData)
        {
            _isDragging = true;
            UpdateJoystickPosition(eventData.position);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;
            UpdateJoystickPosition(eventData.position);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isDragging = false;
            _activeTouchId = -1;
            ResetJoystick();
        }

        // ============================================================
        // 2. DIRECT HARDWARE INPUT POLLING (Mouse & Touch Fallback)
        // ============================================================
        private void Update()
        {
            CacheCanvas();
            if (_background == null) return;

            // MOUSE INPUT (Editor Game View / Simulator / PC)
            var mouse = Mouse.current;
            if (mouse != null)
            {
                Vector2 mousePos = mouse.position.ReadValue();

                if (mouse.leftButton.wasPressedThisFrame)
                {
                    if (IsScreenPointInsideJoystick(mousePos))
                    {
                        _isDragging = true;
                        UpdateJoystickPosition(mousePos);
                    }
                }
                else if (mouse.leftButton.isPressed && _isDragging && _activeTouchId == -1)
                {
                    UpdateJoystickPosition(mousePos);
                }
                else if (mouse.leftButton.wasReleasedThisFrame && _isDragging && _activeTouchId == -1)
                {
                    _isDragging = false;
                    ResetJoystick();
                }
            }

            // TOUCHSCREEN INPUT (Android / Simulator Touch)
            var ts = Touchscreen.current;
            if (ts != null)
            {
                var primary = ts.primaryTouch;
                if (primary.press.wasPressedThisFrame)
                {
                    Vector2 touchPos = primary.position.ReadValue();
                    if (IsScreenPointInsideJoystick(touchPos))
                    {
                        _isDragging = true;
                        _activeTouchId = 1;
                        UpdateJoystickPosition(touchPos);
                    }
                }
                else if (primary.press.isPressed && _isDragging && _activeTouchId == 1)
                {
                    Vector2 touchPos = primary.position.ReadValue();
                    UpdateJoystickPosition(touchPos);
                }
                else if (primary.press.wasReleasedThisFrame && _isDragging && _activeTouchId == 1)
                {
                    _isDragging = false;
                    _activeTouchId = -1;
                    ResetJoystick();
                }
            }
        }

        private bool IsScreenPointInsideJoystick(Vector2 screenPos)
        {
            if (_background == null) return false;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_background, screenPos, _uiCamera, out Vector2 localPoint))
            {
                return localPoint.magnitude <= _baseRadius * 1.5f;
            }
            return false;
        }

        private void UpdateJoystickPosition(Vector2 screenPosition)
        {
            if (_background == null) return;

            CacheCanvas();
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_background, screenPosition, _uiCamera, out Vector2 localPoint))
            {
                Vector2 clamped = Vector2.ClampMagnitude(localPoint, _baseRadius);

                if (_handle != null)
                {
                    _handle.anchoredPosition = clamped;
                }

                Vector2 inputVector = clamped / (_baseRadius > 0.001f ? _baseRadius : 1f);
                if (InputManager.Instance != null)
                {
                    InputManager.Instance.TouchMoveVector = inputVector;
                }
            }
        }

        private void ResetJoystick()
        {
            if (_handle != null)
            {
                _handle.anchoredPosition = Vector2.zero;
            }

            if (InputManager.Instance != null)
            {
                InputManager.Instance.TouchMoveVector = Vector2.zero;
            }
        }
    }
}
