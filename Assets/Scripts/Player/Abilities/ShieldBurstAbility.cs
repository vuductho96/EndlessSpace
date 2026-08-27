using UnityEngine;
using SpaceShooter.Combat;
using SpaceShooter.Input;

namespace SpaceShooter.Player.Abilities
{
    public class ShieldBurstAbility : PlayerAbilityBase
    {
        [Header("Burst Settings")]
        [SerializeField] private float _shieldRestoreAmount = 50f;
        [SerializeField] private float _shockwaveRadius = 6f;
        [SerializeField] private float _shockwaveDamage = 40f;
        [SerializeField] private float _knockbackForce = 18f;

        private void Reset()
        {
            _abilityId = "ability_shield_burst";
            _abilityName = "Fortress Shield Burst";
            _cooldownDuration = 8f;
            _energyCost = 35f;
        }

        protected override void ExecuteAbility(PlayerController player)
        {
            HapticFeedback.TriggerHeavy();

            // Restore shield
            player.Shield?.RestoreShield(_shieldRestoreAmount);

            // Kinetic shockwave against enemies
            Collider2D[] hits = Physics2D.OverlapCircleAll(player.transform.position, _shockwaveRadius);
            foreach (var hit in hits)
            {
                if (hit.transform == player.transform) continue;

                var dmg = hit.GetComponent<IDamageable>();
                dmg?.TakeDamage(_shockwaveDamage);

                var rb = hit.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 dir = (hit.transform.position - player.transform.position).normalized;
                    rb.AddForce(dir * _knockbackForce, ForceMode2D.Impulse);
                }
            }

            player.Visuals?.TriggerShieldImpact(player.transform.position);
            EndAbility();
        }
    }
}
