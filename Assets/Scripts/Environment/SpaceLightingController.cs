using System;
using System.Collections;
using UnityEngine;

namespace SpaceShooter.Environment
{
    public enum LightingState
    {
        STELLAR,
        ECLIPSE,
        DEEP_VOID,
        NEBULA,
        STELLAR_STORM,
        ANOMALY
    }

    public class SpaceLightingController : MonoBehaviour
    {
        public static SpaceLightingController Instance { get; private set; }

        [Header("State & Transitions")]
        [SerializeField] private LightingState _currentState = LightingState.STELLAR;
        [SerializeField] private float _transitionDuration = 1.5f;

        [Header("Overlay Target")]
        [SerializeField] private SpriteRenderer _ambientOverlay;

        public LightingState CurrentState => _currentState;
        public event Action<LightingState> OnLightingStateChanged;

        private Coroutine _transitionRoutine;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            ApplyLightingImmediate(_currentState);
        }

        public void SetLightingState(LightingState targetState)
        {
            if (_currentState == targetState && _transitionRoutine == null) return;
            if (_transitionRoutine != null) StopCoroutine(_transitionRoutine);
            _transitionRoutine = StartCoroutine(TransitionToState(targetState));
        }

        private IEnumerator TransitionToState(LightingState targetState)
        {
            LightingState oldState = _currentState;
            _currentState = targetState;
            OnLightingStateChanged?.Invoke(_currentState);

            Color startCol = GetAmbientColor(oldState);
            Color targetCol = GetAmbientColor(targetState);

            float elapsed = 0f;
            while (elapsed < _transitionDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / _transitionDuration);
                
                Color curCol = Color.Lerp(startCol, targetCol, t);
                if (_ambientOverlay != null) _ambientOverlay.color = curCol;

                yield return null;
            }

            ApplyLightingImmediate(targetState);
            _transitionRoutine = null;
        }

        private void ApplyLightingImmediate(LightingState state)
        {
            _currentState = state;
            Color col = GetAmbientColor(state);
            if (_ambientOverlay != null) _ambientOverlay.color = col;
            OnLightingStateChanged?.Invoke(_currentState);
        }

        public Color GetAmbientColor(LightingState state)
        {
            return state switch
            {
                LightingState.STELLAR => new Color(0.8f, 0.9f, 1.0f, 0.12f),
                LightingState.ECLIPSE => new Color(0.05f, 0.08f, 0.18f, 0.35f),
                LightingState.DEEP_VOID => new Color(0.01f, 0.02f, 0.05f, 0.38f),
                LightingState.NEBULA => new Color(0.65f, 0.25f, 0.95f, 0.18f),
                LightingState.STELLAR_STORM => new Color(0.95f, 0.5f, 0.2f, 0.18f),
                LightingState.ANOMALY => new Color(0.2f, 0.9f, 0.85f, 0.18f),
                _ => Color.clear
            };
        }
    }
}
