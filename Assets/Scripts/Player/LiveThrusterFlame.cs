using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter.Player
{
    /// <summary>
    /// Live aerospace plasma flame system (Xenon/Argon violet-cyan cruising & Krypton golden afterburner).
    /// Positions plumes accurately at dual engine nozzle bells with Mach shock diamonds.
    /// </summary>
    public class LiveThrusterFlame : MonoBehaviour
    {
        [Header("Flame Renderers")]
        [SerializeField] private SpriteRenderer _leftFlameRenderer;
        [SerializeField] private SpriteRenderer _rightFlameRenderer;

        [Header("Plasma Animation Frames")]
        [SerializeField] private Sprite[] _idlePlasmaFrames;
        [SerializeField] private Sprite[] _boostPlasmaFrames;
        [SerializeField] private float _animationFps = 24f;

        [Header("Plasma Dynamics")]
        [SerializeField] private Vector3 _idleScale = new Vector3(1.2f, 2.0f, 1f);
        [SerializeField] private Vector3 _throttleScale = new Vector3(1.5f, 2.8f, 1f);
        [SerializeField] private Vector3 _boostScale = new Vector3(2.0f, 3.8f, 1f);
        [SerializeField] private float _flickerFrequency = 32f;
        [SerializeField] private float _flickerIntensity = 0.12f;

        private PlayerMovement _movement;
        private float _animTimer;
        private int _currentFrameIndex;

        private void Awake()
        {
            _movement = GetComponentInParent<PlayerMovement>() ?? GetComponent<PlayerMovement>();
            LoadDefaultPlasmaFrames();
        }

        private void Update()
        {
            if (_movement == null)
            {
                _movement = GetComponentInParent<PlayerMovement>() ?? GetComponent<PlayerMovement>();
            }

            float throttle = _movement != null ? Mathf.Clamp01(_movement.ThrottleRatio) : 0.2f;
            bool isBoost = _movement != null && _movement.IsBoosting;

            UpdatePlasmaAnimation(isBoost);
            UpdatePlasmaTransform(throttle, isBoost);
        }

        private void UpdatePlasmaAnimation(bool isBoost)
        {
            Sprite[] currentSequence = isBoost ? _boostPlasmaFrames : _idlePlasmaFrames;
            if (currentSequence == null || currentSequence.Length == 0) return;

            _animTimer += Time.deltaTime * _animationFps;
            _currentFrameIndex = Mathf.FloorToInt(_animTimer) % currentSequence.Length;
            Sprite activeFrame = currentSequence[_currentFrameIndex];

            if (_leftFlameRenderer != null) _leftFlameRenderer.sprite = activeFrame;
            if (_rightFlameRenderer != null) _rightFlameRenderer.sprite = activeFrame;
        }

        private void UpdatePlasmaTransform(float throttle, bool isBoost)
        {
            float flicker = 1f + _flickerIntensity * Mathf.Sin(Time.time * _flickerFrequency)
                               + (_flickerIntensity * 0.4f) * Mathf.Cos(Time.time * (_flickerFrequency * 1.5f));

            Vector3 targetBaseScale = isBoost ? _boostScale : Vector3.Lerp(_idleScale, _throttleScale, throttle);
            Vector3 finalScale = targetBaseScale * flicker;

            if (_leftFlameRenderer != null)
            {
                _leftFlameRenderer.transform.localScale = finalScale;
                _leftFlameRenderer.color = Color.white;
            }

            if (_rightFlameRenderer != null)
            {
                _rightFlameRenderer.transform.localScale = finalScale;
                _rightFlameRenderer.color = Color.white;
            }
        }

        public void SetRenderers(SpriteRenderer leftFlame, SpriteRenderer rightFlame)
        {
            _leftFlameRenderer = leftFlame;
            _rightFlameRenderer = rightFlame;
        }

        private void LoadDefaultPlasmaFrames()
        {
            if (_idlePlasmaFrames == null || _idlePlasmaFrames.Length == 0)
            {
                var list = new List<Sprite>();
                for (int i = 0; i < 8; i++)
                {
                    var s = PrototypeSceneSetup.LoadSpriteFromFile($"PlayerFighter/Thruster/PlasmaFrames/xenon_plasma_idle_{i:02d}.png", 100f, new Vector2(0.5f, 1.0f));
                    if (s != null) list.Add(s);
                }
                _idlePlasmaFrames = list.ToArray();
            }

            if (_boostPlasmaFrames == null || _boostPlasmaFrames.Length == 0)
            {
                var list = new List<Sprite>();
                for (int i = 0; i < 8; i++)
                {
                    var s = PrototypeSceneSetup.LoadSpriteFromFile($"PlayerFighter/Thruster/PlasmaFrames/krypton_plasma_boost_{i:02d}.png", 100f, new Vector2(0.5f, 1.0f));
                    if (s != null) list.Add(s);
                }
                _boostPlasmaFrames = list.ToArray();
            }
        }
    }
}
