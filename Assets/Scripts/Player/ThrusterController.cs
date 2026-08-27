using System.Collections.Generic;
using UnityEngine;
using SpaceShooter.Fighters.Data;
using SpaceShooter.Player.Interfaces;

namespace SpaceShooter.Player
{
    public class ThrusterController : MonoBehaviour
    {
        [Header("Discovered Thruster Plumes")]
        [SerializeField] private List<SpriteRenderer> _thrusterPlumes = new List<SpriteRenderer>();

        [Header("Dynamics")]
        [SerializeField] private float _minFlameScale = 0.4f;
        [SerializeField] private float _maxFlameScale = 1.25f;
        [SerializeField] private float _boostScaleMultiplier = 1.6f;

        private IPlayerMovement _movement;
        private Color _plasmaColor = new Color(0.2f, 0.7f, 1f, 1f);

        private void Awake()
        {
            _movement = GetComponentInParent<IPlayerMovement>();
            DiscoverThrusterPlumes();
        }

        public void Initialize(FighterDefinition definition)
        {
            DiscoverThrusterPlumes();
            if (definition != null)
            {
                _plasmaColor = definition.thrusterPlasmaColor;
                foreach (var plume in _thrusterPlumes)
                {
                    if (plume != null) plume.color = _plasmaColor;
                }
            }
        }

        public void DiscoverThrusterPlumes()
        {
            _thrusterPlumes.Clear();
            var renderers = GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var r in renderers)
            {
                string name = r.name.ToLowerInvariant();
                if (name.Contains("flame") || name.Contains("thruster") || name.Contains("plume"))
                {
                    _thrusterPlumes.Add(r);
                }
            }
        }

        private void Update()
        {
            if (_movement == null) _movement = GetComponentInParent<IPlayerMovement>();
            if (_movement == null) return;

            float throttle = _movement.ThrottleRatio;
            bool isBoosting = _movement.IsBoosting;

            float targetScale = Mathf.Lerp(_minFlameScale, _maxFlameScale, throttle);
            if (isBoosting) targetScale *= _boostScaleMultiplier;

            // Subtle flickering
            float flicker = 1f + 0.1f * Mathf.Sin(Time.time * 40f);
            targetScale *= flicker;

            foreach (var plume in _thrusterPlumes)
            {
                if (plume != null)
                {
                    plume.transform.localScale = new Vector3(plume.transform.localScale.x, targetScale, 1f);
                    Color c = _plasmaColor;
                    c.a = Mathf.Lerp(0.6f, 1.0f, throttle);
                    plume.color = c;
                }
            }
        }
    }
}
