using System.Numerics;
using Content.Server.Shuttles.Systems;
using Content.Shared.Damage;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.Shuttles.Components
{
    [RegisterComponent, NetworkedComponent]
    //[Access(typeof(ThrusterSystem))]
    public sealed partial class ThrusterComponent : Component
    {
        /// <summary>
        /// Whether the thruster has been force to be enabled / disabled (e.g. VV, interaction, etc.)
        /// </summary>
        [DataField, ViewVariables(VVAccess.ReadWrite)]
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// This determines whether the thruster is actually enabled for the purposes of thrust
        /// </summary>
        ///
        [ViewVariables(VVAccess.ReadWrite)]
        public bool InThrustArray;
        public bool IsOn;

        [ViewVariables(VVAccess.ReadWrite), DataField("fuelType")]
        public int FuelType = -1;
        [ViewVariables(VVAccess.ReadWrite)]
        public CombuctionComponent? ConnectedChamber;
        [ViewVariables(VVAccess.ReadWrite)]
        public bool HasFuel = true;

        // Need to serialize this because RefreshParts isn't called on Init and this will break post-mapinit maps!
        [ViewVariables(VVAccess.ReadWrite), DataField("thrust")]
        public float Thrust = 100f;

        [DataField("thrusterType")]
        public ThrusterType Type = ThrusterType.Linear;

        [DataField("burnShape")] public List<Vector2> BurnPoly = new()
        {
            new Vector2(-0.4f, 0.5f),
            new Vector2(-0.1f, 1.2f),
            new Vector2(0.1f, 1.2f),
            new Vector2(0.4f, 0.5f)
        };

        /// <summary>
        /// How much damage is done per second to anything colliding with our thrust.
        /// </summary>
        [DataField("damage")] public DamageSpecifier? Damage = new();

        [DataField("requireSpace")]
        public bool RequireSpace = true;

        // Used for burns

        public List<EntityUid> Colliding = new();

        public bool Firing = false;

        /// <summary>
        /// Next time we tick damage for anyone colliding.
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite), DataField("nextFire", customTypeSerializer:typeof(TimeOffsetSerializer))]
        public TimeSpan NextFire;
    }

    public enum ThrusterType
    {
        Linear,
        // Angular meaning rotational.
        Angular,
    }
}
