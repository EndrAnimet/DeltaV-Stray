using Content.Shared.Speech.EntitySystems;
using Content.Shared.StatusEffect;
using Content.Shared.Traits.Assorted;

namespace Content.Shared.Stray.Bloom;

public abstract class SharedBloomSystem : EntitySystem
{
    [ValidatePrototypeId<StatusEffectPrototype>]
    public const string BloomKey = "Bloom";
//
    //[Dependency] private readonly StatusEffectsSystem _statusEffectsSystem = default!;
    //[Dependency] private readonly SharedSlurredSystem _slurredSystem = default!;
//
    //public void TryApplyBloomenness(EntityUid uid, float boozePower, bool applySlur = true,
    //    StatusEffectsComponent? status = null)
    //{
    //    if (!Resolve(uid, ref status, false))
    //        return;
//
    //    //if (TryComp<LightweightDrunkComponent>(uid, out var trait))
    //    boozePower *= 4;//trait.BoozeStrengthMultiplier;
//
    //    if (applySlur)
    //    {
    //        _slurredSystem.DoSlur(uid, TimeSpan.FromSeconds(boozePower), status);
    //    }
//
    //    if (!_statusEffectsSystem.HasStatusEffect(uid, BloomKey, status))
    //    {
    //        _statusEffectsSystem.TryAddStatusEffect<BloomComponent>(uid, BloomKey, TimeSpan.FromSeconds(boozePower), true, status);
    //    }
    //}
//
    //public void TryRemoveBloomenness(EntityUid uid)
    //{
    //    _statusEffectsSystem.TryRemoveStatusEffect(uid, BloomKey);
    //}
    //public void TryRemoveBloomenessTime(EntityUid uid, double timeRemoved)
    //{
    //    _statusEffectsSystem.TryRemoveTime(uid, BloomKey, TimeSpan.FromSeconds(timeRemoved));
    //}

}
