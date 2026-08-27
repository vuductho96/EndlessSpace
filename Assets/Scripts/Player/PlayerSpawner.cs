using UnityEngine;
using SpaceShooter.Core;
using SpaceShooter.Fighters.Data;
using SpaceShooter.Player.Abilities;
using SpaceShooter.Player.Visuals;

namespace SpaceShooter.Player
{
    public class PlayerSpawner : MonoBehaviour
    {
        public static PlayerSpawner Instance { get; private set; }

        [Header("Spawn Position")]
        [SerializeField] private Vector3 _defaultSpawnPosition = new Vector3(0f, -4f, 0f);

        public GameObject SpawnedPlayerObject { get; private set; }
        public PlayerController SpawnedPlayerController { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        public PlayerController SpawnSelectedFighter()
        {
            string selectedId = "vanguard";
            if (SaveManager.Instance != null && SaveManager.Instance.CurrentSave != null)
            {
                selectedId = SaveManager.Instance.CurrentSave.SelectedFighterId;
            }

            FighterDefinition def = FighterDatabase.Instance.GetFighterById(selectedId);
            return SpawnFighter(def);
        }

        public PlayerController SpawnFighter(FighterDefinition definition)
        {
            if (definition == null) definition = FighterDatabase.Instance.GetFighterByIndex(0);

            // Destroy any previous player instance
            if (SpawnedPlayerObject != null)
            {
                Destroy(SpawnedPlayerObject);
            }

            GameObject playerObj = null;

            if (definition.gameplayPrefab != null)
            {
                playerObj = Instantiate(definition.gameplayPrefab, _defaultSpawnPosition, Quaternion.identity);
            }
            else
            {
                // Pure Modular Procedural Composition (PF_Player_Base architecture)
                playerObj = CreateBaseFighterHierarchy(definition, _defaultSpawnPosition);
            }

            var controller = playerObj.GetComponent<PlayerController>();
            if (controller == null) controller = playerObj.AddComponent<PlayerController>();

            controller.Initialize(definition);

            SpawnedPlayerObject = playerObj;
            SpawnedPlayerController = controller;

            Debug.Log($"<color=#00FFCC><b>[PlayerSpawner]</b></color> Successfully spawned fighter: <b>{definition.displayName}</b> ({definition.className})");
            return controller;
        }

        public static GameObject CreateBaseFighterHierarchy(FighterDefinition def, Vector3 position)
        {
            GameObject player = new GameObject($"PlayerFighter_{def.fighterId}");
            player.tag = "Player";
            player.transform.position = position;

            // 1. GAMEPLAY COMPONENTS
            var rb = player.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.linearDamping = def.linearDamping;

            var col = player.AddComponent<CircleCollider2D>();
            col.radius = 1.5f;

            var stats = player.AddComponent<PlayerStats>();
            var move = player.AddComponent<PlayerMovement>();
            var health = player.AddComponent<PlayerHealth>();
            var shield = player.AddComponent<PlayerShield>();
            var weapons = player.AddComponent<PlayerWeaponSystem>();
            var abilities = player.AddComponent<PlayerAbilitySystem>();
            var visuals = player.AddComponent<FighterVisualController>();
            var controller = player.AddComponent<PlayerController>();

            // Attach specific signature ability based on fighter definition
            AttachAbilityByDefinition(player, def);

            // 2. VISUAL HIERARCHY
            GameObject visRoot = new GameObject("Visual");
            visRoot.transform.SetParent(player.transform, false);

            // Left Wing (Order 19)
            GameObject wingLeft = new GameObject("Wing_Left");
            wingLeft.transform.SetParent(visRoot.transform, false);
            var wingLSr = wingLeft.AddComponent<SpriteRenderer>();
            wingLSr.sortingOrder = 19;
            wingLSr.sprite = PrototypeSceneSetup.LoadSpriteFromFile(def.wingLeftSpritePath, 100f);
            wingLSr.color = def.wingTint;

            // Right Wing (Order 19)
            GameObject wingRight = new GameObject("Wing_Right");
            wingRight.transform.SetParent(visRoot.transform, false);
            var wingRSr = wingRight.AddComponent<SpriteRenderer>();
            wingRSr.sortingOrder = 19;
            wingRSr.sprite = PrototypeSceneSetup.LoadSpriteFromFile(def.wingRightSpritePath, 100f);
            wingRSr.color = def.wingTint;

            // Hull (Order 20)
            GameObject hullObj = new GameObject("Hull");
            hullObj.transform.SetParent(visRoot.transform, false);
            var hullSr = hullObj.AddComponent<SpriteRenderer>();
            hullSr.sortingOrder = 20;
            hullSr.sprite = PrototypeSceneSetup.LoadSpriteFromFile(def.hullSpritePath, 100f);
            hullSr.color = def.hullTint;

            // Cockpit (Order 22)
            GameObject cockpitObj = new GameObject("Cockpit");
            cockpitObj.transform.SetParent(visRoot.transform, false);
            var cockpitSr = cockpitObj.AddComponent<SpriteRenderer>();
            cockpitSr.sortingOrder = 22;
            cockpitSr.sprite = PrototypeSceneSetup.LoadSpriteFromFile(def.cockpitSpritePath, 100f);
            cockpitSr.color = def.cockpitTint;

            // Energy Core (Order 21)
            GameObject coreObj = new GameObject("EnergyCore");
            coreObj.transform.SetParent(visRoot.transform, false);
            var coreSr = coreObj.AddComponent<SpriteRenderer>();
            coreSr.sortingOrder = 21;
            coreSr.sprite = PrototypeSceneSetup.LoadSpriteFromFile(def.coreSpritePath, 100f);
            coreSr.color = def.coreGlowColor;

            // Shield Bubble (Order 24)
            GameObject shieldObj = new GameObject("ShieldBubble");
            shieldObj.transform.SetParent(visRoot.transform, false);
            shieldObj.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
            var shieldSr = shieldObj.AddComponent<SpriteRenderer>();
            shieldSr.sortingOrder = 24;
            shieldSr.sprite = PrototypeSceneSetup.LoadSpriteFromFile(def.shieldSpritePath, 100f);
            Color sColor = def.themeColor;
            sColor.a = 0.35f;
            shieldSr.color = sColor;

            // 3. MOUNTS HIERARCHY
            GameObject mountsRoot = new GameObject("Mounts");
            mountsRoot.transform.SetParent(player.transform, false);

            // Default Dual Mounts or Triple Mounts for Striker/Void Wraith
            int mountCount = (def.fighterId == "striker" || def.fighterId == "void_wraith") ? 3 : 2;
            if (mountCount == 3)
            {
                CreateMount(mountsRoot, "WeaponMount_01", new Vector3(-1.4f, 0.5f, 0f));
                CreateMount(mountsRoot, "WeaponMount_02", new Vector3(0f, 1.8f, 0f)); // Center Heavy Mount
                CreateMount(mountsRoot, "WeaponMount_03", new Vector3(1.4f, 0.5f, 0f));
            }
            else
            {
                CreateMount(mountsRoot, "WeaponMount_01", new Vector3(-1.35f, 0.5f, 0f));
                CreateMount(mountsRoot, "WeaponMount_02", new Vector3(1.35f, 0.5f, 0f));
            }

            // 4. THRUSTER SYSTEM
            GameObject thrusterRoot = new GameObject("Thrusters");
            thrusterRoot.transform.SetParent(player.transform, false);
            var tc = thrusterRoot.AddComponent<ThrusterController>();

            GameObject flameL = new GameObject("Flame_Left");
            flameL.transform.SetParent(thrusterRoot.transform, false);
            flameL.transform.localPosition = new Vector3(-1.35f, -3.65f, 0f);
            var flameLSr = flameL.AddComponent<SpriteRenderer>();
            flameLSr.sortingOrder = 18;
            flameLSr.sprite = PrototypeSceneSetup.LoadSpriteFromFile("PlayerFighter/Thruster/PlasmaFrames/xenon_plasma_idle_00.png", 100f, new Vector2(0.5f, 1.0f));
            flameLSr.color = def.thrusterPlasmaColor;

            GameObject flameR = new GameObject("Flame_Right");
            flameR.transform.SetParent(thrusterRoot.transform, false);
            flameR.transform.localPosition = new Vector3(1.35f, -3.65f, 0f);
            var flameRSr = flameR.AddComponent<SpriteRenderer>();
            flameRSr.sortingOrder = 18;
            flameRSr.sprite = PrototypeSceneSetup.LoadSpriteFromFile("PlayerFighter/Thruster/PlasmaFrames/xenon_plasma_idle_00.png", 100f, new Vector2(0.5f, 1.0f));
            flameRSr.color = def.thrusterPlasmaColor;

            var liveFlame = player.AddComponent<LiveThrusterFlame>();
            liveFlame.SetRenderers(flameLSr, flameRSr);

            // Rim light response
            var lightResp = player.AddComponent<PlayerLightingResponse>();
            var rimField = typeof(PlayerLightingResponse).GetField("_rimLightRenderer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (rimField != null) rimField.SetValue(lightResp, shieldSr);

            return player;
        }

        private static void CreateMount(GameObject parent, string mountName, Vector3 localPos)
        {
            GameObject mObj = new GameObject(mountName);
            mObj.transform.SetParent(parent.transform, false);
            mObj.transform.localPosition = localPos;
        }

        private static void AttachAbilityByDefinition(GameObject player, FighterDefinition def)
        {
            if (def == null)
            {
                player.AddComponent<DashAbility>();
                return;
            }

            string abilityStr = $"{def.specialAbilityName} {def.signatureTechnology} {def.fighterId}".ToLowerInvariant();

            if (abilityStr.Contains("overdrive") || abilityStr.Contains("apex") || abilityStr.Contains("surge") || abilityStr.Contains("quantum"))
            {
                player.AddComponent<OverdriveAbility>();
            }
            else if (abilityStr.Contains("rail") || abilityStr.Contains("striker") || abilityStr.Contains("raven") || abilityStr.Contains("siege") || abilityStr.Contains("cannon") || abilityStr.Contains("barrage"))
            {
                player.AddComponent<RailShotAbility>();
            }
            else if (abilityStr.Contains("shield") || abilityStr.Contains("burst") || abilityStr.Contains("bulwark") || abilityStr.Contains("fortress") || abilityStr.Contains("barrier"))
            {
                player.AddComponent<ShieldBurstAbility>();
            }
            else if (abilityStr.Contains("phase") || abilityStr.Contains("shift") || abilityStr.Contains("phantom") || abilityStr.Contains("void") || abilityStr.Contains("stealth"))
            {
                player.AddComponent<PhaseShiftAbility>();
            }
            else
            {
                player.AddComponent<DashAbility>();
            }
        }
    }
}
