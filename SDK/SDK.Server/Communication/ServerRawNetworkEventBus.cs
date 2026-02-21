using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using FreeFramework.SDK.Communication;

namespace FreeFramework.SDK.Server.Communication;

public sealed class ServerRawNetworkEventBus(ServerEntrypoint entrypoint) : IServerRawNetworkEventBus {
    public void Publish(int playerId, string channel, params object[] args) {
        Player player = entrypoint.EntryPlayers[playerId];
        BaseScript.TriggerClientEvent(player, channel, args);
    }

    public void Publish(IEnumerable<int> playerIds, string channel, params object[] args) {
        IEnumerable<Player> players = playerIds.Select(id => entrypoint.EntryPlayers[id]);

        foreach (Player player in players) {
            BaseScript.TriggerClientEvent(player, channel, args);
        }
    }

    public void Publish(string channel, params object[] args) {
        foreach (Player player in entrypoint.EntryPlayers) {
            BaseScript.TriggerClientEvent(player, channel, args);
        }
    }

    public void Subscribe(string channel, Delegate callback) {
        entrypoint.EntryEventHandlers[channel] += callback;
    }
}