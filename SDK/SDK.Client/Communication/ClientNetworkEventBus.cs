using System;
using CitizenFX.Core;
using FreeFramework.SDK.Communication;

namespace FreeFramework.SDK.Client.Communication;

public sealed class ClientNetworkEventBus(ClientEntrypoint entrypoint) : IClientNetworkEventBus {
    public void Publish<T>(T evt) where T : IBusObject {
        string eventName = "__FFE:" + typeof(T).Namespace + "." + typeof(T).Name;
        BaseScript.TriggerServerEvent(eventName, evt);
    }

    public void Subscribe<T>(Action<T> callback) where T : IBusObject {
        string key = "__FFE:" + typeof(T).Namespace + "." + typeof(T).Name;
        entrypoint.EntryEventHandlers[key] += callback;
    }
}