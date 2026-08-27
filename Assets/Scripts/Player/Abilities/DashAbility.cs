using System.Collections;
using UnityEngine;
using SpaceShooter.Input;

namespace SpaceShooter.Player.Abilities
{
    public class DashAbility : PlayerAbilityBase
    {
        [Header("Dash Dynamics")]
        [SerializeField] private float _dashSpeed = 36f;
        [SerializeField] private float _dashDuration = 0.22f;

        private void Reset()
        {
            _abilityId = "ability_dash";
            _abilityName = "Vector Dash";
            _cooldownDuration = 3.5f;
            _energyCost = 20f;
        }

        protected override void ExecuteAbility(PlayerController player)
        {
            StartCoroutine(DashRoutine(player));
        }

        private IEnumerator DashRoutine(PlayerController player)
        {
            HapticFeedback.TriggerMedium();

            Vector2 moveDir = InputManager.Instance != null ? InputManager.Instance.MoveInput : Vector2.up;
            if (moveDir.sqrMagnitude < 0.01f) moveDir = player.transform.up;
            moveDir.Normalize();

            player.Health?.SetInvulnerable(true);
            player.Visuals?.TriggerHitFlash(new Color(0.2f, 0.8f, 1f, 0.8f));

            // Apply single clean directional dash impulse
            player.Movement?.ApplyImpulse(moveDir * _dashSpeed);

            yield return new WaitForSeconds(_dashDuration);

            player.Health?.SetInvulnerable(false);
            EndAbility();
        }
    }
}
