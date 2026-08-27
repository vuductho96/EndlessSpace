using System.Collections;
using UnityEngine;
using SpaceShooter.Fighters.Data;
using SpaceShooter.Player.Interfaces;

namespace SpaceShooter.Player.Visuals
{
    public class FighterVisualController : MonoBehaviour, IFighterVisuals
    {
        [Header("Modular Renderers")]
        [SerializeField] private SpriteRenderer _hullRenderer;
        [SerializeField] private SpriteRenderer _cockpitRenderer;
        [SerializeField] private SpriteRenderer _wingLeftRenderer;
        [SerializeField] private SpriteRenderer _wingRightRenderer;
        [SerializeField] private SpriteRenderer _coreRenderer;
        [SerializeField] private SpriteRenderer _shieldRenderer;
        [SerializeField] private SpriteRenderer _rimLightRenderer;

        private Color _originalHullColor = Color.white;
        private Color _originalCockpitColor = Color.white;
        private Color _originalWingColor = Color.white;
        private Coroutine _flashRoutine;

        private void Awake()
        {
            DiscoverRenderers();
        }

        public void Initialize(FighterDefinition definition)
        {
            DiscoverRenderers();

            if (definition != null)
            {
                _originalHullColor = definition.hullTint;
                _originalCockpitColor = definition.cockpitTint;
                _originalWingColor = definition.wingTint;

                if (_hullRenderer != null) _hullRenderer.color = _originalHullColor;
                if (_cockpitRenderer != null) _cockpitRenderer.color = _originalCockpitColor;
                if (_wingLeftRenderer != null) _wingLeftRenderer.color = _originalWingColor;
                if (_wingRightRenderer != null) _wingRightRenderer.color = _originalWingColor;
                if (_coreRenderer != null) _coreRenderer.color = definition.coreGlowColor;

                if (_shieldRenderer != null)
                {
                    Color sCol = definition.themeColor;
                    sCol.a = 0.35f;
                    _shieldRenderer.color = sCol;
                }
            }
        }

        public void DiscoverRenderers()
        {
            if (_hullRenderer == null) _hullRenderer = GetComponent<SpriteRenderer>();

            var renderers = GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var r in renderers)
            {
                string name = r.name.ToLowerInvariant();
                if (name.Contains("cockpit")) _cockpitRenderer = r;
                else if (name.Contains("wing_l") || name.Contains("wing_left")) _wingLeftRenderer = r;
                else if (name.Contains("wing_r") || name.Contains("wing_right")) _wingRightRenderer = r;
                else if (name.Contains("core")) _coreRenderer = r;
                else if (name.Contains("shield")) _shieldRenderer = r;
                else if (name.Contains("rim")) _rimLightRenderer = r;
            }

            if (_hullRenderer != null) _originalHullColor = _hullRenderer.color;
            if (_cockpitRenderer != null) _originalCockpitColor = _cockpitRenderer.color;
            if (_wingLeftRenderer != null) _originalWingColor = _wingLeftRenderer.color;
        }

        public void TriggerHitFlash(Color flashColor)
        {
            if (_flashRoutine != null) StopCoroutine(_flashRoutine);
            _flashRoutine = StartCoroutine(HitFlashRoutine(flashColor));
        }

        public void TriggerShieldImpact(Vector2 impactPoint)
        {
            if (_shieldRenderer != null)
            {
                StartCoroutine(ShieldImpactRoutine());
            }
        }

        public void TriggerDamageSmoke(float damageRatio)
        {
            // Darken hull slightly when heavily damaged
            if (_hullRenderer != null && damageRatio > 0.5f)
            {
                _hullRenderer.color = Color.Lerp(_originalHullColor, new Color(0.4f, 0.4f, 0.4f, 1f), (damageRatio - 0.5f) * 2f);
            }
        }

        public void TriggerDeathExplosion()
        {
            // Flash bright white and deactivate renderers
            if (_hullRenderer != null) _hullRenderer.enabled = false;
            if (_cockpitRenderer != null) _cockpitRenderer.enabled = false;
            if (_wingLeftRenderer != null) _wingLeftRenderer.enabled = false;
            if (_wingRightRenderer != null) _wingRightRenderer.enabled = false;
            if (_coreRenderer != null) _coreRenderer.enabled = false;
            if (_shieldRenderer != null) _shieldRenderer.enabled = false;
        }

        private IEnumerator HitFlashRoutine(Color flashColor)
        {
            if (_hullRenderer != null) _hullRenderer.color = flashColor;
            if (_wingLeftRenderer != null) _wingLeftRenderer.color = flashColor;
            if (_wingRightRenderer != null) _wingRightRenderer.color = flashColor;

            yield return new WaitForSeconds(0.08f);

            if (_hullRenderer != null) _hullRenderer.color = _originalHullColor;
            if (_wingLeftRenderer != null) _wingLeftRenderer.color = _originalWingColor;
            if (_wingRightRenderer != null) _wingRightRenderer.color = _originalWingColor;
        }

        private IEnumerator ShieldImpactRoutine()
        {
            if (_shieldRenderer == null) yield break;

            Color original = _shieldRenderer.color;
            Color flare = original;
            flare.a = 0.85f;
            _shieldRenderer.color = flare;

            yield return new WaitForSeconds(0.12f);
            _shieldRenderer.color = original;
        }
    }
}
