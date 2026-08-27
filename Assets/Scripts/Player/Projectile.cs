using UnityEngine;
using SpaceShooter.Combat;
using SpaceShooter.Combat.Data;
using SpaceShooter.Core;

namespace SpaceShooter.Player
{
    public class Projectile : MonoBehaviour
    {
        [Header("Projectile Stats")]
        [SerializeField] private float _speed = 36f;
        [SerializeField] private float _damage = 25f;
        [SerializeField] private float _lifeTime = 2.0f;
        [SerializeField] private bool _isPiercing = false;
        [SerializeField] private int _pierceCount = 1;

        private float _spawnTime;
        private int _currentPierces;

        public float Damage => _damage;
        public float Speed => _speed;

        private void OnEnable()
        {
            _spawnTime = Time.time;
            _currentPierces = 0;
        }

        public void Configure(ProjectileDefinition definition, float damageMult = 1f, float speedMult = 1f)
        {
            if (definition != null)
            {
                _damage = definition.baseDamage * damageMult;
                _speed = definition.speed * speedMult;
                _lifeTime = definition.lifetime;
                _isPiercing = definition.isPiercing;
                _pierceCount = definition.maxPierceCount;

                var sr = GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = definition.projectileTint;
                }
            }
        }

        private void Update()
        {
            transform.position += transform.up * (_speed * Time.deltaTime);

            if (Time.time - _spawnTime >= _lifeTime)
            {
                Despawn();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player")) return;

            IDamageable target = collision.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(_damage);
                _currentPierces++;

                if (!_isPiercing || _currentPierces >= _pierceCount)
                {
                    Despawn();
                }
            }
        }

        private string _poolKey;

        public void SetPoolKey(string key)
        {
            _poolKey = key;
        }

        private void Despawn()
        {
            if (ObjectPooler.Instance != null)
            {
                string key = !string.IsNullOrEmpty(_poolKey) ? _poolKey : gameObject.name.Replace("(Clone)", "").Trim();
                ObjectPooler.Instance.Despawn(gameObject, key);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
