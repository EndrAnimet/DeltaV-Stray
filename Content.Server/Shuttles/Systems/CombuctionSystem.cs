using System.Linq;
using Content.Server.Atmos.Piping.Components;
using Content.Server.Audio;
using Content.Server.Construction;
using Content.Server.NodeContainer;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Server.Shuttles.Components;
using Content.Shared.Construction.Components;
using Content.Shared.Damage;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Content.Shared.Shuttles.Components;
using Content.Shared.Temperature;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Components;
using Content.Server.NodeContainer.Nodes;
using Content.Server.Popups;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Components;
using Content.Server.Nutrition.Components;
using Content.Server.NodeContainer.EntitySystems;
using System.Net.Http;
using Content.Server.Morgue.Components;

namespace Content.Server.Shuttles.Systems
{
    public sealed class CombuctionSystem : EntitySystem
    {
        [Dependency] private readonly IGameTiming _timing = default!;
        [Dependency] private readonly IMapManager _mapManager = default!;
        [Dependency] private readonly EntityLookupSystem _lookup = default!;
        [Dependency] private readonly AtmosphereSystem _atmo = default!;
        [Dependency] private readonly NodeContainerSystem _nodeContainer = default!;
        [Dependency] private readonly SharedAppearanceSystem _appearance = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<CombuctionComponent, ComponentInit>(OnChamberInit);
            SubscribeLocalEvent<CombuctionComponent, BeforeAnchoredEvent>(OnChamberAnchored);
            SubscribeLocalEvent<CombuctionComponent, BeforeUnanchoredEvent>(OnChamberUnanchored);
            SubscribeLocalEvent<CombuctionComponent, ExaminedEvent>(OnChamberExamine);

            SubscribeLocalEvent<CombuctionComponent, RefreshPartsEvent>(OnRefreshParts);
            SubscribeLocalEvent<CombuctionComponent, UpgradeExamineEvent>(OnUpgradeExamine);
        }

        public override void Update(float frameTime)
        {
            base.Update(frameTime);


            var query = EntityQueryEnumerator<CombuctionComponent>();
            var curTime = _timing.CurTime;

            while (query.MoveNext(out var comp))
            {
                if (comp.Burning && comp.NextBurn > curTime) BurnFuel(comp);

                comp.NextBurn = curTime + TimeSpan.FromSeconds(0.1f);
            }
        }

        public void RefreshChambers()
        {
            var query = EntityQueryEnumerator<CombuctionComponent>();

            while (query.MoveNext(out var comp))
            {
                comp.ConnectedThruster = TryFindThruster(Transform(comp.Owner));
                if (comp.ConnectedThruster != null) comp.ConnectedThruster.ConnectedChamber = comp;
                comp.Test++;
            }
        }

        private void OnChamberExamine(EntityUid uid, CombuctionComponent component, ExaminedEvent agrs)
        {
            var thrusterString = Loc.GetString(component.ConnectedThruster != null ? "chamber-comp-has-thruster" : "chamber-comp-not-has-truster");

            agrs.PushMarkup(thrusterString);

            if (component.ConnectedThruster != null)
            {
                bool hasFuel = CheckFuel(component, out float fuelMoles);
                var fuelString = "";

                if (hasFuel)
                {
                    fuelString += fuelMoles.ToString() + " ";
                    fuelString += Loc.GetString("chamber-comp-has-fuel");
                }
                else
                    fuelString = Loc.GetString("chamber-comp-not-has-fuel");

                agrs.PushMarkup(fuelString);
            }
        }

        private void OnChamberInit(EntityUid uid, CombuctionComponent component, ComponentInit args)
        {
            component.ConnectedThruster = TryFindThruster(Transform(uid));
            if (component.ConnectedThruster != null) component.ConnectedThruster.ConnectedChamber = component;
        }

        private void OnChamberAnchored(EntityUid uid, CombuctionComponent component, BeforeAnchoredEvent args)
        {
            component.ConnectedThruster = TryFindThruster(Transform(uid));
            if (component.ConnectedThruster != null) component.ConnectedThruster.ConnectedChamber = component;
        }

        private void OnChamberUnanchored(EntityUid uid, CombuctionComponent component, BeforeUnanchoredEvent args)
        {
            TryEndBurn(component);

            if (component.ConnectedThruster != null) component.ConnectedThruster.ConnectedChamber = null;
            component.ConnectedThruster = null;
        }

