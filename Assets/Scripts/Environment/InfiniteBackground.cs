using UnityEngine;

namespace SpaceShooter.Environment
{
    public class InfiniteBackground : MonoBehaviour
    {
        [System.Serializable]
        public class ParallaxLayer
        {
            public string name;
            public SpriteRenderer renderer;
            public float parallaxFactor = 0.1f;
            [HideInInspector] public Vector3 startPos;
        }

        [Header("Camera & Layers")]
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private ParallaxLayer[] _layers;
        [SerializeField] private bool _enableParallax = true;

        private Vector3 _lastCameraPos;

        private void Start()
        {
            if (_cameraTransform == null && UnityEngine.Camera.main != null)
            {
                _cameraTransform = UnityEngine.Camera.main.transform;
            }
            if (_cameraTransform != null)
            {
                _lastCameraPos = _cameraTransform.position;
            }

            if (_layers != null)
            {
                foreach (var l in _layers)
                {
                    if (l.renderer != null) l.startPos = l.renderer.transform.position;
                }
            }
        }

        private void LateUpdate()
        {
            if (_cameraTransform == null || !_enableParallax) return;

            Vector3 delta = _cameraTransform.position - _lastCameraPos;
            _lastCameraPos = _cameraTransform.position;

            if (_layers == null) return;

            foreach (var l in _layers)
            {
                if (l.renderer != null)
                {
                    // Move layer relative to camera by parallax factor
                    l.renderer.transform.position += new Vector3(delta.x * l.parallaxFactor, delta.y * l.parallaxFactor, 0f);

                    // Infinite tiling offset check
                    Vector3 camPos = _cameraTransform.position;
                    Vector3 layerPos = l.renderer.transform.position;
                    float repeatDist = 20f;

                    if (Mathf.Abs(camPos.x - layerPos.x) > repeatDist)
                    {
                        layerPos.x = camPos.x;
                    }
                    if (Mathf.Abs(camPos.y - layerPos.y) > repeatDist)
                    {
                        layerPos.y = camPos.y;
                    }
                    l.renderer.transform.position = layerPos;
                }
            }
        }

        public void ToggleParallax()
        {
            _enableParallax = !_enableParallax;
        }
    }
}
