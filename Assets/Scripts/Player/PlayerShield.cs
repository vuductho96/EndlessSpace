using System;
using UnityEngine;
using SpaceShooter.Fighters.Data;
using SpaceShooter.Player.Interfaces;
using SpaceShooter.Input;

namespace SpaceShooter.Player
{
    public class PlayerShield : MonoBehaviour, IShieldSystem
    {
        [Header("Shield Config")]
        [SerializeField] private float _maxShield = 100f;
        [SerializeField] private float _currentShield = 100f;
        [SerializeField] private float _shieldRechargeRate = 15f;
        [SerializeField] private float _shieldDelay = 3.0f;

        private float _lastDamageTime = -999f;
        private PlayerController _player;

        public float CurrentShield => _currentShield;
        public float MaxShield => _maxShield;
        public float ShieldRatio => _maxShield > 0f ? Mathf.Clamp01(_currentShield / _maxShield) : 0f;
        public bool IsShieldActive => _currentShield > 0.01f;

        public event Action<float, float> OnShieldChanged;
        public event Action OnShieldBroken;
        public event Action OnShieldRestored;

        private void Awake()
        {
            _player = GetComponent<PlayerController>();
        }

        public void Initialize(FighterDefinition definition)
        {
            if (definition != null)
            {
                _maxShield = definition.maxShield;
                _shieldRechargeRate = definition.shieldRechargeRate;
                _shieldDelay = definition.shieldDelay;
            }

            _currentShield = _maxShield;
            _lastDamageTime = -999f;
            OnShieldChanged?.Invoke(_currentShield, _maxShield);
        }

        private void Update()
        {
            // Shield Recharge
            if (Time.time - _lastDamageTime > _shieldDelay && _currentShield < _maxShield)
            {
                bool wasBroken = !IsShieldActive;
                _currentShield = Mathf.Min(_maxShield, _currentShield + _shieldRechargeRate * Time.deltaTime);
                OnShieldChanged?.Invoke(_currentShield, _maxShield);

                if (wasBroken && IsShieldActive)
                {
                    OnShieldRestored?.Invoke();
                }
            }
        }

        public bool AbsorbDamage(ref float incomingDamage)
        {
            if (_maxShield <= 0f || _currentShield <= 0f) return false;

            _lastDamageTime = Time.time;

            if (_currentShield >= incomingDamage)
            {
                _currentShield -= incomingDamage;
                incomingDamage = 0f;
                HapticFeedback.TriggerLight();
                _player?.Visuals?.TriggerShieldImpact(transform.position);
                OnShieldChanged?.Invoke(_currentShield, _maxShield);
                return true;
            }
            else
            {
                incomingDamage -= _currentShield;
                _currentShield = 0f;
                HapticFeedback.TriggerMedium();
                _player?.Visuals?.TriggerShieldImpact(transform.position);
                OnShieldBroken?.Invoke();
                OnShieldChanged?.Invoke(_currentShield, _maxShield);
                return false;
            }
        }

        public void RestoreShield(float amount)
        {
            if (amount <= 0f) return;
            bool wasBroken = !IsShieldActive;
            _currentShield = Mathf.Min(_maxShield, _currentShield + amount);
            OnShieldChanged?.Invoke(_currentShield, _maxShield);

            if (wasBroken && IsShieldActive)
            {
                OnShieldRestored?.Invoke();
            }
        }
    }
}
