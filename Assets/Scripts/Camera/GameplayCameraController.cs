using UnityEngine;

namespace SpaceShooter.Cameras
{
    /// <summary>
    /// Dedicated camera controller for 2D top-down space combat.
    /// Preserves a consistent horizontal/vertical gameplay corridor across all aspect ratios
    /// without distorting or resizing world objects.
    /// </summary>
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class GameplayCameraController : MonoBehaviour
    {
        public static GameplayCameraController Instance { get; private set; }

        [Header("Target Tracking")]
        [SerializeField] private Transform _target;
        [SerializeField] private float _smoothSpeed = 10f;
        [SerializeField] private Vector3 _offset = new Vector3(0f, 0f, -10f);

        [Header("Gameplay Reference Corridor (World Units)")]
        [Tooltip("Minimum horizontal world units visible in portrait mode (fixed width policy).")]
        [SerializeField] private float _portraitTargetWidth = 18f;

        [Tooltip("Reference vertical world units visible in landscape 16:9 mode.")]
        [SerializeField] private float _landscapeTargetHeight = 18f;

        [Header("Smoothing")]
        [SerializeField] private bool _smoothZoom = true;
        [SerializeField] private float _zoomSpeed = 8f;

        private UnityEngine.Camera _cam;
        private float _targetOrthoSize;
        private int _lastScreenWidth = -1;
        private int _lastScreenHeight = -1;

        // Telemetry properties
        public float OrthographicSize => _cam != null ? _cam.orthographicSize : _targetOrthoSize;
        public float CameraAspect => _cam != null ? _cam.aspect : (float)Screen.width / Screen.height;
        public float WorldVisibleHeight => _cam != null ? _cam.orthographicSize * 2f : 0f;
        public float WorldVisibleWidth => _cam != null ? WorldVisibleHeight * _cam.aspect : 0f;
        public bool IsPortrait => Screen.height > Screen.width;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            _cam = GetComponent<UnityEngine.Camera>();
            _cam.orthographic = true;
            RecalculateOrthographicSize(forceInstant: true);
        }

        private void Update()
        {
            if (Screen.width != _lastScreenWidth || Screen.height != _lastScreenHeight)
            {
                RecalculateOrthographicSize(forceInstant: false);
            }

            if (_smoothZoom && _cam != null)
            {
                _cam.orthographicSize = Mathf.Lerp(_cam.orthographicSize, _targetOrthoSize, Time.unscaledDeltaTime * _zoomSpeed);
            }
        }

        private void LateUpdate()
        {
            if (_target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) _target = player.transform;
            }

            if (_target != null)
            {
                Vector3 desiredPos = _target.position + _offset;
                transform.position = Vector3.Lerp(transform.position, desiredPos, _smoothSpeed * Time.deltaTime);
            }
        }

        /// <summary>
        /// Mathematically calculates the exact orthographicSize needed to preserve
        /// the intended gameplay arena corridor across any aspect ratio (Portrait 3:4, 9:16, 9:20, Landscape 16:9, 21:9).
        /// </summary>
        public void RecalculateOrthographicSize(bool forceInstant = false)
        {
            _lastScreenWidth = Screen.width;
            _lastScreenHeight = Screen.height;

            if (_cam == null) _cam = GetComponent<UnityEngine.Camera>();
            if (_cam == null) return;

            float currentAspect = (float)Screen.width / Screen.height;

            if (currentAspect < 1.0f) // Portrait orientation
            {
                // In Portrait: preserve horizontal combat width W = 2 * OrthoSize * Aspect >= _portraitTargetWidth
                // OrthoSize = _portraitTargetWidth / (2 * Aspect)
                _targetOrthoSize = _portraitTargetWidth / (2f * Mathf.Max(0.01f, currentAspect));
            }
            else // Landscape orientation
            {
                // In Landscape: standard vertical reference height H = 2 * OrthoSize = _landscapeTargetHeight
                // If the screen is narrower than 16:9 in landscape (e.g. 4:3), ensure min width is also preserved
                float defaultLandscapeOrtho = _landscapeTargetHeight / 2f;
                float landscapeWidth = defaultLandscapeOrtho * 2f * currentAspect;

                if (landscapeWidth < _portraitTargetWidth)
                {
                    _targetOrthoSize = _portraitTargetWidth / (2f * Mathf.Max(0.01f, currentAspect));
                }
                else
                {
                    _targetOrthoSize = defaultLandscapeOrtho;
                }
            }

            if (forceInstant)
            {
                _cam.orthographicSize = _targetOrthoSize;
            }
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }
    }
}
