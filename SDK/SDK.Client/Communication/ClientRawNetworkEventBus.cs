using System;
using CitizenFX.Core;
using FreeFramework.SDK.Communication;

namespace FreeFramework.SDK.Client.Communication;

public sealed class ClientRawNetworkEventBus(ClientEntrypoint entrypoint) : IClientRawNetworkEventBus {
    public void Publish(string channel, params object[] args) {
        BaseScript.TriggerServerEvent(channel, args);
    }

    public void Subscribe(string channel, Delegate callback) {
        entrypoint.EntryEventHandlers[channel] += callback;
    }
}