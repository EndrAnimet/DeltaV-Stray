﻿using Robust.Shared.Configuration;

namespace Content.Shared.DeltaV.CCVars;

/// <summary>
/// DeltaV specific cvars.
/// </summary>
[CVarDefs]
// ReSharper disable once InconsistentNaming - Shush you
public sealed class DCCVars
{
    /// <summary>
    /// Anti-EORG measure. Will add pacified to all players upon round end.
    /// Its not perfect, but gets the job done.
    /// </summary>
    public static readonly CVarDef<bool> RoundEndPacifist =
        CVarDef.Create("game.round_end_pacifist", false, CVar.SERVERONLY);

    /// <summary>
    /// Disables all vision filters for species like Vulpkanin or Harpies. There are good reasons someone might want to disable these.
    /// </summary>
    public static readonly CVarDef<bool> NoVisionFilters =
        CVarDef.Create("accessibility.no_vision_filters", true, CVar.CLIENTONLY | CVar.ARCHIVE);

    /// <summary>
    /// Whether the Shipyard is enabled.
    /// </summary>
    public static readonly CVarDef<bool> Shipyard =
        CVarDef.Create("shuttle.shipyard", true, CVar.SERVERONLY);

    /// <summary>
    /// Prevent VPN and proxy connections from joining
    /// </summary
    public static readonly CVarDef<bool> BlockProxyConnections =
        CVarDef.Create("game.deltav.blockproxy", false, CVar.SERVERONLY);

    /// <summary>
    /// neutrinoapi.com API user for proxy detection
    /// </summary>
    public static readonly CVarDef<string> ProxyApiUser =
        CVarDef.Create("game.deltav.proxyapiuser", "", CVar.SERVERONLY);

    /// <summary>
    /// neutrinoapi.com API key for proxy detection
    /// </summary>
    public static readonly CVarDef<string> ProxyApiKey =
        CVarDef.Create("game.deltav.proxyapikey", "", CVar.SERVERONLY | CVar.CONFIDENTIAL);

    /// <summary>
    /// Probe IPs?
    /// </summary>
    public static readonly CVarDef<bool> ProxyProbe =
        CVarDef.Create("game.deltav.proxyprobe", false, CVar.SERVERONLY);
}
