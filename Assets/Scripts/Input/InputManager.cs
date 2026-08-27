using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace SpaceShooter.Input
{
    public enum ActiveInputDevice
    {
        Touch,
        Gamepad,
        KeyboardMouse
    }

    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        [Header("Telemetry")]
        [SerializeField] private ActiveInputDevice _activeDevice = ActiveInputDevice.Touch;
        public ActiveInputDevice ActiveDevice => _activeDevice;

        public Vector2 MoveInput { get; private set; }
        public Vector2 MovementInput => MoveInput;
        public Vector2 AimDirection { get; private set; } = Vector2.up;
        public Vector3 AimWorldPosition { get; private set; }
        public bool IsFiring { get; private set; }
        public bool IsBoosting { get; private set; }
        public bool IsAbilityTriggered { get; private set; }
        public bool IsPauseTriggered { get; private set; }

        // Touch & Virtual Control Inputs
        public Vector2 TouchMoveVector { get; set; }
        public Vector2 TouchAimVector { get; set; }
        public bool TouchFirePressed { get; set; }
        public bool TouchBoostPressed { get; set; }
        public bool TouchAbilityPressed { get; set; }

        private UnityEngine.Camera _mainCam;
        private Vector2 _lastMousePos;
        private bool _hasMouseMoved = false;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            _mainCam = UnityEngine.Camera.main;
            AimDirection = Vector2.up;
        }

        private void Update()
        {
            if (_mainCam == null) _mainCam = UnityEngine.Camera.main;

            var kb = Keyboard.current;
            var mouse = Mouse.current;
            var pad = Gamepad.current;

            bool isPointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

            // 1. MOVEMENT PROCESSING
            if (TouchMoveVector.sqrMagnitude > 0.01f)
            {
                MoveInput = Vector2.ClampMagnitude(TouchMoveVector, 1f);
                _activeDevice = ActiveInputDevice.Touch;
            }
            else if (pad != null && pad.leftStick.ReadValue().sqrMagnitude > 0.05f)
            {
                MoveInput = Vector2.ClampMagnitude(pad.leftStick.ReadValue(), 1f);
                _activeDevice = ActiveInputDevice.Gamepad;
            }
            else
            {
                Vector2 kbMove = Vector2.zero;
                if (kb != null)
                {
                    if (kb.wKey.isPressed || kb.upArrowKey.isPressed) kbMove.y += 1f;
                    if (kb.sKey.isPressed || kb.downArrowKey.isPressed) kbMove.y -= 1f;
                    if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) kbMove.x += 1f;
                    if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) kbMove.x -= 1f;
                }
                MoveInput = Vector2.ClampMagnitude(kbMove, 1f);
                if (MoveInput.sqrMagnitude > 0.01f) _activeDevice = ActiveInputDevice.KeyboardMouse;
            }

            // 2. AIMING PROCESSING
            Vector2 rawAim = AimDirection;

            if (TouchAimVector.sqrMagnitude > 0.1f)
            {
                rawAim = TouchAimVector.normalized;
                _activeDevice = ActiveInputDevice.Touch;
            }
            else if (pad != null && pad.rightStick.ReadValue().sqrMagnitude > 0.15f)
            {
                rawAim = pad.rightStick.ReadValue().normalized;
                _activeDevice = ActiveInputDevice.Gamepad;
            }
            else if (_activeDevice == ActiveInputDevice.Touch && MoveInput.sqrMagnitude > 0.01f)
            {
                // When navigating with Virtual Joystick, align ship facing with movement
                rawAim = MoveInput.normalized;
            }
            else if (mouse != null && _mainCam != null && !isPointerOverUI)
            {
                Vector2 curMouse = mouse.position.ReadValue();
                if ((curMouse - _lastMousePos).sqrMagnitude > 2f) _hasMouseMoved = true;
                _lastMousePos = curMouse;

                if (_hasMouseMoved || mouse.leftButton.isPressed)
                {
                    Vector3 mouseWorld = _mainCam.ScreenToWorldPoint(new Vector3(curMouse.x, curMouse.y, -_mainCam.transform.position.z));
                    AimWorldPosition = mouseWorld;

                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player != null)
                    {
                        Vector2 dir = (mouseWorld - player.transform.position);
                        if (dir.sqrMagnitude > 0.1f) rawAim = dir.normalized;
                    }
                }
                else if (MoveInput.sqrMagnitude > 0.01f)
                {
                    rawAim = MoveInput.normalized;
                }
            }
            else if (MoveInput.sqrMagnitude > 0.01f)
            {
                rawAim = MoveInput.normalized;
            }

            // Apply Auto-Aim Assist
            GameObject pObj = GameObject.FindGameObjectWithTag("Player");
            if (pObj != null && AutoAimSystem.Instance != null)
            {
                AimDirection = AutoAimSystem.Instance.AdjustAim(pObj.transform.position, rawAim);
            }
            else
            {
                AimDirection = rawAim;
            }

            // 3. ACTIONS PROCESSING
            bool mouseFire = mouse != null && mouse.leftButton.isPressed && !isPointerOverUI;
            bool kbFire = kb != null && (kb.jKey.isPressed);
            bool padFire = pad != null && (pad.rightTrigger.isPressed || pad.buttonSouth.isPressed);
            IsFiring = TouchFirePressed || mouseFire || kbFire || padFire;

            bool mouseBoost = mouse != null && mouse.rightButton.isPressed && !isPointerOverUI;
            bool kbBoost = kb != null && (kb.spaceKey.isPressed || kb.leftShiftKey.isPressed);
            bool padBoost = pad != null && (pad.leftTrigger.isPressed || pad.buttonWest.isPressed);
            IsBoosting = TouchBoostPressed || mouseBoost || kbBoost || padBoost;

            bool kbAbility = kb != null && kb.kKey.wasPressedThisFrame;
            bool padAbility = pad != null && pad.buttonNorth.wasPressedThisFrame;
            IsAbilityTriggered = TouchAbilityPressed || kbAbility || padAbility;

            bool kbPause = kb != null && kb.escapeKey.wasPressedThisFrame;
            bool padPause = pad != null && pad.startButton.wasPressedThisFrame;
            IsPauseTriggered = kbPause || padPause;
        }
    }
}
