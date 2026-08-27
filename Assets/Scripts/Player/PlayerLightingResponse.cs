using UnityEngine;
using SpaceShooter.Environment;

namespace SpaceShooter.Player
{
    public class PlayerLightingResponse : MonoBehaviour
    {
        [Header("Response Layers")]
        [SerializeField] private SpriteRenderer _rimLightRenderer;
        [SerializeField] private SpriteRenderer _sheenRenderer;
        [SerializeField] private SpriteRenderer _emissiveBoostRenderer;

        private void Start()
        {
            if (SpaceLightingController.Instance != null)
            {
                SpaceLightingController.Instance.OnLightingStateChanged += UpdateLightingResponse;
                UpdateLightingResponse(SpaceLightingController.Instance.CurrentState);
            }
        }

        private void OnDestroy()
        {
            if (SpaceLightingController.Instance != null)
            {
                SpaceLightingController.Instance.OnLightingStateChanged -= UpdateLightingResponse;
            }
        }

        public void UpdateLightingResponse(LightingState state)
        {
            switch (state)
            {
                case LightingState.STELLAR:
                    if (_rimLightRenderer != null) _rimLightRenderer.color = new Color(0.8f, 0.95f, 1f, 0.4f);
                    if (_emissiveBoostRenderer != null) _emissiveBoostRenderer.color = new Color(1f, 1f, 1f, 0.3f);
                    break;
                case LightingState.ECLIPSE:
                    if (_rimLightRenderer != null) _rimLightRenderer.color = new Color(0.3f, 0.7f, 1f, 0.8f);
                    if (_emissiveBoostRenderer != null) _emissiveBoostRenderer.color = new Color(0f, 0.9f, 1f, 0.9f);
                    break;
                case LightingState.DEEP_VOID:
                    if (_rimLightRenderer != null) _rimLightRenderer.color = new Color(0.1f, 0.4f, 0.8f, 0.9f);
                    if (_emissiveBoostRenderer != null) _emissiveBoostRenderer.color = new Color(0f, 1f, 1f, 1f);
                    break;
                case LightingState.NEBULA:
                    if (_rimLightRenderer != null) _rimLightRenderer.color = new Color(0.8f, 0.3f, 1f, 0.6f);
                    if (_sheenRenderer != null) _sheenRenderer.color = new Color(0.7f, 0.2f, 0.9f, 0.5f);
                    break;
                case LightingState.STELLAR_STORM:
                    if (_rimLightRenderer != null) _rimLightRenderer.color = new Color(1f, 0.6f, 0.2f, 0.8f);
                    break;
                case LightingState.ANOMALY:
                    if (_rimLightRenderer != null) _rimLightRenderer.color = new Color(0.2f, 1f, 0.9f, 0.7f);
                    break;
            }
        }
    }
}
