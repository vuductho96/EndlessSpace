using System;
using System.Collections.Generic;
using UnityEngine;
using SpaceShooter.Combat.Data;

namespace SpaceShooter.Fighters.Data
{
    [CreateAssetMenu(fileName = "FighterDatabase", menuName = "SpaceShooter/Fighter/Fighter Database")]
    public class FighterDatabase : ScriptableObject
    {
        private static FighterDatabase _instance;
        public static FighterDatabase Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<FighterDatabase>("FighterDatabase");
                    if (_instance == null)
                    {
                        _instance = CreateInstance<FighterDatabase>();
                        _instance.InitializeDefaultDefinitions();
                    }
                }
                return _instance;
            }
        }

        [SerializeField] private List<FighterDefinition> _fighters = new List<FighterDefinition>();
        public IReadOnlyList<FighterDefinition> Fighters => _fighters;

        public void InitializeDefaultDefinitions()
        {
            if (_fighters != null && _fighters.Count > 0) return;

            _fighters = new List<FighterDefinition>
            {
                // 1. VANGUARD (Balanced, 0 min)
                CreateDefinition(
                    id: "vanguard",
                    displayName: "VANGUARD",
                    className: "BALANCED FIGHTER",
                    description: "Standard frontline air-space superiority fighter equipped with adaptable dual plasma converters and balanced composite hull.",
                    tech: "Adaptive Core",
                    special: "Emergency Surge",
                    moveSpeed: 12f, boostSpeed: 22f, acceleration: 45f,
                    health: 100f, shield: 100f, energy: 100f,
                    mobilityRating: 0.65f, firepowerRating: 0.65f, defenseRating: 0.60f, specialRating: 0.60f,
                    unlockMinutes: 0f, isDefault: true,
                    theme: new Color(0f, 0.85f, 1f, 1f),
                    accent: new Color(1f, 0.65f, 0.1f, 1f),
                    hullTint: Color.white,
                    wingTint: Color.white,
                    cockpitTint: new Color(0f, 0.9f, 1f, 0.95f),
                    coreGlow: new Color(0f, 1f, 1f, 1f),
                    plasmaColor: new Color(0.2f, 0.8f, 1f, 1f),
                    weaponName: "Dual Plasma Converters",
                    weaponDamage: 25f, weaponFireRate: 0.12f
                ),

                // 2. INTERCEPTOR (High Mobility, 10 min)
                CreateDefinition(
                    id: "interceptor",
                    displayName: "INTERCEPTOR",
                    className: "HIGH MOBILITY",
                    description: "High-thrust interceptor fitted with lightweight composite wings and vector RCS micro-thrusters for rapid dogfight maneuvers.",
                    tech: "Vector Drive",
                    special: "Vector Shift",
                    moveSpeed: 16f, boostSpeed: 30f, acceleration: 65f,
                    health: 80f, shield: 85f, energy: 125f,
                    mobilityRating: 0.95f, firepowerRating: 0.55f, defenseRating: 0.40f, specialRating: 0.75f,
                    unlockMinutes: 10f, isDefault: false,
                    theme: new Color(0.1f, 1f, 0.65f, 1f),
                    accent: new Color(1f, 0.9f, 0.2f, 1f),
                    hullTint: new Color(0.85f, 1f, 0.9f, 1f),
                    wingTint: new Color(0.7f, 1f, 0.85f, 1f),
                    cockpitTint: new Color(0.1f, 1f, 0.6f, 0.95f),
                    coreGlow: new Color(0.2f, 1f, 0.7f, 1f),
                    plasmaColor: new Color(0.1f, 1f, 0.8f, 1f),
                    weaponName: "Twin Rapid Repeaters",
                    weaponDamage: 18f, weaponFireRate: 0.08f
                ),

                // 3. STRIKER (Heavy Firepower, 30 min)
                CreateDefinition(
                    id: "striker",
                    displayName: "STRIKER",
                    className: "HEAVY ASSAULT",
                    description: "Devastating assault craft carrying reinforced heavy weapons mountings and magnetic rail accelerator capacitors.",
                    tech: "Rail Core",
                    special: "Siege Barrage",
                    moveSpeed: 10.5f, boostSpeed: 19f, acceleration: 38f,
                    health: 120f, shield: 95f, energy: 85f,
                    mobilityRating: 0.45f, firepowerRating: 0.95f, defenseRating: 0.65f, specialRating: 0.70f,
                    unlockMinutes: 30f, isDefault: false,
                    theme: new Color(1f, 0.35f, 0.15f, 1f),
                    accent: new Color(1f, 0.8f, 0.1f, 1f),
                    hullTint: new Color(1f, 0.85f, 0.8f, 1f),
                    wingTint: new Color(1f, 0.7f, 0.65f, 1f),
                    cockpitTint: new Color(1f, 0.4f, 0.1f, 0.95f),
                    coreGlow: new Color(1f, 0.45f, 0.1f, 1f),
                    plasmaColor: new Color(1f, 0.55f, 0.15f, 1f),
                    weaponName: "Heavy Rail Accelerators",
                    weaponDamage: 45f, weaponFireRate: 0.22f
                ),

                // 4. BULWARK (Defense Juggernaut, 60 min)
                CreateDefinition(
                    id: "bulwark",
                    displayName: "BULWARK",
                    className: "DEFENSE JUGGERNAUT",
                    description: "Armored dread-chassis with dual layered phase barrier emitters, built to withstand extreme punishment in heavy combat zones.",
                    tech: "Barrier Matrix",
                    special: "Fortress Dome",
                    moveSpeed: 9f, boostSpeed: 16f, acceleration: 30f,
                    health: 160f, shield: 150f, energy: 90f,
                    mobilityRating: 0.35f, firepowerRating: 0.60f, defenseRating: 1.0f, specialRating: 0.80f,
                    unlockMinutes: 60f, isDefault: false,
                    theme: new Color(0.2f, 0.6f, 1f, 1f),
                    accent: new Color(0.4f, 0.85f, 1f, 1f),
                    hullTint: new Color(0.8f, 0.9f, 1f, 1f),
                    wingTint: new Color(0.75f, 0.85f, 1f, 1f),
                    cockpitTint: new Color(0.3f, 0.7f, 1f, 0.95f),
                    coreGlow: new Color(0.2f, 0.75f, 1f, 1f),
                    plasmaColor: new Color(0.3f, 0.65f, 1f, 1f),
                    weaponName: "Ion Flak Cannons",
                    weaponDamage: 30f, weaponFireRate: 0.16f
                ),

                // 5. RAVEN (Precision Sniper, 120 min)
                CreateDefinition(
                    id: "raven",
                    displayName: "RAVEN",
                    className: "PRECISION SNIPER",
                    description: "High-spec electronic warfare and long-range precision interceptor with phase beam harmonic lenses.",
                    tech: "Precision Core",
                    special: "Hyper-Focus Matrix",
                    moveSpeed: 13.5f, boostSpeed: 24f, acceleration: 50f,
                    health: 90f, shield: 110f, energy: 115f,
                    mobilityRating: 0.75f, firepowerRating: 0.85f, defenseRating: 0.50f, specialRating: 0.85f,
                    unlockMinutes: 120f, isDefault: false,
                    theme: new Color(0.75f, 0.3f, 1f, 1f),
                    accent: new Color(0.95f, 0.5f, 1f, 1f),
                    hullTint: new Color(0.9f, 0.8f, 1f, 1f),
                    wingTint: new Color(0.85f, 0.75f, 1f, 1f),
                    cockpitTint: new Color(0.8f, 0.35f, 1f, 0.95f),
                    coreGlow: new Color(0.85f, 0.4f, 1f, 1f),
                    plasmaColor: new Color(0.75f, 0.35f, 1f, 1f),
                    weaponName: "Phase Beam Lances",
                    weaponDamage: 38f, weaponFireRate: 0.14f
                ),

                // 6. PHANTOM (Phase / Void, 240 min)
                CreateDefinition(
                    id: "phantom",
                    displayName: "PHANTOM",
                    className: "PHASE / VOID",
                    description: "Dark-matter stealth prototype capable of dimensional phase shifts, slipping through projectile fire unharmed.",
                    tech: "Phase Core",
                    special: "Dimensional Phase",
                    moveSpeed: 15f, boostSpeed: 28f, acceleration: 60f,
                    health: 85f, shield: 120f, energy: 130f,
                    mobilityRating: 0.90f, firepowerRating: 0.80f, defenseRating: 0.45f, specialRating: 1.0f,
                    unlockMinutes: 240f, isDefault: false,
                    theme: new Color(0.9f, 0.1f, 0.5f, 1f),
                    accent: new Color(0.3f, 0.95f, 1f, 1f),
                    hullTint: new Color(0.75f, 0.7f, 0.85f, 1f),
                    wingTint: new Color(0.7f, 0.65f, 0.8f, 1f),
                    cockpitTint: new Color(0.9f, 0.2f, 0.6f, 0.95f),
                    coreGlow: new Color(0.95f, 0.2f, 0.65f, 1f),
                    plasmaColor: new Color(0.85f, 0.15f, 0.55f, 1f),
                    weaponName: "Void Pulse Disruptors",
                    weaponDamage: 32f, weaponFireRate: 0.11f
                ),

                // 7. APEX (Experimental Quantum, 480 min)
                CreateDefinition(
                    id: "apex",
                    displayName: "APEX",
                    className: "EXPERIMENTAL QUANTUM",
                    description: "Apex flagship experimental prototype harnessing quantum resonance tachyon fields to bend combat physics.",
                    tech: "Quantum Core",
                    special: "Resonance Cascade",
                    moveSpeed: 15.5f, boostSpeed: 29f, acceleration: 62f,
                    health: 125f, shield: 130f, energy: 140f,
                    mobilityRating: 0.95f, firepowerRating: 0.95f, defenseRating: 0.90f, specialRating: 0.95f,
                    unlockMinutes: 480f, isDefault: false,
                    theme: new Color(1f, 0.85f, 0.2f, 1f),
                    accent: new Color(0f, 1f, 0.9f, 1f),
                    hullTint: new Color(1f, 0.95f, 0.8f, 1f),
                    wingTint: new Color(1f, 0.9f, 0.75f, 1f),
                    cockpitTint: new Color(1f, 0.8f, 0.2f, 0.95f),
                    coreGlow: new Color(1f, 0.9f, 0.3f, 1f),
                    plasmaColor: new Color(1f, 0.85f, 0.25f, 1f),
                    weaponName: "Quantum Tachyon Arrays",
                    weaponDamage: 40f, weaponFireRate: 0.10f
                ),

                // 8. TEST_FIGHTER / VOID WRAITH (Created purely via Data + Reusable Components)
                CreateDefinition(
                    id: "void_wraith",
                    displayName: "VOID WRAITH",
                    className: "RECON ELITE PROTOTYPE",
                    description: "Experimental deep-space interceptor testing 100% data-driven component modularity with triple weapon hardpoints and Phase Shift capability.",
                    tech: "Wraith Phase Shifter",
                    special: "Dimensional Phasing",
                    moveSpeed: 16.5f, boostSpeed: 31f, acceleration: 68f,
                    health: 95f, shield: 115f, energy: 135f,
                    mobilityRating: 0.98f, firepowerRating: 0.88f, defenseRating: 0.55f, specialRating: 0.92f,
                    unlockMinutes: 15f, isDefault: false,
                    theme: new Color(0.4f, 0f, 0.9f, 1f), // Deep Violet
                    accent: new Color(0f, 1f, 0.8f, 1f), // Cyan highlight
                    hullTint: new Color(0.7f, 0.65f, 0.9f, 1f),
                    wingTint: new Color(0.6f, 0.55f, 0.85f, 1f),
                    cockpitTint: new Color(0.5f, 0.1f, 1f, 0.95f),
                    coreGlow: new Color(0.6f, 0.2f, 1f, 1f),
                    plasmaColor: new Color(0.5f, 0.2f, 1f, 1f),
                    weaponName: "Triple Tachyon Blasters",
                    weaponDamage: 28f, weaponFireRate: 0.10f
                )
            };
        }

        private FighterDefinition CreateDefinition(
            string id, string displayName, string className, string description,
            string tech, string special,
            float moveSpeed, float boostSpeed, float acceleration,
            float health, float shield, float energy,
            float mobilityRating, float firepowerRating, float defenseRating, float specialRating,
            float unlockMinutes, bool isDefault,
            Color theme, Color accent, Color hullTint, Color wingTint, Color cockpitTint, Color coreGlow, Color plasmaColor,
            string weaponName, float weaponDamage, float weaponFireRate)
        {
            var def = CreateInstance<FighterDefinition>();
            def.fighterId = id;
            def.displayName = displayName;
            def.className = className;
            def.description = description;
            def.signatureTechnology = tech;
            def.specialAbilityName = special;
            def.moveSpeed = moveSpeed;
            def.boostSpeed = boostSpeed;
            def.acceleration = acceleration;
            def.maxHealth = health;
            def.maxShield = shield;
            def.maxEnergy = energy;
            def.mobilityRating = mobilityRating;
            def.firepowerRating = firepowerRating;
            def.defenseRating = defenseRating;
            def.specialRating = specialRating;
            def.unlockRequiredMinutes = unlockMinutes;
            def.isDefault = isDefault;
            def.themeColor = theme;
            def.accentColor = accent;
            def.hullTint = hullTint;
            def.wingTint = wingTint;
            def.cockpitTint = cockpitTint;
            def.coreGlowColor = coreGlow;
            def.thrusterPlasmaColor = plasmaColor;

            // Map technology icon path
            switch (id.ToLowerInvariant())
            {
                case "vanguard":
                    def.techIconPath = "UI/Technology/UI_Tech_AdaptiveCore.png";
                    break;
                case "interceptor":
                    def.techIconPath = "UI/Technology/UI_Tech_VectorDrive.png";
                    break;
                case "striker":
                    def.techIconPath = "UI/Technology/UI_Tech_RailCore.png";
                    break;
                case "bulwark":
                    def.techIconPath = "UI/Technology/UI_Tech_BarrierMatrix.png";
                    break;
                case "phantom":
                case "void_wraith":
                    def.techIconPath = "UI/Technology/UI_Tech_PhaseCore.png";
                    break;
                case "apex":
                    def.techIconPath = "UI/Technology/UI_Tech_QuantumCore.png";
                    break;
                case "raven":
                    def.techIconPath = "UI/Technology/UI_Tech_RailCore.png";
                    break;
                default:
                    def.techIconPath = "UI/Technology/UI_Tech_AdaptiveCore.png";
                    break;
            }

            // Create default weapon definition
            var proj = CreateInstance<ProjectileDefinition>();
            proj.projectileId = $"{id}_projectile";
            proj.displayName = $"{displayName} Round";
            proj.baseDamage = weaponDamage;
            proj.projectileTint = theme;

            var weap = CreateInstance<WeaponDefinition>();
            weap.weaponId = $"{id}_weapon";
            weap.displayName = weaponName;
            weap.fireRate = weaponFireRate;
            weap.projectileDefinition = proj;
            weap.muzzleFlashColor = accent;
            def.defaultWeapon = weap;

            return def;
        }

        public FighterDefinition GetFighterById(string id)
        {
            InitializeDefaultDefinitions();
            return _fighters.Find(f => string.Equals(f.fighterId, id, StringComparison.OrdinalIgnoreCase)) ?? _fighters[0];
        }

        public FighterDefinition GetFighterByIndex(int index)
        {
            InitializeDefaultDefinitions();
            if (index >= 0 && index < _fighters.Count) return _fighters[index];
            return _fighters[0];
        }

        public int GetFighterIndex(string id)
        {
            InitializeDefaultDefinitions();
            return _fighters.FindIndex(f => string.Equals(f.fighterId, id, StringComparison.OrdinalIgnoreCase));
        }
    }
}
