using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using FreeFramework.SDK.Communication;

namespace FreeFramework.SDK.Server.Communication;

public sealed class ServerNetworkEventBus(ServerEntrypoint entrypoint) : IServerNetworkEventBus {
    public void Publish<T>(int playerId, T evt) where T : IBusObject {
        string eventName = "__FFE:" + typeof(T).AssemblyQualifiedName;
        Player player = entrypoint.EntryPlayers[playerId];
        BaseScript.TriggerClientEvent(player, eventName, evt);
    }

    public void Publish<T>(IEnumerable<int> playerIds, T evt) where T : IBusObject {
        string eventName = "__FFE:" + typeof(T).Namespace + "." + typeof(T).Name;
        IEnumerable<Player> players = playerIds.Select(id => entrypoint.EntryPlayers[id]);

        foreach (Player player in players) {
            BaseScript.TriggerClientEvent(player, eventName, evt);
        }
    }

    public void Publish<T>(T evt) where T : IBusObject {
        string eventName = "__FFE:" + typeof(T).Namespace + "." + typeof(T).Name;

        foreach (Player player in entrypoint.EntryPlayers) {
            BaseScript.TriggerClientEvent(player, eventName, evt);
        }
    }

    public void Subscribe<T>(Action<int, T> callback) where T : IBusObject {
        string key = "__FFE:" + typeof(T).Namespace + "." + typeof(T).Name;

        entrypoint.EntryEventHandlers[key] += ([FromSource] Player player, T evt) => {
            callback(int.Parse(player.Handle), evt);
        };
    }
}