using System.Collections.Generic;
using UnityEngine;
using SpaceShooter.Input;

namespace SpaceShooter.Player
{
    /// <summary>
    /// Live, dynamic multi-stage thruster system for the modular player fighter.
    /// Drives twin main engine flames, boost afterburner, RCS maneuvering jets, and particle sparks.
    /// </summary>
    public class LiveThrusterSystem : MonoBehaviour
    {
        [Header("Main Engine Renderers")]
        [SerializeField] private SpriteRenderer _leftMainFlame;
        [SerializeField] private SpriteRenderer _rightMainFlame;

        [Header("RCS Maneuver Renderers")]
        [SerializeField] private SpriteRenderer _leftRcsFlame;
        [SerializeField] private SpriteRenderer _rightRcsFlame;

        [Header("Sprite Animation Sequences")]
        [SerializeField] private Sprite[] _idleFrames;
        [SerializeField] private Sprite[] _boostFrames;
        [SerializeField] private float _animationFps = 24f;

        [Header("Dynamic Scaling & Glow")]
        [SerializeField] private Vector3 _idleScale = new Vector3(0.6f, 0.7f, 1f);
        [SerializeField] private Vector3 _fullThrottleScale = new Vector3(0.9f, 1.3f, 1f);
        [SerializeField] private Vector3 _boostScale = new Vector3(1.3f, 2.0f, 1f);

        [Header("Particles")]
        [SerializeField] private ParticleSystem _exhaustParticles;

        private PlayerMovement _movement;
        private float _animTimer;
        private int _currentFrame;

        private void Awake()
        {
            _movement = GetComponentInParent<PlayerMovement>() ?? GetComponent<PlayerMovement>();
            LoadDefaultFramesIfEmpty();
        }

        private void Start()
        {
            SetupParticleTrail();
        }

        private void Update()
        {
            if (_movement == null)
            {
                _movement = GetComponentInParent<PlayerMovement>() ?? GetComponent<PlayerMovement>();
            }

            float throttle = _movement != null ? Mathf.Clamp01(_movement.ThrottleRatio) : 0.2f;
            bool isBoost = _movement != null && _movement.IsBoosting;
            Vector2 moveDir = InputManager.Instance != null ? InputManager.Instance.MoveInput : Vector2.zero;

            UpdateFlameAnimation(isBoost);
            UpdateFlameTransform(throttle, isBoost);
            UpdateRcsJets(moveDir.x);
            UpdateParticles(throttle, isBoost);
        }

        private void UpdateFlameAnimation(bool isBoost)
        {
            Sprite[] activeSequence = isBoost ? _boostFrames : _idleFrames;
            if (activeSequence == null || activeSequence.Length == 0) return;

            _animTimer += Time.deltaTime * _animationFps;
            _currentFrame = Mathf.FloorToInt(_animTimer) % activeSequence.Length;
            Sprite frame = activeSequence[_currentFrame];

            if (_leftMainFlame != null) _leftMainFlame.sprite = frame;
            if (_rightMainFlame != null) _rightMainFlame.sprite = frame;
        }

        private void UpdateFlameTransform(float throttle, bool isBoost)
        {
            float pulse = 1f + 0.08f * Mathf.Sin(Time.time * 35f);
            Vector3 baseScale = isBoost ? _boostScale : Vector3.Lerp(_idleScale, _fullThrottleScale, throttle);
            Vector3 currentScale = baseScale * pulse;

            if (_leftMainFlame != null) _leftMainFlame.transform.localScale = currentScale;
            if (_rightMainFlame != null) _rightMainFlame.transform.localScale = currentScale;
        }

        private void UpdateRcsJets(float horizontalInput)
        {
            // Banking right -> Left RCS jet fires to push left flank
            if (_leftRcsFlame != null)
            {
                bool active = horizontalInput > 0.15f;
                _leftRcsFlame.gameObject.SetActive(active);
                if (active)
                {
                    float intensity = Mathf.Abs(horizontalInput);
                    _leftRcsFlame.transform.localScale = new Vector3(0.5f * intensity, 0.7f * intensity, 1f);
                }
            }

            // Banking left -> Right RCS jet fires
            if (_rightRcsFlame != null)
            {
                bool active = horizontalInput < -0.15f;
                _rightRcsFlame.gameObject.SetActive(active);
                if (active)
                {
                    float intensity = Mathf.Abs(horizontalInput);
                    _rightRcsFlame.transform.localScale = new Vector3(0.5f * intensity, 0.7f * intensity, 1f);
                }
            }
        }

        private void UpdateParticles(float throttle, bool isBoost)
        {
            if (_exhaustParticles == null) return;

            var emission = _exhaustParticles.emission;
            float rate = Mathf.Lerp(15f, 60f, throttle);
            if (isBoost) rate *= 2.2f;
            emission.rateOverTime = rate;
        }

        private void SetupParticleTrail()
        {
            if (_exhaustParticles != null) return;

            GameObject pObj = new GameObject("ExhaustParticles");
            pObj.transform.SetParent(transform, false);
            pObj.transform.localPosition = new Vector3(0f, -1.2f, 0f);

            _exhaustParticles = pObj.AddComponent<ParticleSystem>();
            var pRenderer = pObj.GetComponent<ParticleSystemRenderer>();
            pRenderer.sortingOrder = 17;

            var main = _exhaustParticles.main;
            main.startLifetime = 0.35f;
            main.startSpeed = 4.5f;
            main.startSize = 0.18f;
            main.startColor = new Color(0.2f, 0.8f, 1f, 0.8f);
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var shape = _exhaustParticles.shape;
            shape.shapeType = ParticleSystemShapeType.Rectangle;
            shape.scale = new Vector3(0.8f, 0.1f, 0.1f);
            shape.rotation = new Vector3(90f, 0f, 0f); // Point downward

            var colOverLifetime = _exhaustParticles.colorOverLifetime;
            colOverLifetime.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] { new GradientColorKey(new Color(0f, 0.9f, 1f), 0f), new GradientColorKey(new Color(1f, 0.5f, 0.1f), 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(0.9f, 0f), new GradientAlphaKey(0f, 1f) }
            );
            colOverLifetime.color = grad;
        }

        public void SetFrames(Sprite[] idle, Sprite[] boost)
        {
            _idleFrames = idle;
            _boostFrames = boost;
        }

        public void SetRenderers(SpriteRenderer leftMain, SpriteRenderer rightMain, SpriteRenderer leftRcs, SpriteRenderer rightRcs)
        {
            _leftMainFlame = leftMain;
            _rightMainFlame = rightMain;
            _leftRcsFlame = leftRcs;
            _rightRcsFlame = rightRcs;
        }

        private void LoadDefaultFramesIfEmpty()
        {
            if (_idleFrames == null || _idleFrames.Length == 0)
            {
                var list = new List<Sprite>();
                for (int i = 0; i < 8; i++)
                {
                    var s = PrototypeSceneSetup.LoadSpriteFromFile($"PlayerFighter/Thruster/Frames/thrust_idle_{i:02d}.png", 100f);
                    if (s != null) list.Add(s);
                }
                _idleFrames = list.ToArray();
            }

            if (_boostFrames == null || _boostFrames.Length == 0)
            {
                var list = new List<Sprite>();
                for (int i = 0; i < 8; i++)
                {
                    var s = PrototypeSceneSetup.LoadSpriteFromFile($"PlayerFighter/Thruster/Frames/thrust_boost_{i:02d}.png", 100f);
                    if (s != null) list.Add(s);
                }
                _boostFrames = list.ToArray();
            }
        }
    }
}
