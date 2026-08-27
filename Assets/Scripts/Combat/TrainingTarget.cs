using System.Collections;
using UnityEngine;

namespace SpaceShooter.Combat
{
    public class TrainingTarget : MonoBehaviour, IDamageable
    {
        [Header("Target Health")]
        [SerializeField] private float _maxHealth = 150f;
        [SerializeField] private float _currentHealth = 150f;
        [SerializeField] private float _respawnDelay = 3f;

        [Header("Visuals")]
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private GameObject _hpBarContainer;
        [SerializeField] private Transform _hpBarFill;

        private bool _isDead = false;
        private Color _originalColor;

        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;
        public bool IsDead => _isDead;

        private void Awake()
        {
            if (_renderer == null) _renderer = GetComponentInChildren<SpriteRenderer>();
            if (_renderer != null) _originalColor = _renderer.color;
            _currentHealth = _maxHealth;
            UpdateHPBar();
        }

        public void TakeDamage(float damage)
        {
            if (_isDead) return;

            _currentHealth = Mathf.Max(0f, _currentHealth - damage);
            StartCoroutine(FlashEffect());
            UpdateHPBar();

            if (_currentHealth <= 0f)
            {
                Die();
            }
        }

        private IEnumerator FlashEffect()
        {
            if (_renderer != null)
            {
                _renderer.color = Color.white;
                yield return new WaitForSeconds(0.06f);
                _renderer.color = _originalColor;
            }
        }

        private void UpdateHPBar()
        {
            if (_hpBarFill != null)
            {
                float ratio = Mathf.Clamp01(_currentHealth / _maxHealth);
                _hpBarFill.localScale = new Vector3(ratio, 1f, 1f);
            }
        }

        private void Die()
        {
            _isDead = true;
            if (_renderer != null) _renderer.enabled = false;
            if (_hpBarContainer != null) _hpBarContainer.SetActive(false);
            StartCoroutine(RespawnRoutine());
        }

        private IEnumerator RespawnRoutine()
        {
            yield return new WaitForSeconds(_respawnDelay);
            _currentHealth = _maxHealth;
            _isDead = false;
            if (_renderer != null) _renderer.enabled = true;
            if (_hpBarContainer != null) _hpBarContainer.SetActive(true);
            UpdateHPBar();
        }
    }
}
