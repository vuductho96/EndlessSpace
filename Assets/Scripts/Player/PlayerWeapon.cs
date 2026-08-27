using UnityEngine;
using SpaceShooter.Input;
using SpaceShooter.Core;

namespace SpaceShooter.Player
{
    public class PlayerWeapon : MonoBehaviour
    {
        [Header("Hardpoints")]
        [SerializeField] private Transform[] _firePoints;
        [SerializeField] private GameObject _projectilePrefab;

        [Header("Weapon Stats")]
        [SerializeField] private float _fireRate = 0.12f;
        [SerializeField] private float _energyCost = 3f;
        [SerializeField] private float _heatPerShot = 4f;

        private float _nextFireTime;
        private PlayerStats _stats;

        private void Awake()
        {
            _stats = GetComponent<PlayerStats>();
        }

        private void Update()
        {
            if (InputManager.Instance == null) return;

            if (InputManager.Instance.IsFiring && Time.time >= _nextFireTime)
            {
                TryFire();
            }
        }

        private void TryFire()
        {
            if (_stats != null && _stats.IsOverheated) return;

            if (_stats != null && !_stats.ConsumeEnergy(_energyCost)) return;

            if (_stats != null) _stats.AddHeat(_heatPerShot);

            _nextFireTime = Time.time + _fireRate;
            HapticFeedback.TriggerLight();

            if (_firePoints != null && _firePoints.Length > 0 && _projectilePrefab != null)
            {
                foreach (var point in _firePoints)
                {
                    if (point != null)
                    {
                        GameObject spawned = null;
                        if (ObjectPooler.Instance != null)
                        {
                            spawned = ObjectPooler.Instance.Spawn(_projectilePrefab, point.position, point.rotation);
                        }
                        else
                        {
                            spawned = Instantiate(_projectilePrefab, point.position, point.rotation);
                            spawned.SetActive(true);
                        }
                    }
                }
            }
        }
    }
}
