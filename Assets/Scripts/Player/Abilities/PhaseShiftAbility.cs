using System.Collections;
using UnityEngine;
using SpaceShooter.Input;

namespace SpaceShooter.Player.Abilities
{
    public class PhaseShiftAbility : PlayerAbilityBase
    {
        [Header("Phase Settings")]
        [SerializeField] private float _phaseDuration = 2.0f;
        [SerializeField] private float _phaseSpeedBoost = 1.35f;

        private void Reset()
        {
            _abilityId = "ability_phase_shift";
            _abilityName = "Dimensional Phase Shift";
            _cooldownDuration = 7.0f;
            _energyCost = 30f;
        }

        protected override void ExecuteAbility(PlayerController player)
        {
            StartCoroutine(PhaseRoutine(player));
        }

        private IEnumerator PhaseRoutine(PlayerController player)
        {
            HapticFeedback.TriggerMedium();

            player.Health?.SetInvulnerable(true);
            player.Visuals?.TriggerHitFlash(new Color(0.7f, 0.2f, 1f, 0.5f));

            if (player.Movement != null)
            {
                player.Movement.ApplyImpulse(player.transform.up * (_phaseSpeedBoost * 6f));
            }

            yield return new WaitForSeconds(_phaseDuration);

            player.Health?.SetInvulnerable(false);
            EndAbility();
        }
    }
}
