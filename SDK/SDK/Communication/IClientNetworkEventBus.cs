using System;

namespace FreeFramework.SDK.Communication;

/// <summary>
/// Network event bus used for sending and receiving typed events to and from the server.
///
/// Server -> Client
/// Client -> Server
/// </summary>
public interface IClientNetworkEventBus {
    /// <summary>
    /// Publishes an event to the server
    /// </summary>
    /// <param name="evt">The event object</param>
    /// <typeparam name="T">The event object type</typeparam>
    void Publish<T>(T evt) where T : IBusObject;

    /// <summary>
    /// Subscribes an action to a specific type
    /// </summary>
    /// <param name="callback">The action to bind the event to.</param>
    /// <typeparam name="T">The event object type</typeparam>
    void Subscribe<T>(Action<T> callback) where T : IBusObject;
}