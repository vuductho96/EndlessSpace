using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SpaceShooter.Core;
using SpaceShooter.Cameras;
using SpaceShooter.Environment;
using SpaceShooter.Player;
using SpaceShooter.Combat;
using SpaceShooter.UI;
using SpaceShooter.Input;
using SpaceShooter.Performance;
using SpaceShooter.Audio;

namespace SpaceShooter
{
    public class PrototypeSceneSetup : MonoBehaviour
    {
        [Header("Constant Gameplay World Scales")]
        [SerializeField] private float _playerConstantWorldScale = 0.3f;
        [SerializeField] private float _targetConstantWorldScale = 0.35f;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void InitializeSceneIfEmpty()
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Prototype_SpaceCombat")
            {
                return;
            }

            if (GameObject.FindWithTag("Player") != null || GameObject.Find("PlayerShip") != null)
            {
                Debug.Log("[PrototypeSceneSetup] Scene already populated with GameObjects. Ready!");
                return;
            }

            if (GameObject.Find("Prototype_SpaceCombat_Manager") != null) return;

            GameObject root = new GameObject("Prototype_SpaceCombat_Manager");
            root.AddComponent<PrototypeSceneSetup>().BuildSceneHierarchy();
        }

        public static Sprite LoadSpriteFromFile(string relativeAssetPath, float pixelsPerUnit = 100f)
        {
            return LoadSpriteFromFile(relativeAssetPath, pixelsPerUnit, new Vector2(0.5f, 0.5f));
        }

