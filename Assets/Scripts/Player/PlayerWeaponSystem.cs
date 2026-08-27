using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceShooter.Combat.Data;
using SpaceShooter.Fighters.Data;
using SpaceShooter.Player.Interfaces;
using SpaceShooter.Input;
using SpaceShooter.Core;

namespace SpaceShooter.Player
{
    public class PlayerWeaponSystem : MonoBehaviour, IWeaponSystem
    {
        [Header("Mount Points")]
        [SerializeField] private List<Transform> _weaponMounts = new List<Transform>();
        public IReadOnlyList<Transform> WeaponMounts => _weaponMounts;
        public int MountCount => _weaponMounts.Count;

        [Header("Active Weapon")]
        [SerializeField] private WeaponDefinition _currentWeapon;
        public WeaponDefinition CurrentWeapon => _currentWeapon;

        private float _nextFireTime = 0f;
        private bool _isFiring = false;
        private PlayerController _player;

        public bool IsFiring => _isFiring;

        private void Awake()
        {
            _player = GetComponent<PlayerController>();
            DiscoverWeaponMounts();
        }

        public void Initialize(FighterDefinition definition, WeaponDefinition overrideWeapon = null)
        {
            DiscoverWeaponMounts();

            if (overrideWeapon != null)
            {
                _currentWeapon = overrideWeapon;
            }
            else if (definition != null && definition.defaultWeapon != null)
            {
                _currentWeapon = definition.defaultWeapon;
            }

            _nextFireTime = 0f;
            _isFiring = false;
        }

        public void DiscoverWeaponMounts()
        {
            _weaponMounts.Clear();

            // Find all children matching WeaponMount or Weapon_
            var allChildren = GetComponentsInChildren<Transform>(true);
            foreach (var child in allChildren)
            {
                if (child == transform) continue;

                string lower = child.name.ToLowerInvariant();
                if (lower.StartsWith("weaponmount") || lower.StartsWith("weapon_") || lower.StartsWith("firepoint") || lower.Contains("mount"))
                {
                    _weaponMounts.Add(child);
                }
            }

            // Fallback: If no dedicated mount transform is found, use self
            if (_weaponMounts.Count == 0)
            {
                _weaponMounts.Add(transform);
            }
        }

        private void Update()
        {
            if (InputManager.Instance == null) return;

            if (InputManager.Instance.IsFiring)
            {
                if (Time.time >= _nextFireTime)
                {
                    Fire();
                }
            }
            else
            {
                _isFiring = false;
            }
        }

        private float _fireRateMultiplier = 1.0f;
        private float _heatMultiplier = 1.0f;
        private float _damageMultiplier = 1.0f;

        public void SetTemporaryBuffs(float fireRateMultiplier = 1.0f, float heatMultiplier = 1.0f, float damageMultiplier = 1.0f)
        {
            _fireRateMultiplier = Mathf.Max(0.1f, fireRateMultiplier);
            _heatMultiplier = Mathf.Max(0f, heatMultiplier);
            _damageMultiplier = Mathf.Max(0.1f, damageMultiplier);
        }

        public void ResetTemporaryBuffs()
        {
            _fireRateMultiplier = 1.0f;
            _heatMultiplier = 1.0f;
            _damageMultiplier = 1.0f;
        }

        public void Fire()
        {
            if (_currentWeapon == null) return;
            if (_player != null && _player.Stats != null && _player.Stats.IsOverheated) return;

            // Check energy cost
            if (_player != null && _player.Stats != null && !_player.Stats.ConsumeEnergy(_currentWeapon.energyCost)) return;

            // Add heat
            if (_player != null && _player.Stats != null)
            {
                _player.Stats.AddHeat(_currentWeapon.heatPerShot * _heatMultiplier);
            }

            _isFiring = true;
            _nextFireTime = Time.time + (_currentWeapon.fireRate * _fireRateMultiplier);
            HapticFeedback.TriggerLight();

            if (_currentWeapon.isBurst)
            {
                StartCoroutine(BurstFireRoutine());
            }
            else
            {
                SpawnProjectilesFromAllMounts();
            }
        }

        public void StopFiring()
        {
            _isFiring = false;
        }

        public void SetWeapon(WeaponDefinition newWeapon)
        {
            _currentWeapon = newWeapon;
        }

        private void SpawnProjectilesFromAllMounts()
        {
            if (_currentWeapon == null || _currentWeapon.projectileDefinition == null) return;

            foreach (var mount in _weaponMounts)
            {
                if (mount == null) continue;

                int count = Mathf.Max(1, _currentWeapon.projectilesPerMount);
                float baseSpread = _currentWeapon.spreadAngle;

                for (int i = 0; i < count; i++)
                {
                    float angleOffset = 0f;
                    if (count > 1 && baseSpread > 0f)
                    {
                        angleOffset = Mathf.Lerp(-baseSpread * 0.5f, baseSpread * 0.5f, (float)i / (count - 1));
                    }

                    Quaternion rot = mount.rotation * Quaternion.Euler(0f, 0f, angleOffset);
                    SpawnProjectile(mount.position, rot);
                }
            }
        }

        private void SpawnProjectile(Vector3 position, Quaternion rotation)
        {
            GameObject projPrefab = _currentWeapon.projectileDefinition.projectilePrefab;
            GameObject spawned = null;

            if (ObjectPooler.Instance != null && projPrefab != null)
            {
                spawned = ObjectPooler.Instance.Spawn(projPrefab, position, rotation);
            }
            else if (projPrefab != null)
            {
                spawned = Instantiate(projPrefab, position, rotation);
            }
            else
            {
                // Procedural projectile fallback
                spawned = CreateProceduralProjectile(position, rotation);
            }

            var proj = spawned.GetComponent<Projectile>();
            if (proj != null)
            {
                proj.Configure(_currentWeapon.projectileDefinition, _currentWeapon.damageMultiplier * _damageMultiplier, _currentWeapon.projectileSpeedMultiplier);
            }
        }

        private GameObject CreateProceduralProjectile(Vector3 position, Quaternion rotation)
        {
            GameObject pObj = new GameObject("PlayerProjectile");
            pObj.transform.position = position;
            pObj.transform.rotation = rotation;
            pObj.transform.localScale = Vector3.one * 0.35f;

            var sr = pObj.AddComponent<SpriteRenderer>();
            sr.sprite = PrototypeSceneSetup.LoadSpriteFromFile("PlayerFighter_Experimental/CombatFX/projectile_basic.png", 100f);
            sr.color = _currentWeapon.projectileDefinition != null ? _currentWeapon.projectileDefinition.projectileTint : Color.cyan;
            sr.sortingOrder = 30;

            var col = pObj.AddComponent<CircleCollider2D>();
            col.radius = 0.4f;
            col.isTrigger = true;

            var rb = pObj.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;

            pObj.AddComponent<Projectile>();
            return pObj;
        }

        private IEnumerator BurstFireRoutine()
        {
            int burst = _currentWeapon.burstCount;
            float interval = _currentWeapon.burstInterval;

            for (int b = 0; b < burst; b++)
            {
                SpawnProjectilesFromAllMounts();
                if (b < burst - 1) yield return new WaitForSeconds(interval);
            }
        }
    }
}
