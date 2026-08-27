using System;
using System.Collections.Generic;
using UnityEngine;
using SpaceShooter.Combat;
using SpaceShooter.Combat.Data;
using SpaceShooter.Fighters.Data;

namespace SpaceShooter.Player.Interfaces
{
    public interface IPlayerMovement
    {
        Vector2 CurrentVelocity { get; }
        float ThrottleRatio { get; }
        bool IsBoosting { get; }
        float MoveSpeed { get; }
        float Acceleration { get; }
        void Initialize(FighterDefinition definition);
        void ApplyImpulse(Vector2 impulseForce);
        void SetMovementLock(bool isLocked);
    }

    public interface IHealthSystem
    {
        float CurrentHealth { get; }
        float MaxHealth { get; }
        float HealthRatio { get; }
        bool IsDead { get; }
        event Action<float, float> OnHealthChanged;
        event Action OnDeath;
        void Initialize(FighterDefinition definition);
        void Heal(float amount);
        void SetInvulnerable(bool invulnerable);
    }

    public interface IShieldSystem
    {
        float CurrentShield { get; }
        float MaxShield { get; }
        float ShieldRatio { get; }
        bool IsShieldActive { get; }
        event Action<float, float> OnShieldChanged;
        event Action OnShieldBroken;
        event Action OnShieldRestored;
        void Initialize(FighterDefinition definition);
        bool AbsorbDamage(ref float incomingDamage);
        void RestoreShield(float amount);
    }

    public interface IWeaponSystem
    {
        WeaponDefinition CurrentWeapon { get; }
        int MountCount { get; }
        bool IsFiring { get; }
        void Initialize(FighterDefinition definition, WeaponDefinition overrideWeapon = null);
        void Fire();
        void StopFiring();
        void SetWeapon(WeaponDefinition newWeapon);
    }

    public interface IPlayerAbility
    {
        string AbilityId { get; }
        string AbilityName { get; }
        Sprite AbilityIcon { get; }
        float CooldownDuration { get; }
        float CooldownRemaining { get; }
        float CooldownRatio { get; }
        float EnergyCost { get; }
        bool IsReady { get; }
        bool IsActive { get; }
        event Action<IPlayerAbility> OnAbilityActivated;
        event Action<IPlayerAbility> OnAbilityEnded;
        bool CanActivate(PlayerController player);
        bool TryActivate(PlayerController player);
    }

    public interface IPlayerAbilitySystem
    {
        IReadOnlyList<IPlayerAbility> Abilities { get; }
        IPlayerAbility PrimaryAbility { get; }
        event Action<IPlayerAbility> OnAbilityRegistered;
        void Initialize(PlayerController player, FighterDefinition definition);
        bool TryActivatePrimary();
        bool TryActivateAbility(string abilityId);
    }

    public interface IFighterVisuals
    {
        void Initialize(FighterDefinition definition);
        void TriggerHitFlash(Color flashColor);
        void TriggerShieldImpact(Vector2 impactPoint);
        void TriggerDamageSmoke(float damageRatio);
        void TriggerDeathExplosion();
    }
}