        public static Sprite LoadSpriteFromFile(string relativeAssetPath, float pixelsPerUnit, Vector2 pivot)
        {
            string fullPath = Path.Combine(Application.dataPath, relativeAssetPath);
            if (!File.Exists(fullPath))
            {
                Debug.LogWarning($"[PrototypeSceneSetup] Sprite not found at: {fullPath}");
                return null;
            }

            byte[] fileData = File.ReadAllBytes(fullPath);
            Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (tex.LoadImage(fileData))
            {
                tex.filterMode = FilterMode.Bilinear;
                tex.wrapMode = TextureWrapMode.Repeat;
                return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), pivot, pixelsPerUnit);
            }
            return null;
        }

        public void BuildSceneHierarchy()
        {
            Debug.Log("[PrototypeSceneSetup] Assembling Pure World-Scale Architecture...");

            // ==========================================
            // 1. CORE & PERFORMANCE MANAGERS
            // ==========================================
            GameObject sysObj = new GameObject("Systems");
            sysObj.AddComponent<SaveManager>();
            sysObj.AddComponent<PlayTimeManager>();
            sysObj.AddComponent<PlayerSpawner>();
            sysObj.AddComponent<SettingsManager>();
            sysObj.AddComponent<QualityManager>();
            sysObj.AddComponent<PerformanceManager>();
            sysObj.AddComponent<AudioManager>();
            sysObj.AddComponent<GameStateManager>();
            sysObj.AddComponent<ObjectPooler>();
            sysObj.AddComponent<InputManager>();
            sysObj.AddComponent<AutoAimSystem>();
            var lightCtrl = sysObj.AddComponent<SpaceLightingController>();

            // ==========================================
            // 2. CAMERA (DEDICATED GAMEPLAY CAMERA CONTROLLER)
            // ==========================================
            UnityEngine.Camera cam = UnityEngine.Camera.main;
            if (cam == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                cam = camObj.AddComponent<UnityEngine.Camera>();
                camObj.tag = "MainCamera";
            }
            cam.orthographic = true;
            cam.backgroundColor = new Color(0.015f, 0.025f, 0.05f, 1f);

            var gameCam = cam.gameObject.GetComponent<GameplayCameraController>() ?? cam.gameObject.AddComponent<GameplayCameraController>();
            gameCam.RecalculateOrthographicSize(forceInstant: true);

            // EventSystem with New Input System Module & Default Actions Assigned
            EventSystem existingEs = FindAnyObjectByType<EventSystem>();
            if (existingEs == null)
            {
                GameObject esObj = new GameObject("EventSystem");
                existingEs = esObj.AddComponent<EventSystem>();
                var mod = esObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
                mod.AssignDefaultActions();
            }
            else
            {
                var legacyModule = existingEs.GetComponent<StandaloneInputModule>();
                if (legacyModule != null) Object.DestroyImmediate(legacyModule);

                var mod = existingEs.GetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
                if (mod == null)
                {
                    mod = existingEs.gameObject.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
                }
                mod.AssignDefaultActions();
            }

            // ==========================================
            // 3. MULTI-LAYER STARFIELD PARALLAX
            // ==========================================
            GameObject envObj = new GameObject("Environment");
            var parallax = envObj.AddComponent<InfiniteBackground>();

            // Starfield Layer
            GameObject starfieldObj = new GameObject("Deep_Starfield_Layer");
            starfieldObj.transform.SetParent(envObj.transform);
            var sfSr = starfieldObj.AddComponent<SpriteRenderer>();
            sfSr.sortingOrder = 0;
            sfSr.sprite = LoadSpriteFromFile("Environment/starfield_deep_space.png", 64f);
            starfieldObj.transform.localScale = new Vector3(6f, 6f, 1f);

            // Nebula Layer
            GameObject nebulaObj = new GameObject("Nebula_Layer");
            nebulaObj.transform.SetParent(envObj.transform);
            var nebSr = nebulaObj.AddComponent<SpriteRenderer>();
            nebSr.sortingOrder = 5;
            nebSr.sprite = LoadSpriteFromFile("Environment/nebula_backdrop.png", 64f);
            nebSr.color = new Color(1f, 1f, 1f, 0.5f);
            nebulaObj.transform.localScale = new Vector3(6f, 6f, 1f);

            // Dynamic Ambient Lighting
            GameObject lightOverlayObj = new GameObject("Lighting_Overlay");
            lightOverlayObj.transform.SetParent(cam.transform);
            lightOverlayObj.transform.localPosition = new Vector3(0, 0, 10f);
            var lightSr = lightOverlayObj.AddComponent<SpriteRenderer>();
            lightSr.sortingOrder = 10;
            lightSr.sprite = LoadSpriteFromFile("DynamicLighting/Stellar/stellar_light_overlay.png", 50f);
            lightSr.color = new Color(0.8f, 0.9f, 1.0f, 0.12f);
            lightOverlayObj.transform.localScale = new Vector3(8f, 8f, 1f);

            var ambField = typeof(SpaceLightingController).GetField("_ambientOverlay", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (ambField != null) ambField.SetValue(lightCtrl, lightSr);

            var layerArray = new InfiniteBackground.ParallaxLayer[]
            {
                new InfiniteBackground.ParallaxLayer { name = "Starfield", renderer = sfSr, parallaxFactor = 0.05f },
                new InfiniteBackground.ParallaxLayer { name = "Nebula", renderer = nebSr, parallaxFactor = 0.25f }
            };
            var plField = typeof(InfiniteBackground).GetField("_layers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (plField != null) plField.SetValue(parallax, layerArray);

            // ==========================================
            // 4. DATA-DRIVEN PLAYER FIGHTER SPAWN
            // ==========================================
            string selectedId = SaveManager.Instance != null && SaveManager.Instance.CurrentSave != null ?
                SaveManager.Instance.CurrentSave.SelectedFighterId : "vanguard";
            var fighterDef = SpaceShooter.Fighters.Data.FighterDatabase.Instance.GetFighterById(selectedId);

            var spawner = sysObj.GetComponent<PlayerSpawner>();
            var pController = spawner.SpawnFighter(fighterDef);
            pController.SetShipScale(_playerConstantWorldScale);
            GameObject player = pController.gameObject;

            gameCam.SetTarget(player.transform);

            // ==========================================
            // 5. TRAINING TARGET (CONSTANT WORLD SCALE 0.35f)
            // ==========================================
            GameObject target = new GameObject("TrainingTarget");
            target.transform.position = new Vector3(0f, 6.0f, 0f);
            var targetSr = target.AddComponent<SpriteRenderer>();
            targetSr.sortingOrder = 20;
            targetSr.sprite = LoadSpriteFromFile("PlayerFighter/Ship/player_fighter_hull.png", 100f);
            targetSr.color = new Color(1f, 0.35f, 0.35f, 1f);
            target.transform.localScale = new Vector3(_targetConstantWorldScale, _targetConstantWorldScale, 1f);
            target.transform.rotation = Quaternion.Euler(0, 0, 180f);
            var targetCol = target.AddComponent<CircleCollider2D>();
            targetCol.radius = 1.5f;
            target.AddComponent<TrainingTarget>();

            // ==========================================
            // 6. ANDROID RESPONSIVE TOUCH UI & HUD
            // ==========================================
            GameObject canvasObj = new GameObject("Canvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObj.AddComponent<GraphicRaycaster>();
            canvasObj.AddComponent<ResponsiveUIManager>();
            var devControls = canvasObj.AddComponent<DeveloperTestControls>();

            // 6a. Responsive Virtual Joystick & Touch Controls
            GameObject touchOverlay = new GameObject("TouchControlsRoot");
            touchOverlay.transform.SetParent(canvasObj.transform, false);
            var touchRt = touchOverlay.AddComponent<RectTransform>();
            touchRt.anchorMin = Vector2.zero;
            touchRt.anchorMax = Vector2.one;
            touchRt.sizeDelta = Vector2.zero;
            touchOverlay.AddComponent<SafeAreaController>();

            // Virtual Joystick Base (Left Thumb)
            GameObject joyBg = new GameObject("JoystickBG");
            joyBg.transform.SetParent(touchOverlay.transform, false);
            var joyBgImg = joyBg.AddComponent<Image>();
            joyBgImg.sprite = LoadSpriteFromFile("UI/Touch/touch_joystick_base.png", 100f);
            joyBgImg.color = new Color(1f, 1f, 1f, 0.85f);
            joyBgImg.raycastTarget = true; // MUST receive pointer raycasts directly!
            var joyBgRt = joyBg.GetComponent<RectTransform>();
            joyBgRt.anchorMin = new Vector2(0f, 0f);
            joyBgRt.anchorMax = new Vector2(0f, 0f);
            joyBgRt.pivot = new Vector2(0.5f, 0.5f);
            joyBgRt.anchoredPosition = new Vector2(200f, 200f);
            joyBgRt.sizeDelta = new Vector2(220f, 220f);

            // Joystick Handle Knob
            GameObject joyHandle = new GameObject("JoystickHandle");
            joyHandle.transform.SetParent(joyBg.transform, false);
            var joyHandleImg = joyHandle.AddComponent<Image>();
            joyHandleImg.sprite = LoadSpriteFromFile("UI/Touch/touch_joystick_knob.png", 100f);
            joyHandleImg.color = new Color(1f, 1f, 1f, 0.95f);
            joyHandleImg.raycastTarget = false; // Knob lets clicks pass to background
            var joyHandleRt = joyHandle.GetComponent<RectTransform>();
            joyHandleRt.sizeDelta = new Vector2(95f, 95f);

            var vj = joyBg.AddComponent<VirtualJoystick>();
            vj.Setup(joyBgRt, joyHandleRt, 95f);

            // Action Buttons (Right Thumb)
            // Fire Button
            GameObject fireBtn = new GameObject("FireButton");
            fireBtn.transform.SetParent(touchOverlay.transform, false);
            var fireImg = fireBtn.AddComponent<Image>();
            fireImg.sprite = LoadSpriteFromFile("UI/Touch/touch_button_fire.png", 100f);
            fireImg.color = Color.white;
            fireImg.raycastTarget = true;
            var fireRt = fireBtn.GetComponent<RectTransform>();
            fireRt.anchorMin = new Vector2(1f, 0f);
            fireRt.anchorMax = new Vector2(1f, 0f);
            fireRt.pivot = new Vector2(0.5f, 0.5f);
            fireRt.anchoredPosition = new Vector2(-150f, 150f);
            fireRt.sizeDelta = new Vector2(130f, 130f);

            var fireAction = fireBtn.AddComponent<TouchActionButton>();
            fireAction.Setup(ActionButtonType.Fire);

            // Boost Button
            GameObject boostBtn = new GameObject("BoostButton");
            boostBtn.transform.SetParent(touchOverlay.transform, false);
            var boostImg = boostBtn.AddComponent<Image>();
            boostImg.sprite = LoadSpriteFromFile("UI/Touch/touch_button_boost.png", 100f);
            boostImg.color = Color.white;
            boostImg.raycastTarget = true;
            var boostRt = boostBtn.GetComponent<RectTransform>();
            boostRt.anchorMin = new Vector2(1f, 0f);
            boostRt.anchorMax = new Vector2(1f, 0f);
            boostRt.pivot = new Vector2(0.5f, 0.5f);
            boostRt.anchoredPosition = new Vector2(-265f, 95f);
            boostRt.sizeDelta = new Vector2(100f, 100f);

            var boostAction = boostBtn.AddComponent<TouchActionButton>();
            boostAction.Setup(ActionButtonType.Boost);

            // 6b. Targeting Reticle
            GameObject reticleObj = new GameObject("TargetReticle");
            reticleObj.transform.SetParent(canvasObj.transform, false);
            var reticleImg = reticleObj.AddComponent<Image>();
            reticleImg.sprite = LoadSpriteFromFile("UI/Targeting/target_reticle.png", 100f);
            reticleImg.rectTransform.sizeDelta = new Vector2(70f, 70f);
            reticleImg.color = new Color(0f, 0.9f, 1f, 0.8f);
            reticleImg.raycastTarget = false; // Never block touch inputs

            var hudCtrl = canvasObj.AddComponent<HUDController>();
            var retField = typeof(HUDController).GetField("_reticleTransform", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (retField != null) retField.SetValue(hudCtrl, reticleImg.rectTransform);

            // 6c. Telemetry Debug Panel (Toggleable F11/F12)
            GameObject debugObj = new GameObject("DebugPanel");
            debugObj.transform.SetParent(canvasObj.transform, false);
            var debugText = debugObj.AddComponent<Text>();
            debugText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
            debugText.fontSize = 18;
            debugText.color = new Color(0f, 1f, 1f, 0.95f);
            debugText.raycastTarget = false; // Never block touch inputs
            var debugRect = debugText.GetComponent<RectTransform>();
            debugRect.anchorMin = new Vector2(0f, 1f);
            debugRect.anchorMax = new Vector2(0f, 1f);
            debugRect.pivot = new Vector2(0f, 1f);
            debugRect.anchoredPosition = new Vector2(30f, -30f);
            debugRect.sizeDelta = new Vector2(850f, 320f);

            var debugPanel = canvasObj.AddComponent<DebugPanel>();
            var pRootField = typeof(DebugPanel).GetField("_panelRoot", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (pRootField != null) pRootField.SetValue(debugPanel, debugObj);
            var pTextField = typeof(DebugPanel).GetField("_debugText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (pTextField != null) pTextField.SetValue(debugPanel, debugText);

            var devDbgField = typeof(DeveloperTestControls).GetField("_debugPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (devDbgField != null) devDbgField.SetValue(devControls, debugPanel);
            var devParallaxField = typeof(DeveloperTestControls).GetField("_parallax", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (devParallaxField != null) devParallaxField.SetValue(devControls, parallax);

            canvasObj.AddComponent<FighterArchitectureDebugPanel>();

            Debug.Log("[PrototypeSceneSetup] GameplayCameraController and World Scale Configured!");
        }
    }
}
