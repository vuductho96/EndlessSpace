using System;
using UnityEngine;
using SpaceShooter.Player.Interfaces;

namespace SpaceShooter.Player.Abilities
{
    public abstract class PlayerAbilityBase : MonoBehaviour, IPlayerAbility
    {
        [Header("Ability Metadata")]
        [SerializeField] protected string _abilityId = "ability_generic";
        [SerializeField] protected string _abilityName = "Tactical Ability";
        [SerializeField] protected Sprite _abilityIcon;
        [SerializeField] protected float _cooldownDuration = 5.0f;
        [SerializeField] protected float _energyCost = 25f;

        protected float _lastActivatedTime = -999f;
        protected bool _isActive = false;

        public string AbilityId => _abilityId;
        public string AbilityName => _abilityName;
        public Sprite AbilityIcon => _abilityIcon;
        public float CooldownDuration => _cooldownDuration;
        public float CooldownRemaining => Mathf.Max(0f, (_lastActivatedTime + _cooldownDuration) - Time.time);
        public float CooldownRatio => _cooldownDuration > 0f ? Mathf.Clamp01(CooldownRemaining / _cooldownDuration) : 0f;
        public float EnergyCost => _energyCost;
        public bool IsReady => CooldownRemaining <= 0.001f && !_isActive;
        public bool IsActive => _isActive;

        public event Action<IPlayerAbility> OnAbilityActivated;
        public event Action<IPlayerAbility> OnAbilityEnded;

        public virtual bool CanActivate(PlayerController player)
        {
            if (!IsReady) return false;
            if (player == null || player.Stats == null) return false;
            return player.Stats.CurrentEnergy >= _energyCost;
        }

        public virtual bool TryActivate(PlayerController player)
        {
            if (!CanActivate(player)) return false;

            if (player.Stats.ConsumeEnergy(_energyCost))
            {
                _lastActivatedTime = Time.time;
                _isActive = true;
                OnAbilityActivated?.Invoke(this);
                ExecuteAbility(player);
                return true;
            }
            return false;
        }

        protected abstract void ExecuteAbility(PlayerController player);

        protected virtual void EndAbility()
        {
            _isActive = false;
            OnAbilityEnded?.Invoke(this);
        }
    }
}
