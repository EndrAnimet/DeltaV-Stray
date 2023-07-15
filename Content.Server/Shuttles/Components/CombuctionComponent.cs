using Robust.Shared.GameStates;
using Content.Server.Shuttles.Systems;
using Content.Server.Atmos;
using Content.Shared.Construction.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Shuttles.Components
{
    [RegisterComponent]
    [Access(typeof(CombuctionSystem))]
    public sealed class CombuctionComponent : Component
    {
        [ViewVariables(VVAccess.ReadWrite)]
        public ThrusterComponent? ConnectedThruster;

        [ViewVariables(VVAccess.ReadWrite)]
        public string InletName = "inlet";

        [ViewVariables(VVAccess.ReadWrite), DataField("fuelType")]
        public float BaseBurn = 0.1f;
        [ViewVariables(VVAccess.ReadWrite)]
        public float Burn = 0.1f;
        [ViewVariables(VVAccess.ReadWrite)]
        public bool Burning;

        [DataField("machinePartBurn", customTypeSerializer: typeof(PrototypeIdSerializer<MachinePartPrototype>))]
        public string MachinePartBurn = "Capacitor";

        [ViewVariables(VVAccess.ReadWrite)]
        public int Test;

        public TimeSpan NextBurn;

        public GasMixture? ChamberMixture;
    }
}
