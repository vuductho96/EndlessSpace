using UnityEngine;
using SpaceShooter.Fighters.Data;
using SpaceShooter.Player.Interfaces;
using SpaceShooter.Player.Visuals;
using SpaceShooter.Player.Abilities;

namespace SpaceShooter.Player
{
    /// <summary>
    /// Master player fighter controller shared by ALL spacecraft prefabs.
    /// Uses pure component-based composition with ZERO fighter-specific hardcoding or switch statements.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController ActivePlayer { get; private set; }

        [Header("Configuration")]
        [SerializeField] private FighterDefinition _currentDefinition;
        public FighterDefinition CurrentDefinition => _currentDefinition;

        [Header("Constant World Scale")]
        [Range(0.1f, 1.0f)]
        [SerializeField] private float _playerScale = 0.3f;
        public float PlayerScale => _playerScale;

        // Shared Generic Subsystem References
        public IPlayerMovement Movement { get; private set; }
        public IHealthSystem Health { get; private set; }
        public IShieldSystem Shield { get; private set; }
        public IWeaponSystem WeaponSystem { get; private set; }
        public IPlayerAbilitySystem AbilitySystem { get; private set; }
        public IFighterVisuals Visuals { get; private set; }
        public ThrusterController Thrusters { get; private set; }
        public PlayerStats Stats { get; private set; }

        // Backward compatibility for existing systems
        public PlayerWeapon Weapon => GetComponent<PlayerWeapon>();

        private void Awake()
        {
            ActivePlayer = this;
            gameObject.tag = "Player";
            BindSubsystems();
            ApplyScale();
        }

        public void BindSubsystems()
        {
            Movement = GetComponent<IPlayerMovement>();
            Health = GetComponent<IHealthSystem>();
            Shield = GetComponent<IShieldSystem>();
            WeaponSystem = GetComponent<IWeaponSystem>();
            AbilitySystem = GetComponent<IPlayerAbilitySystem>();
            Visuals = GetComponent<IFighterVisuals>();
            Thrusters = GetComponentInChildren<ThrusterController>();
            Stats = GetComponent<PlayerStats>();
        }

        public void Initialize(FighterDefinition definition)
        {
            _currentDefinition = definition;
            BindSubsystems();

            // Inject data slices into each component
            Movement?.Initialize(definition);
            Health?.Initialize(definition);
            Shield?.Initialize(definition);
            WeaponSystem?.Initialize(definition);
            AbilitySystem?.Initialize(this, definition);
            Visuals?.Initialize(definition);
            Thrusters?.Initialize(definition);
            Stats?.InitializeFromDefinition(definition);

            ApplyScale();
            Debug.Log($"<color=#00FFCC><b>[PlayerController]</b></color> Initialized generic fighter: <b>{definition?.displayName}</b> (Mounts: {WeaponSystem?.MountCount ?? 0})");
        }

        public void SetShipScale(float scale)
        {
            _playerScale = Mathf.Clamp(scale, 0.1f, 1.0f);
            ApplyScale();
        }

        private void ApplyScale()
        {
            transform.localScale = new Vector3(_playerScale, _playerScale, 1f);
        }

        private void OnDestroy()
        {
            if (ActivePlayer == this) ActivePlayer = null;
        }
    }
}
