using Content.Shared.Stray.Bloom;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Player;

namespace Content.Client.Stray.Bloom;

public sealed class BloomSystem : SharedBloomSystem
{
    //[Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly IOverlayManager _overlayMan = default!;

    private BloomOverlay _overlay = default!;

    public override void Initialize()
    {
        base.Initialize();
//
        _overlay = new();
    }
//
}
