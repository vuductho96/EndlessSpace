using System;
using UnityEngine;
using SpaceShooter.Combat.Data;

namespace SpaceShooter.Fighters.Data
{
    [CreateAssetMenu(fileName = "FighterDefinition", menuName = "SpaceShooter/Fighter/Fighter Definition")]
    public class FighterDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string fighterId = "vanguard";
        public string displayName = "VANGUARD";
        public string className = "BALANCED FIGHTER";
        [TextArea(2, 4)]
        public string description = "Frontline balanced superiority fighter featuring adaptable modular subsystems and xenon plasma propulsion.";
        public Sprite icon;
        public string signatureTechnology = "Adaptive Core";
        public string specialAbilityName = "Emergency System Surge";

        [Header("Technology & Visual Icons")]
        public string techIconPath = "UI/Technology/UI_Tech_AdaptiveCore.png";
        public Sprite techIconSprite;

        [Header("Prefabs")]
        [Tooltip("Prefab instantiated in combat gameplay.")]
        public GameObject gameplayPrefab;
        [Tooltip("Prefab or visual composition used for Hangar 3D preview.")]
        public GameObject previewPrefab;

        [Header("Progression")]
        public float unlockRequiredMinutes = 0f;
        public bool isDefault = false;

        [Header("Movement & Kinematics")]
        public float moveSpeed = 12f;
        public float boostSpeed = 22f;
        public float acceleration = 45f;
        public float deceleration = 25f;
        public float rotationSpeed = 720f;
        public float linearDamping = 1.5f;
        public float dashSpeed = 35f;
        public float dashDuration = 0.25f;
        public float dashCooldown = 3.5f;

        [Header("Health & Armor")]
        public float maxHealth = 100f;
        public float healthRegen = 0f;
        [Range(0f, 0.8f)]
        public float damageResistance = 0f;

        [Header("Shield Subsystem")]
        public float maxShield = 100f;
        public float shieldRechargeRate = 15f;
        public float shieldDelay = 3.0f;

        [Header("Energy & Thermal Subsystems")]
        public float maxEnergy = 100f;
        public float energyRegenRate = 25f;
        public float boostEnergyCostPerSec = 20f;
        public float maxHeat = 100f;
        public float heatDissipationRate = 20f;

        [Header("Default Armament")]
        public WeaponDefinition defaultWeapon;

        [Header("Tactical Ratings (0.0 to 1.0)")]
        [Range(0f, 1f)] public float mobilityRating = 0.65f;
        [Range(0f, 1f)] public float firepowerRating = 0.65f;
        [Range(0f, 1f)] public float defenseRating = 0.60f;
        [Range(0f, 1f)] public float specialRating = 0.60f;

        [Header("Visual Theming & Customization")]
        public Color themeColor = new Color(0f, 0.85f, 1f, 1f);
        public Color accentColor = new Color(1f, 0.7f, 0.1f, 1f);
        public Color hullTint = Color.white;
        public Color wingTint = Color.white;
        public Color cockpitTint = new Color(0f, 0.9f, 1f, 0.95f);
        public Color coreGlowColor = new Color(0f, 1f, 1f, 1f);
        public Color thrusterPlasmaColor = new Color(0.2f, 0.7f, 1f, 1f);

        [Header("Modular Asset Paths (For dynamic procedural compositions)")]
        public string hullSpritePath = "PlayerFighter/Ship/player_fighter_hull.png";
        public string wingLeftSpritePath = "PlayerFighter/Ship/player_fighter_wing_left.png";
        public string wingRightSpritePath = "PlayerFighter/Ship/player_fighter_wing_right.png";
        public string cockpitSpritePath = "PlayerFighter/Ship/player_fighter_cockpit.png";
        public string coreSpritePath = "PlayerFighter/Energy/energy_core.png";
        public string shieldSpritePath = "PlayerFighter/Shield/player_shield.png";

        // Backward compatibility getters for existing UI bindings
        public string id => fighterId;
        public string weaponType => defaultWeapon != null ? defaultWeapon.displayName : "Dual Plasma Converters";
        public string specialAbility => specialAbilityName;
        public float mobility => mobilityRating;
        public float firepower => firepowerRating;
        public float defense => defenseRating;
        public float special => specialRating;
        public float speedMultiplier => moveSpeed / 12f;
        public float healthMultiplier => maxHealth / 100f;
        public float shieldMultiplier => maxShield / 100f;
        public float damageMultiplier => defaultWeapon != null ? defaultWeapon.damageMultiplier : 1.0f;
        public float energyRegenMultiplier => energyRegenRate / 25f;
    }
}
