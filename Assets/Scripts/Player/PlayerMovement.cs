using UnityEngine;
using SpaceShooter.Input;
using SpaceShooter.Fighters.Data;
using SpaceShooter.Player.Interfaces;

namespace SpaceShooter.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour, IPlayerMovement
    {
        [Header("Kinematics & Speeds")]
        [SerializeField] private float _normalSpeed = 12f;
        [SerializeField] private float _boostSpeed = 22f;
        [SerializeField] private float _acceleration = 45f;
        [SerializeField] private float _deceleration = 25f;
        [SerializeField] private float _rotationSpeed = 720f;
        [SerializeField] private float _boostEnergyCostPerSec = 20f;

        private Rigidbody2D _rb;
        private PlayerController _player;
        private bool _isMovementLocked = false;

        public Vector2 CurrentVelocity => _rb != null ? _rb.linearVelocity : Vector2.zero;
        public float ThrottleRatio { get; private set; }
        public bool IsBoosting { get; private set; }
        public float MoveSpeed => _normalSpeed;
        public float Acceleration => _acceleration;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _player = GetComponent<PlayerController>();
            _rb.gravityScale = 0f;
            _rb.linearDamping = 1.5f;
        }

        public void Initialize(FighterDefinition definition)
        {
            if (definition != null)
            {
                _normalSpeed = definition.moveSpeed;
                _boostSpeed = definition.boostSpeed;
                _acceleration = definition.acceleration;
                _deceleration = definition.deceleration;
                _rotationSpeed = definition.rotationSpeed;
                _boostEnergyCostPerSec = definition.boostEnergyCostPerSec;
                if (_rb != null) _rb.linearDamping = definition.linearDamping;
            }
            _isMovementLocked = false;
        }

        private void FixedUpdate()
        {
            if (_isMovementLocked)
            {
                _rb.linearVelocity = Vector2.zero;
                ThrottleRatio = 0f;
                IsBoosting = false;
                return;
            }

            if (InputManager.Instance == null) return;

            Vector2 moveInput = InputManager.Instance.MoveInput;
            bool boostRequested = InputManager.Instance.IsBoosting;

            // Handle Boost
            bool wasBoosting = IsBoosting;
            IsBoosting = false;
            float targetMaxSpeed = _normalSpeed;
            if (boostRequested && _player != null && _player.Stats != null && _player.Stats.ConsumeEnergy(_boostEnergyCostPerSec * Time.fixedDeltaTime))
            {
                targetMaxSpeed = _boostSpeed;
                IsBoosting = true;
                if (!wasBoosting)
                {
                    HapticFeedback.TriggerLight();
                }
            }

            // Acceleration / Deceleration
            if (moveInput.sqrMagnitude > 0.01f)
            {
                Vector2 targetVelocity = moveInput * targetMaxSpeed;
                _rb.linearVelocity = Vector2.MoveTowards(_rb.linearVelocity, targetVelocity, _acceleration * Time.fixedDeltaTime);
                ThrottleRatio = _rb.linearVelocity.magnitude / _boostSpeed;
            }
            else
            {
                _rb.linearVelocity = Vector2.MoveTowards(_rb.linearVelocity, Vector2.zero, _deceleration * Time.fixedDeltaTime);
                ThrottleRatio = 0f;
            }

            // Smooth Facing Alignment
            Vector2 aimInput = InputManager.Instance.AimDirection;
            Vector2 faceDir = aimInput.sqrMagnitude > 0.01f ? aimInput : (moveInput.sqrMagnitude > 0.01f ? moveInput : Vector2.zero);

            if (faceDir.sqrMagnitude > 0.01f)
            {
                float targetAngle = Mathf.Atan2(faceDir.y, faceDir.x) * Mathf.Rad2Deg - 90f;
                float currentAngle = _rb.rotation;
                float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, _rotationSpeed * Time.fixedDeltaTime);
                _rb.rotation = newAngle;
            }
        }

        public void ApplyImpulse(Vector2 impulseForce)
        {
            if (_rb != null)
            {
                _rb.AddForce(impulseForce, ForceMode2D.Impulse);
            }
        }

        public void SetMovementLock(bool isLocked)
        {
            _isMovementLocked = isLocked;
        }
    }
}