        private void OnRefreshParts(EntityUid uid, CombuctionComponent component, RefreshPartsEvent args)
        {
            var burningRating = args.PartRatings[component.MachinePartBurn];
            component.Burn = component.BaseBurn / burningRating;
        }

        private void OnUpgradeExamine(EntityUid uid, CombuctionComponent component, UpgradeExamineEvent args)
        {
            args.AddPercentageUpgrade("chamber-comp-upgrade", component.Burn / component.BaseBurn);
        }

        private ThrusterComponent? TryFindThruster(TransformComponent xform)
        {
            if (xform.GridUid == null) return null;

            var (x, y) = xform.LocalPosition + xform.LocalRotation.ToWorldVec();
            var tile = _mapManager.GetGrid(xform.GridUid.Value).GetTileRef(new Vector2i((int) Math.Floor(x), (int) Math.Floor(y)));

            var entities = tile.GetEntitiesInTile().ToArray();
            foreach (var entity in entities)
            {
                if (EntityManager.TryGetComponent<ThrusterComponent>(entity, out ThrusterComponent? result))
                {
                    if (result.FuelType != -1 && (Transform(entity).LocalRotation + 180).EqualsApprox(xform.LocalRotation + 180, 10)) return result;
                }
            }

            return null;
        }

        public bool TryStartBurn(CombuctionComponent? component)
        {
            if (component == null || component.ConnectedThruster == null) return false;

            if (CheckFuel(component, out float fuelMoles))
            {
                _nodeContainer.TryGetNode(EntityManager.GetComponent<NodeContainerComponent>(component.Owner), component.InletName, out PipeNode? node);
                if (node != null) component.ChamberMixture = node.Air;
                component.Burning = true;

                ChangeThrust(component.ConnectedThruster, true);

                if (EntityManager.TryGetComponent(component.Owner, out AppearanceComponent? appearance))
                    _appearance.SetData(component.Owner, CombuctionVisuals.Burning, true, appearance);

                return true;
            }

            ChangeThrust(component.ConnectedThruster, false);
            return false;
        }

        public void TryEndBurn(CombuctionComponent? component)
        {
            if (component == null) return;

            component.Burning = false;

            ChangeThrust(component.ConnectedThruster, false);

            if (EntityManager.TryGetComponent(component.Owner, out AppearanceComponent? appearance))
                _appearance.SetData(component.Owner, CombuctionVisuals.Burning, false, appearance);
        }

        public void BurnFuel(CombuctionComponent component)
        {
            if (component.ChamberMixture == null || component.ConnectedThruster == null)
                return;

            float fuelMoles = component.ChamberMixture.GetMoles(component.ConnectedThruster.FuelType);
            if (fuelMoles >= component.Burn)
            {
                component.ChamberMixture.SetMoles(component.ConnectedThruster.FuelType, fuelMoles - component.Burn);
            }
            else
                TryEndBurn(component);
        }

        public void ChangeThrust(ThrusterComponent? component, bool hasFuel)
        {
            if (component == null) return;

            TransformComponent xform = Transform(component.Owner);
            if (EntityManager.TryGetComponent<ShuttleComponent>(xform.GridUid, out ShuttleComponent? shuttle))
            {
                //if (component.ConnectedThruster.HasFuel == hasFuel) return;

                component.HasFuel = hasFuel;
                if (hasFuel && !component.InThrustArray)
                {
                    shuttle.LinearThrust[(int) Transform(component.Owner).LocalRotation.GetCardinalDir() / 2] += component.Thrust;
                    component.InThrustArray = true;
                }
                else if (component.InThrustArray)
                {
                    shuttle.LinearThrust[(int) Transform(component.Owner).LocalRotation.GetCardinalDir() / 2] -= component.Thrust;
                    component.InThrustArray = false;
                }
            }
        }

        public bool CheckFuel(CombuctionComponent? component, out float fuelMoles)
        {
            fuelMoles = 0;

            if (component == null || component.ConnectedThruster == null)
                return false;
            if (!EntityManager.TryGetComponent(component.Owner, out NodeContainerComponent? nodeContainer))
                return false;

            if (!_nodeContainer.TryGetNode(nodeContainer, component.InletName, out PipeNode? node))
                return false;

            if (node.Air.GetMoles(component.ConnectedThruster.FuelType) >= component.Burn)
            {
                fuelMoles = node.Air.GetMoles(component.ConnectedThruster.FuelType);
                return true;
            }

            return false;
        }
    }
}
