using Robust.Shared.Serialization;

namespace Content.Shared.Shuttles.Components
{
    [Serializable, NetSerializable]
    public enum CombuctionVisualLayers : byte
    {
        Base,
        Burning,
    }

    [Serializable, NetSerializable]
    public enum CombuctionVisuals : byte
    {
        Burning
    }
}
