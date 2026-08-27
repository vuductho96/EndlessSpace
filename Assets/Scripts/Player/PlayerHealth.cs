using System;
using UnityEngine;
using SpaceShooter.Combat;
using SpaceShooter.Fighters.Data;
using SpaceShooter.Player.Interfaces;
using SpaceShooter.Input;

namespace SpaceShooter.Player
{
    public class PlayerHealth : MonoBehaviour, IDamageable, IHealthSystem
    {
        [Header("Health Config")]
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private float _currentHealth = 100f;
        [SerializeField] private float _healthRegen = 0f;
        [SerializeField] private float _damageResistance = 0f;

        private bool _isInvulnerable = false;
        private PlayerController _player;

        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;
        public float HealthRatio => _maxHealth > 0f ? Mathf.Clamp01(_currentHealth / _maxHealth) : 0f;
        public bool IsDead => _currentHealth <= 0f;

        public event Action<float, float> OnHealthChanged;
        public event Action OnDeath;

        private void Awake()
        {
            _player = GetComponent<PlayerController>();
        }

        public void Initialize(FighterDefinition definition)
        {
            if (definition != null)
            {
                _maxHealth = definition.maxHealth;
                _healthRegen = definition.healthRegen;
                _damageResistance = Mathf.Clamp(definition.damageResistance, 0f, 0.8f);
            }

            _currentHealth = _maxHealth;
            _isInvulnerable = false;
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        private void Update()
        {
            if (IsDead) return;

            if (_healthRegen > 0f && _currentHealth < _maxHealth)
            {
                Heal(_healthRegen * Time.deltaTime);
            }
        }

        public void TakeDamage(float amount)
        {
            if (IsDead || _isInvulnerable || amount <= 0f) return;

            // Check if Shield can absorb damage first
            if (_player != null && _player.Shield != null)
            {
                bool fullyAbsorbed = _player.Shield.AbsorbDamage(ref amount);
                if (fullyAbsorbed || amount <= 0f) return;
            }

            // Apply armor mitigation
            float effectiveDamage = amount * (1f - _damageResistance);
            _currentHealth = Mathf.Max(0f, _currentHealth - effectiveDamage);

            HapticFeedback.TriggerMedium();
            _player?.Visuals?.TriggerHitFlash(Color.red);
            _player?.Visuals?.TriggerDamageSmoke(1f - HealthRatio);

            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

            if (_currentHealth <= 0f)
            {
                Die();
            }
        }

        public void Heal(float amount)
        {
            if (IsDead || amount <= 0f) return;
            _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        public void SetInvulnerable(bool invulnerable)
        {
            _isInvulnerable = invulnerable;
        }

        private void Die()
        {
            Debug.Log($"<color=#FF3366><b>[PlayerHealth]</b></color> Player destroyed!");
            _player?.Visuals?.TriggerDeathExplosion();
            OnDeath?.Invoke();
        }
    }
}
