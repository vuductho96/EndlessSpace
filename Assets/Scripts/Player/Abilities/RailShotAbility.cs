using UnityEngine;
using SpaceShooter.Combat;
using SpaceShooter.Input;

namespace SpaceShooter.Player.Abilities
{
    public class RailShotAbility : PlayerAbilityBase
    {
        [Header("Rail Parameters")]
        [SerializeField] private float _railDamage = 150f;
        [SerializeField] private float _beamLength = 30f;
        [SerializeField] private float _beamWidth = 0.8f;

        private void Reset()
        {
            _abilityId = "ability_rail_shot";
            _abilityName = "Overcharge Rail Cannon";
            _cooldownDuration = 6.0f;
            _energyCost = 40f;
        }

        protected override void ExecuteAbility(PlayerController player)
        {
            HapticFeedback.TriggerHeavy();

            Vector2 origin = player.transform.position;
            Vector2 direction = player.transform.up;

            RaycastHit2D[] hits = Physics2D.CircleCastAll(origin, _beamWidth * 0.5f, direction, _beamLength);
            foreach (var hit in hits)
            {
                if (hit.transform == player.transform) continue;

                var dmg = hit.collider.GetComponent<IDamageable>();
                dmg?.TakeDamage(_railDamage);
            }

            player.Visuals?.TriggerHitFlash(new Color(1f, 0.4f, 0.1f, 1f));
            EndAbility();
        }
    }
}
