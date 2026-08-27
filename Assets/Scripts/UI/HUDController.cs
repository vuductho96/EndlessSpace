using UnityEngine;
using UnityEngine.UI;
using SpaceShooter.Player;

namespace SpaceShooter.UI
{
    public class HUDController : MonoBehaviour
    {
        [Header("Gauges")]
        [SerializeField] private Slider _hullSlider;
        [SerializeField] private Slider _shieldSlider;
        [SerializeField] private Slider _energySlider;
        [SerializeField] private Slider _heatSlider;

        [Header("Radar Sweep")]
        [SerializeField] private RectTransform _radarSweepTransform;
        [SerializeField] private float _radarSweepSpeed = 180f;

        [Header("Targeting Reticle")]
        [SerializeField] private RectTransform _reticleTransform;

        private PlayerStats _playerStats;

        private void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerStats = player.GetComponent<PlayerStats>();
            }
        }

        private void Update()
        {
            if (_playerStats == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) _playerStats = player.GetComponent<PlayerStats>();
                if (_playerStats == null) return;
            }

            if (_hullSlider != null) _hullSlider.value = _playerStats.MaxHealth > 0.001f ? Mathf.Clamp01(_playerStats.CurrentHealth / _playerStats.MaxHealth) : 0f;
            if (_shieldSlider != null) _shieldSlider.value = _playerStats.MaxShield > 0.001f ? Mathf.Clamp01(_playerStats.CurrentShield / _playerStats.MaxShield) : 0f;
            if (_energySlider != null) _energySlider.value = _playerStats.MaxEnergy > 0.001f ? Mathf.Clamp01(_playerStats.CurrentEnergy / _playerStats.MaxEnergy) : 0f;
            if (_heatSlider != null) _heatSlider.value = _playerStats.MaxHeat > 0.001f ? Mathf.Clamp01(_playerStats.CurrentHeat / _playerStats.MaxHeat) : 0f;

            // Radar continuous rotation
            if (_radarSweepTransform != null)
            {
                _radarSweepTransform.Rotate(0f, 0f, -_radarSweepSpeed * Time.unscaledDeltaTime);
            }

            // Reticle follows aim
            if (_reticleTransform != null && SpaceShooter.Input.InputManager.Instance != null && UnityEngine.Camera.main != null)
            {
                Vector3 screenPos = UnityEngine.Camera.main.WorldToScreenPoint(SpaceShooter.Input.InputManager.Instance.AimWorldPosition);
                _reticleTransform.position = screenPos;
            }
        }
    }
}
