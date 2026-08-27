using UnityEngine;

namespace SpaceShooter.Combat.Data
{
    [CreateAssetMenu(fileName = "ProjectileDefinition", menuName = "SpaceShooter/Combat/Projectile Definition")]
    public class ProjectileDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string projectileId = "plasma_bolt_standard";
        public string displayName = "Plasma Bolt";

        [Header("Prefabs & Visuals")]
        public GameObject projectilePrefab;
        public Color projectileTint = new Color(0f, 1f, 1f, 1f);
        public float visualScale = 1.0f;
        public string spritePath = "PlayerFighter_Experimental/CombatFX/projectile_basic.png";

        [Header("Kinematics & Dynamics")]
        public float speed = 38f;
        public float lifetime = 2.2f;
        public float colliderRadius = 0.35f;

        [Header("Combat Payload")]
        public float baseDamage = 25f;
        public bool isPiercing = false;
        public int maxPierceCount = 1;
        public float explosionRadius = 0f;
        public float explosionDamage = 0f;

        [Header("Homing Guidance")]
        public bool isHoming = false;
        public float homingTurnRate = 180f;
        public float homingAcquireRadius = 15f;

        [Header("Impact FX")]
        public GameObject impactFxPrefab;
        public Color impactFxColor = new Color(0f, 1f, 0.9f, 1f);
    }
}
