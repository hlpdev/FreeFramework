using System;

namespace FreeFramework.SDK.Communication;

/// <summary>
/// Event bus for sending typed events locally.
///
/// Server -> Server
/// Client -> Client
/// </summary>
public interface IEventBus {
    /// <summary>
    /// Publishes an event locally
    /// </summary>
    /// <param name="evt">The event object</param>
    /// <typeparam name="T">The event object type</typeparam>
    void Publish<T>(T evt) where T : IBusObject;
    
    /// <summary>
    /// Subscribes an action to a specific event type
    /// </summary>
    /// <param name="callback">The action to bind the event to</param>
    /// <typeparam name="T">The event object type</typeparam>
    void Subscribe<T>(Action<T> callback) where T : IBusObject;
}