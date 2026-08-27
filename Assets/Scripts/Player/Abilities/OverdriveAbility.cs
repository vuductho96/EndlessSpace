using System.Collections;
using UnityEngine;
using SpaceShooter.Input;

namespace SpaceShooter.Player.Abilities
{
    public class OverdriveAbility : PlayerAbilityBase
    {
        [Header("Overdrive Settings")]
        [SerializeField] private float _overdriveDuration = 4.0f;

        private void Reset()
        {
            _abilityId = "ability_overdrive";
            _abilityName = "Quantum Overdrive";
            _cooldownDuration = 9.0f;
            _energyCost = 45f;
        }

        protected override void ExecuteAbility(PlayerController player)
        {
            StartCoroutine(OverdriveRoutine(player));
        }

        private IEnumerator OverdriveRoutine(PlayerController player)
        {
            HapticFeedback.TriggerHeavy();
            player.Visuals?.TriggerHitFlash(new Color(1f, 0.9f, 0.2f, 1f));

            var weaponSystem = player.WeaponSystem as PlayerWeaponSystem;
            if (weaponSystem != null)
            {
                // Double fire rate (0.5x delay), 75% less heat, 15% damage bonus
                weaponSystem.SetTemporaryBuffs(fireRateMultiplier: 0.5f, heatMultiplier: 0.25f, damageMultiplier: 1.15f);
            }

            yield return new WaitForSeconds(_overdriveDuration);

            if (weaponSystem != null)
            {
                weaponSystem.ResetTemporaryBuffs();
            }

            EndAbility();
        }
    }
}
