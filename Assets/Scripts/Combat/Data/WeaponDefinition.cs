using UnityEngine;

namespace SpaceShooter.Combat.Data
{
    [CreateAssetMenu(fileName = "WeaponDefinition", menuName = "SpaceShooter/Combat/Weapon Definition")]
    public class WeaponDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string weaponId = "plasma_converters_dual";
        public string displayName = "Dual Plasma Converters";
        public string category = "Plasma Repeater";

        [Header("Projectile Config")]
        public ProjectileDefinition projectileDefinition;

        [Header("Firing Mechanics")]
        [Tooltip("Time between shots in seconds")]
        public float fireRate = 0.12f;
        public float damageMultiplier = 1.0f;
        public float projectileSpeedMultiplier = 1.0f;
        public int projectilesPerMount = 1;
        public float spreadAngle = 0f;

        [Header("Burst Settings")]
        public bool isBurst = false;
        public int burstCount = 3;
        public float burstInterval = 0.05f;

        [Header("Energy & Thermal Cost")]
        public float energyCost = 2.5f;
        public float heatPerShot = 3.5f;

        [Header("Audio & FX")]
        public string fireAudioClip = "laser_fire";
        public Color muzzleFlashColor = new Color(0f, 1f, 1f, 1f);
    }
}
