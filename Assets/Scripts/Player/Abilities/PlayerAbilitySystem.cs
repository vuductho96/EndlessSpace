using System;
using System.Collections.Generic;
using UnityEngine;
using SpaceShooter.Fighters.Data;
using SpaceShooter.Player.Interfaces;

namespace SpaceShooter.Player.Abilities
{
    public class PlayerAbilitySystem : MonoBehaviour, IPlayerAbilitySystem
    {
        private readonly List<IPlayerAbility> _abilities = new List<IPlayerAbility>();
        public IReadOnlyList<IPlayerAbility> Abilities => _abilities;
        public IPlayerAbility PrimaryAbility => _abilities.Count > 0 ? _abilities[0] : null;

        private PlayerController _player;

        public event Action<IPlayerAbility> OnAbilityRegistered;

        private void Awake()
        {
            _player = GetComponent<PlayerController>();
            DiscoverAbilities();
        }

        public void Initialize(PlayerController player, FighterDefinition definition)
        {
            _player = player;
            DiscoverAbilities();
        }

        public void DiscoverAbilities()
        {
            _abilities.Clear();
            var found = GetComponentsInChildren<IPlayerAbility>(true);
            foreach (var ab in found)
            {
                _abilities.Add(ab);
                OnAbilityRegistered?.Invoke(ab);
            }
        }

        private void Update()
        {
            if (SpaceShooter.Input.InputManager.Instance != null)
            {
                if (SpaceShooter.Input.InputManager.Instance.IsAbilityTriggered || SpaceShooter.Input.InputManager.Instance.TouchAbilityPressed)
                {
                    TryActivatePrimary();
                }
            }
        }

        public bool TryActivatePrimary()
        {
            if (PrimaryAbility != null && _player != null)
            {
                return PrimaryAbility.TryActivate(_player);
            }
            return false;
        }

        public bool TryActivateAbility(string abilityId)
        {
            if (_player == null) return false;
            var ab = _abilities.Find(a => string.Equals(a.AbilityId, abilityId, StringComparison.OrdinalIgnoreCase));
            if (ab != null)
            {
                return ab.TryActivate(_player);
            }
            return false;
        }
    }
}
