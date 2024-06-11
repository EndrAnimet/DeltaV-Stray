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
        SubscribeLocalEvent<BloomComponent, ComponentInit>(OnBloomInit);
        SubscribeLocalEvent<BloomComponent, ComponentShutdown>(OnBloomShutdown);
//
        //SubscribeLocalEvent<BloomComponent, LocalPlayerAttachedEvent>(OnPlayerAttached);
        //SubscribeLocalEvent<BloomComponent, LocalPlayerDetachedEvent>(OnPlayerDetached);
//
        _overlay = new();
    }
//
    //private void OnPlayerAttached(EntityUid uid, BloomComponent component, LocalPlayerAttachedEvent args)
    //{
    //    _overlayMan.AddOverlay(_overlay);
    //}
///
    //private void OnPlayerDetached(EntityUid uid, BloomComponent component, LocalPlayerDetachedEvent args)
    //{
    //    //_overlay.CurrentBoozePower = 0;
    //    _overlayMan.RemoveOverlay(_overlay);
    //}
//
    private void OnBloomInit(EntityUid uid, BloomComponent component, ComponentInit args)
    {
        //if (_player.LocalEntity == uid)
        _overlayMan.AddOverlay(_overlay);
    }
//
    private void OnBloomShutdown(EntityUid uid, BloomComponent component, ComponentShutdown args)
    {
        //if (_player.LocalEntity == uid)
        //{
        //    _overlay.CurrentBoozePower = 0;
        _overlayMan.RemoveOverlay(_overlay);
        //}
    }
}
