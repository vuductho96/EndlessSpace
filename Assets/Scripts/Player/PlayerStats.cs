using System;
using UnityEngine;
using SpaceShooter.Combat;
using SpaceShooter.Input;
using SpaceShooter.Fighters.Data;

namespace SpaceShooter.Player
{
    public class PlayerStats : MonoBehaviour, IDamageable
    {
        [Header("Hull Fallback")]
        [SerializeField] private float _maxHull = 100f;
        [SerializeField] private float _currentHull = 100f;

        [Header("Shield Fallback")]
        [SerializeField] private float _maxShield = 100f;
        [SerializeField] private float _currentShield = 100f;
        [SerializeField] private float _shieldRegenRate = 15f;
        [SerializeField] private float _shieldRegenDelay = 3f;

        [Header("Energy Subsystem")]
        [SerializeField] private float _maxEnergy = 100f;
        [SerializeField] private float _currentEnergy = 100f;
        [SerializeField] private float _energyRegenRate = 25f;

        [Header("Heat Subsystem")]
        [SerializeField] private float _maxHeat = 100f;
        [SerializeField] private float _currentHeat = 0f;
        [SerializeField] private float _heatDissipationRate = 20f;
        [SerializeField] private bool _isOverheated = false;

        private PlayerHealth _healthComponent;
        private PlayerShield _shieldComponent;
        private float _lastDamageTime;

        public float CurrentHealth => _healthComponent != null ? _healthComponent.CurrentHealth : _currentHull;
        public float MaxHealth => _healthComponent != null ? _healthComponent.MaxHealth : _maxHull;
        public float CurrentShield => _shieldComponent != null ? _shieldComponent.CurrentShield : _currentShield;
        public float MaxShield => _shieldComponent != null ? _shieldComponent.MaxShield : _maxShield;
        public float CurrentEnergy => _currentEnergy;
        public float MaxEnergy => _maxEnergy;
        public float CurrentHeat => _currentHeat;
        public float MaxHeat => _maxHeat;
        public bool IsOverheated => _isOverheated;
        public bool IsDead => _healthComponent != null ? _healthComponent.IsDead : (_currentHull <= 0f);

        public event Action OnStatsChanged;

        private void Awake()
        {
            BindComponents();
        }

        private void BindComponents()
        {
            if (_healthComponent == null) _healthComponent = GetComponent<PlayerHealth>();
            if (_shieldComponent == null) _shieldComponent = GetComponent<PlayerShield>();
        }

        public void InitializeFromDefinition(FighterDefinition definition)
        {
            BindComponents();

            if (definition != null)
            {
                _maxHull = definition.maxHealth;
                _maxShield = definition.maxShield;
                _shieldRegenRate = definition.shieldRechargeRate;
                _shieldRegenDelay = definition.shieldDelay;
                _maxEnergy = definition.maxEnergy;
                _energyRegenRate = definition.energyRegenRate;
                _maxHeat = definition.maxHeat;
                _heatDissipationRate = definition.heatDissipationRate;
            }

            _currentHull = _maxHull;
            _currentShield = _maxShield;
            _currentEnergy = _maxEnergy;
            _currentHeat = 0f;
            _isOverheated = false;
            OnStatsChanged?.Invoke();
        }

        private void Start()
        {
            BindComponents();
            if (_currentHull <= 0f) _currentHull = _maxHull;
            if (_currentShield <= 0f) _currentShield = _maxShield;
            if (_currentEnergy <= 0f) _currentEnergy = _maxEnergy;
            _currentHeat = 0f;
        }

        private void Update()
        {
            // Standalone shield regen only if no PlayerShield component is present
            if (_shieldComponent == null)
            {
                if (Time.time - _lastDamageTime > _shieldRegenDelay && _currentShield < _maxShield)
                {
                    _currentShield = Mathf.Min(_maxShield, _currentShield + _shieldRegenRate * Time.deltaTime);
                }
            }

            // Energy Regen
            if (_currentEnergy < _maxEnergy)
            {
                _currentEnergy = Mathf.Min(_maxEnergy, _currentEnergy + _energyRegenRate * Time.deltaTime);
            }

            // Heat Dissipation
            if (_currentHeat > 0f)
            {
                _currentHeat = Mathf.Max(0f, _currentHeat - _heatDissipationRate * Time.deltaTime);
                if (_isOverheated && _currentHeat <= 15f)
                {
                    _isOverheated = false;
                }
            }

            OnStatsChanged?.Invoke();
        }

        public void TakeDamage(float damage)
        {
            if (_healthComponent != null)
            {
                _healthComponent.TakeDamage(damage);
                OnStatsChanged?.Invoke();
                return;
            }

            _lastDamageTime = Time.time;
            if (_currentShield > 0f)
            {
                float shieldAbsorption = Mathf.Min(_currentShield, damage);
                _currentShield -= shieldAbsorption;
                damage -= shieldAbsorption;
                HapticFeedback.TriggerMedium();
            }

            if (damage > 0f)
            {
                _currentHull = Mathf.Max(0f, _currentHull - damage);
                HapticFeedback.TriggerHeavy();
            }

            OnStatsChanged?.Invoke();
        }

        public bool ConsumeEnergy(float amount)
        {
            if (_currentEnergy >= amount)
            {
                _currentEnergy -= amount;
                OnStatsChanged?.Invoke();
                return true;
            }
            return false;
        }

        public void AddHeat(float amount)
        {
            _currentHeat = Mathf.Min(_maxHeat, _currentHeat + amount);
            if (_currentHeat >= _maxHeat)
            {
                _isOverheated = true;
            }
            OnStatsChanged?.Invoke();
        }
    }
}
