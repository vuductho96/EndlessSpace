using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using SpaceShooter.Input;

namespace SpaceShooter.UI
{
    public enum ActionButtonType
    {
        Fire,
        Boost,
        Ability
    }

    /// <summary>
    /// Dual-mode action button supporting EventSystem Pointer Events and Direct Mouse/Touch fallback.
    /// Guarantees instant press and release response with visual feedback.
    /// </summary>
    public class TouchActionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] private ActionButtonType _actionType = ActionButtonType.Fire;
        [SerializeField] private float _pressedScale = 0.88f;

        private RectTransform _rectTransform;
        private Canvas _canvas;
        private Camera _uiCamera;
        private Vector3 _originalScale = Vector3.one;
        private bool _isPressed = false;
        private bool _isPointerHandling = false;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _originalScale = transform.localScale;
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

        public void Setup(ActionButtonType type)
        {
            _actionType = type;
            if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
            _originalScale = transform.localScale;
            CacheCanvas();
        }

        private void OnDisable()
        {
            if (_isPressed)
            {
                SetPressed(false);
            }
        }

        // ============================================================
        // 1. EVENT SYSTEM INTERFACE HANDLERS (uGUI)
        // ============================================================
        public void OnPointerDown(PointerEventData eventData)
        {
            _isPointerHandling = true;
            SetPressed(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isPointerHandling = false;
            SetPressed(false);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isPointerHandling = false;
            SetPressed(false);
        }

        // ============================================================
        // 2. DIRECT HARDWARE INPUT POLLING (Mouse & Touch Fallback)
        // ============================================================
        private void Update()
        {
            if (_isPointerHandling) return;

            CacheCanvas();
            if (_rectTransform == null) return;

            // Direct Mouse Check
            var mouse = Mouse.current;
            if (mouse != null)
            {
                Vector2 mousePos = mouse.position.ReadValue();
                if (mouse.leftButton.wasPressedThisFrame)
                {
                    if (IsScreenPointInsideButton(mousePos))
                    {
                        SetPressed(true);
                    }
                }
                else if (_isPressed && mouse.leftButton.wasReleasedThisFrame)
                {
                    SetPressed(false);
                }
                else if (_isPressed && !mouse.leftButton.isPressed)
                {
                    SetPressed(false);
                }
            }

            // Direct Touch Check
            var ts = Touchscreen.current;
            if (ts != null)
            {
                var primary = ts.primaryTouch;
                if (primary.press.wasPressedThisFrame)
                {
                    Vector2 touchPos = primary.position.ReadValue();
                    if (IsScreenPointInsideButton(touchPos))
                    {
                        SetPressed(true);
                    }
                }
                else if (_isPressed && primary.press.wasReleasedThisFrame)
                {
                    SetPressed(false);
                }
                else if (_isPressed && !primary.press.isPressed)
                {
                    SetPressed(false);
                }
            }
        }

        private bool IsScreenPointInsideButton(Vector2 screenPos)
        {
            if (_rectTransform == null) return false;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, screenPos, _uiCamera, out Vector2 localPoint))
            {
                return _rectTransform.rect.Contains(localPoint);
            }
            return false;
        }

        private void SetPressed(bool pressed)
        {
            _isPressed = pressed;
            transform.localScale = pressed ? _originalScale * _pressedScale : _originalScale;

            if (InputManager.Instance == null) return;

            switch (_actionType)
            {
                case ActionButtonType.Fire:
                    InputManager.Instance.TouchFirePressed = pressed;
                    break;
                case ActionButtonType.Boost:
                    InputManager.Instance.TouchBoostPressed = pressed;
                    break;
                case ActionButtonType.Ability:
                    InputManager.Instance.TouchAbilityPressed = pressed;
                    break;
            }
        }
    }
}
